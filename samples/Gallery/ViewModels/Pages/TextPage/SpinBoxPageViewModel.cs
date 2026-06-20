using System.ComponentModel;
using Avalonia.Controls;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class SpinBoxPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("NumberBox");
    
    [ObservableProperty]
    private NumberBoxSpinButtonPlacementMode _numberBoxSpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Inline;

    public NumberBoxSpinButtonPlacementMode[] NumberBoxSpinButtonPlacementModes => 
    [
        NumberBoxSpinButtonPlacementMode.Hidden,
        NumberBoxSpinButtonPlacementMode.Inline,
        NumberBoxSpinButtonPlacementMode.Compact
    ];

    [ObservableProperty]
    private Location _spinnerLocation = Location.Right;

    public Location[] SpinnerLocations => [Location.Left, Location.Right];

    [ObservableProperty]
    private bool _isEnabledSpinner = true;
}
