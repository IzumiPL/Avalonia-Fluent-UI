using System;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class DateTimeViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("DateTime");
    
    [ObservableProperty]
    private CalendarSelectionMode _calendarSelectionMode = CalendarSelectionMode.SingleDate;
    
    [ObservableProperty]
    private CalendarSelectionMode[] _calendarSelectionModes =
    {
        CalendarSelectionMode.None,
        CalendarSelectionMode.MultipleRange,
        CalendarSelectionMode.SingleDate,
        CalendarSelectionMode.SingleRange
    };

    [ObservableProperty]
    private CalendarMode[] _calenderDisplayModes =
    [
        CalendarMode.Month,
        CalendarMode.Decade,
        CalendarMode.Year
    ];

    /// <summary>
    /// 起始位置
    /// </summary>
    [ObservableProperty]
    private CalendarMode _calenderDisplayMode = CalendarMode.Month;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentSelectedDate))]
    private DateTime? _calenderSelectedDate;

    /// <summary>
    /// 日历首次显示要显示的日期
    /// </summary>
    public DateTime DisplayDate => new DateTime(2077, 3, 7);

    public string CurrentSelectedDate =>  "当前选择的日期: " + CalenderSelectedDate?.ToString("yyyy-MM-dd");

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentDatePickerSelectedDate))]
    private DateTime? _calenderDatePickerSelectedDate;

    public string CurrentDatePickerSelectedDate => "当前选择的日期: " + CalenderDatePickerSelectedDate?.ToString("yyyy-MM-dd");

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DatePickerCurrentDate))]
    private DateTimeOffset? _datePickerSelectedDate;
    
    public string DatePickerCurrentDate => "当前选择的时间: " + DatePickerSelectedDate;

    [ObservableProperty]
    private bool _datePickerIsDisplayDay = true;

    [ObservableProperty]
    private bool _datePickerIsDisplayMonth = true;

    [ObservableProperty]
    private bool _datePickerIsDisplayYear = true;

    [ObservableProperty]
    private string _timePickerCurrentClockIdentifier = "24HourClock";
    
    [ObservableProperty]
    private string[] _timePickerClockIdentifiers = ["12HourClock", "24HourClock"];
    
    [ObservableProperty]
    private int _timePickerCurrentMinuteIncrement = 1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TimePickerCurrentTime))]
    private TimeSpan? _timePickerSelectedTime;

    [ObservableProperty]
    private int[] _timePickerMinuteIncrements = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

    public string TimePickerCurrentTime => "当前选择的时间: " + TimePickerSelectedTime;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentColor))]
    private Color _colorPickerSelectedColor = Colors.DeepPink;

    public string CurrentColor => "当前选择的颜色: " + ColorPickerSelectedColor;
}
