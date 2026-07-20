using System.Collections.ObjectModel;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Gallery.ViewModels;

public partial class ComboBoxPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("ComboBox");
    
    public string[] Items => ["小鸟游六花", "亚托莉", "上坂茅羽耶", "和泉妃爱", "常盘华乃", "结城明日奈", "御坂美琴", "佐天泪子", "后藤一里", "山田凉", "伊地知虹夏", "喜多郁代", "锦亚澄"];
    
    [ObservableProperty]
    private ObservableCollection<string> _multiSelectionSelectedItems = new ObservableCollection<string>();

    [RelayCommand]
    private void ClearMultiSelectionSelectedItem() => MultiSelectionSelectedItems.Clear();
}
