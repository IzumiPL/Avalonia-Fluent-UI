using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Gallery.Pages;
using Gallery.ViewModels;
using Gallery.Views;

namespace Gallery;

public class ViewLocator : IDataTemplate
{
    private readonly Dictionary<Type, Func<Control>> _factory = new();

    public ViewLocator()
    {
        Register();
    }

    private void Register()
    {
        _factory[typeof(HomeViewModel)] = () => new HomeView();
        _factory[typeof(IconsViewModel)] = () => new IconsView();
        
        _factory[typeof(BasicInputViewModel)] = () => new BasicInputView();
        _factory[typeof(ButtonPageViewModel)] = () => new ButtonPage();
        _factory[typeof(ComboBoxPageViewModel)] = () => new ComboBoxPage();
        _factory[typeof(SlierPageViewModel)] = () => new SliderPage();
        
        _factory[typeof(DialogBoxAndPopupViewModel)] = () => new DialogBoxAndPopupView();
        _factory[typeof(DialogPageViewModel)] = () => new DialogPage();
        _factory[typeof(FlyoutPageViewModel)] = () => new FlyoutPage();
        _factory[typeof(ShortcutKeyPickerPageViewModel)] = () => new ShortcutKeyPickerPage();
        
        _factory[typeof(LayoutViewModel)] = () => new LayoutView();
        _factory[typeof(BorderPageViewModel)] = () => new BorderPage();
        _factory[typeof(PanelPageViewModel)] = () => new PanelPage();
        
        _factory[typeof(NavigationViewModel)] = () => new NavigationView();
        _factory[typeof(NavigationViewPageViewModel)] = () => new NavigationViewPage();
        _factory[typeof(TabsPageViewModel)] = () => new TabsPage();
        _factory[typeof(SegmentedViewPageViewModel)] = () => new SegmentedViewPage();
        _factory[typeof(FrameViewPageViewModel)] = () => new FrameViewPage();
        _factory[typeof(BreadcrumbBarPageViewModel)] = () => new BreadcrumbBarPage();
        
        _factory[typeof(TextViewModel)] = () => new TextView();
        _factory[typeof(TextBlockPageViewModel)] = () => new TextBlockPage();
        _factory[typeof(TextBoxPageViewModel)] = () => new TextBoxPage();
        _factory[typeof(SpinBoxPageViewModel)] = () => new SpinBoxPage();
        
        _factory[typeof(ViewModel)] = () => new View();
        _factory[typeof(ListPageViewModel)] = () => new ListPage();
        _factory[typeof(TreeViewPageViewModel)] = () => new TreeViewPage();
        _factory[typeof(CarouselViewPageViewModel)] = () => new CarouselViewPage();
        _factory[typeof(CardPageViewModel)] = () => new CardPage();
        _factory[typeof(AvatarViewPageViewModel)] = () => new AvatarViewPage();
        _factory[typeof(FilesDropPickerPageViewModel)] = () => new FilesDropPickerPage();
        
        _factory[typeof(ScrollViewModel)] = () => new ScrollView();
        
        _factory[typeof(StatusAndInformationViewModel)] = () => new StatusAndInformationView();
        
        _factory[typeof(MenuAndToolBarViewModel)] = () => new MenuAndToolBarView();
        _factory[typeof(MenuPageViewModel)] = () => new MenuPage();
        _factory[typeof(ContextMenuViewModel)] = () => new ContextMenuPage();
        _factory[typeof(CommandBarViewPageViewModel)] = () => new CommandBarViewPage();
        
        _factory[typeof(DateTimeViewModel)] = () => new DateTimeView();
        
        _factory[typeof(SettingsViewModel)] = () => new SettingsView();
    }

    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var vmType = param.GetType();

        return _factory.TryGetValue(vmType, out var creator)
            ? creator()
            : new TextBlock
            {
                Text = $"View not registered: {vmType.Name}"
            };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
