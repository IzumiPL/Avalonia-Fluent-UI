using System.Collections.Generic;
using System.Collections.ObjectModel;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Gallery.ViewModels;

public partial class FilesDropPickerPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("FileDropPicker");

    // FileDropPicker
    [ObservableProperty]
    private string? _fileDropPickerDefaultStartLocation = null;

    [ObservableProperty]
    private bool _fileDropPickerSelectedButtonIsVisible = true;

    [ObservableProperty]
    private bool _fileDropPickerAllowMultiple = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FileDropPickerSelectItemCount))]
    private ObservableCollection<string> _fileDropPickerSelectItems;

    public string FileDropPickerSelectItemCount => "Count: " + FileDropPickerSelectItems.Count;

    [RelayCommand]
    private void OnFileDropPickerSelectedComplete(IReadOnlyList<string> files)
    {
        FileDropPickerSelectItems = new ObservableCollection<string>(files);
    }

    // FolderDropPicker
    [ObservableProperty]
    private string? _folderDropPickerDefaultStartLocation = null;

    [ObservableProperty]
    private bool _folderDropPickerSelectedButtonIsVisible = true;

    [ObservableProperty]
    private bool _folderDropPickerAllowMultiple = true;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FolderDropPickerSelectItemCount))]
    private ObservableCollection<string> _folderDropPickerSelectItems;
    
    public string FolderDropPickerSelectItemCount => "Count: " + FolderDropPickerSelectItems.Count;

    [RelayCommand]
    private void OnFolderDropPickerSelectedComplete(IReadOnlyList<string> folders)
    {
        FolderDropPickerSelectItems = new ObservableCollection<string>(folders);
    }

    public FilesDropPickerPageViewModel()
    {
        FileDropPickerSelectItems = new ObservableCollection<string>();
        FolderDropPickerSelectItems = new ObservableCollection<string>();
    }
}
