using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.Messaging;
using Gallery.Controls;
using Gallery.Messages;

namespace Gallery.Views;

public partial class DialogBoxAndPopupView : UserControl 
{
    public DialogBoxAndPopupView()// : base("DialogBoxAndPopup")
    {
        InitializeComponent();

        // CodeCards = new Dictionary<string, CodeCard>()
        // {
            // {"TaskDialog", TaskDialogCard},
            // {"Flyout", FlyoutCard},
            // {"ContentDialog", ContentDialogCard},
            // {"TeachingTip", TeachingTipCard}
        // };
    }
}
