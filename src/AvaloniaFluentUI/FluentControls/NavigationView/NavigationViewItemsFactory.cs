using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Element factory for the ItemsRepeaters in a NavigationView
/// </summary>
internal class NavigationViewItemsFactory : ElementFactory
{
    public NavigationViewItemBase SettingsItem { set => _settingsItem = value; }

    public void UserElementFactory(object newValue)
    {
        _itemTemplateWrapper = newValue as IElementFactory;
        if (_itemTemplateWrapper == null)
        {
            // ItemTemplate set does not implement IElementFactoryShim. We also 
            // want to support DataTemplate and DataTemplateSelectors automagically.
            if (newValue is IDataTemplate dt)
            {
                _itemTemplateWrapper = new ItemTemplateWrapper(dt);
            }
            else if (newValue is DataTemplateSelector dts)
            {
                _itemTemplateWrapper = new ItemTemplateWrapper(dts);
            }
        }

        _navViewPool = new List<NavigationViewItem>(4);
    }

    // Retrieve the element that will be displayed for a specific data item.
    // If the resolved element is not derived from NavigationViewItemBase, wrap in a NavigationViewItem before returning.
    protected override Control GetElementCore(ElementFactoryGetArgs args)
    {
        object newContent = args.Data;
        if (_settingsItem != null && _settingsItem == args.Data)
        {
            //This is the settings item, return it directly
            return args.Data as NavigationViewItem;
        }

        if (_itemTemplateWrapper != null)
        {
            newContent = _itemTemplateWrapper.GetElement(args);
        }

        // Element is already of expected type, just return it
        if (newContent is NavigationViewItemBase nvib)
        {
            return nvib;
        }

        //If no template is provided _navViewPool will never initialize
        //check here in case
        if (_navViewPool == null)
            _navViewPool = new List<NavigationViewItem>();

        // Get or create a wrapping container for the data
        NavigationViewItem nvi;
        if (_navViewPool.Count > 0)
        {
            nvi = _navViewPool[_navViewPool.Count - 1];
            _navViewPool.RemoveAt(_navViewPool.Count - 1);
        }
        else
        {
            nvi = new NavigationViewItem();
        }

        nvi.CreatedByNavigationViewItemsFactory = true;

        if (_itemTemplateWrapper != null)
        {
            if (_itemTemplateWrapper is ItemTemplateWrapper itw)
            {
                var tempArgs = new ElementFactoryRecycleArgs();
                tempArgs.Element = newContent as Control;
                _itemTemplateWrapper.RecycleElement(tempArgs);

                nvi.Content = args.Data;
                nvi.ContentTemplate = itw;
                return nvi;
            }
        }

        nvi.Content = newContent;
        return nvi;
    }

    protected override void RecycleElementCore(ElementFactoryRecycleArgs args)
    {
        if (args.Element != null)
        {
            if (args.Element is NavigationViewItem nvi)
            {
                // Check whether we wrapped the element in a NavigationViewItem ourselves.
                // If yes, we are responsible for recycling it.
                if (nvi.CreatedByNavigationViewItemsFactory)
                {
                    nvi.CreatedByNavigationViewItemsFactory = false;
                    UnlinkElementFromParent(args);
                    args.Element = null;

                    _navViewPool.Add(nvi);

                    // Retrieve the proper element that requires recycling for a user defined item template
                    // and update the args correspondingly
                    if (_itemTemplateWrapper != null)
                    {
                        // TODO: Retrieve the element and add to the args
                    }
                }
            }

            // Do not recycle SettingsItem
            bool isSettingsItem = _settingsItem != null && _settingsItem == args.Element;

            if (_itemTemplateWrapper != null && !isSettingsItem)
            {
                _itemTemplateWrapper.RecycleElement(args);
            }
            else
            {
                UnlinkElementFromParent(args);
            }
        }
    }

    private void UnlinkElementFromParent(ElementFactoryRecycleArgs args)
    {
        // We want to unlink the containers from the parent repeater
        // in case we are required to move it to a different repeater.
        if (args.Parent is Panel p)
        {
            p.Children.Remove(args.Element);
        }
    }

    private IElementFactory? _itemTemplateWrapper;
    private NavigationViewItemBase? _settingsItem;
    private List<NavigationViewItem>? _navViewPool;
}
