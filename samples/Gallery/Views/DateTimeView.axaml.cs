using System;
using System.Collections.Generic;
using Gallery.Controls;

namespace Gallery.Views;

public partial class DateTimeView : ViewBase
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Views/DateTimeView.axaml");

    public DateTimeView() : base("DateTime")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>()
        {
            { "CalendarDatePicker", CalendarDatePickerCard },
            { "DatePicker", DatePickerCard },
            { "TimePicker", TimePickerCard }
        };
    }
}
