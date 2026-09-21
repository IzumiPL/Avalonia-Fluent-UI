using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Gallery.ViewModels;

public partial class RepeatButtonPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("RepeatButton");

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RepeatCountText))]
    private int _repeatCount;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SlowRepeatCountText))]
    private int _slowRepeatCount;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TransparentRepeatCountText))]
    private int _transparentRepeatCount;

    [ObservableProperty]
    private bool _repeatButtonIsDisabled;

    [ObservableProperty]
    private bool _slowRepeatButtonIsDisabled;

    [ObservableProperty]
    private bool _transparentRepeatButtonIsDisabled;

    public string RepeatCountText => $"Count: {RepeatCount}";

    public string SlowRepeatCountText => $"Count: {SlowRepeatCount}";

    public string TransparentRepeatCountText => $"Count: {TransparentRepeatCount}";

    [RelayCommand]
    private void IncreaseRepeatCount() => RepeatCount++;

    [RelayCommand]
    private void IncreaseSlowRepeatCount() => SlowRepeatCount++;

    [RelayCommand]
    private void IncreaseTransparentRepeatCount() => TransparentRepeatCount++;
}
