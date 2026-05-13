using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Styling;
using AvaloniaFluentUI.Animations;

namespace AvaloniaFluentUI.UI.Controls;

public class FluentFlyout : Avalonia.Controls.Primitives.PopupFlyoutBase
{
    /// <summary>
        /// Defines the <see cref="Content"/> property
        /// </summary>
        public static readonly StyledProperty<object> ContentProperty =
            AvaloniaProperty.Register<Flyout, object?>(nameof(Content));

        /// <summary>
        /// Defines the <see cref="ContentTemplate"/> property.
        /// </summary>
        public static readonly StyledProperty<IDataTemplate?> ContentTemplateProperty =
            AvaloniaProperty.Register<Flyout, IDataTemplate?>(nameof(ContentTemplate));

        private Classes? _classes;

        /// <summary>
        /// Gets the Classes collection to apply to the FlyoutPresenter this Flyout is hosting
        /// </summary>
        public Classes FlyoutPresenterClasses => _classes ??= new Classes();

        /// <summary>
        /// Defines the <see cref="FlyoutPresenterTheme"/> property.
        /// </summary>
        public static readonly StyledProperty<ControlTheme?> FlyoutPresenterThemeProperty =
            AvaloniaProperty.Register<Flyout, ControlTheme?>(nameof(FlyoutPresenterTheme));
        
        /// <summary>
        /// Gets or sets the <see cref="ControlTheme"/> that is applied to the container element generated for the flyout presenter.
        /// </summary>
        public ControlTheme? FlyoutPresenterTheme
        {
            get => GetValue(FlyoutPresenterThemeProperty); 
            set => SetValue(FlyoutPresenterThemeProperty, value);
        }
        
        /// <summary>
        /// Gets or sets the content to display in this flyout
        /// </summary>
        [Content]
        public object? Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        /// <summary>
        /// Gets or sets the data template used to display the content of the flyout.
        /// </summary>
        public IDataTemplate? ContentTemplate
        {
            get => GetValue(ContentTemplateProperty);
            set => SetValue(ContentTemplateProperty, value);
        }

        protected override Control CreatePresenter()
        {
            return new FluentFlyoutPresenter
            {
                [!ContentControl.ContentProperty] = this[!ContentProperty],
                [!ContentControl.ContentTemplateProperty] = this[!ContentTemplateProperty]
            };
        }

        protected override void OnOpened()
        {
            if (Popup.Child is { } presenter)
            {
                double offset = Placement switch
                {
                    PlacementMode.Top => 32d,
                    PlacementMode.Left => 32d,
                    PlacementMode.Right => -32d,
                    PlacementMode.Bottom => -32d,
                    _ => 24d
                };
                
                var property = ((Placement == PlacementMode.Top) || (Placement == PlacementMode.Bottom)) ? TranslateTransform.YProperty : TranslateTransform.XProperty;
                _ = FluentAnimation.SlideInAsync(presenter, offset, property);
                // _ = FlyoutAnimation.RunOpenAnimationAsync(presenter, offset, property);
            }
            base.OnOpened();
        }

        protected override void OnOpening(CancelEventArgs args)
        {
            if (Popup.Child is { } presenter)
            {

                // _=RunOpenAnimationAsync(presenter);
                if (_classes != null)
                {
                    foreach (var c in FlyoutPresenterClasses)
                    {
                        presenter.Classes.Add(c);
                    }
                    // SetPresenterClasses(presenter, FlyoutPresenterClasses);
                }

                if (FlyoutPresenterTheme is { } theme)
                {
                    presenter.SetValue(Control.ThemeProperty, theme);
                }
            }
            base.OnOpening(args);
        }
}
