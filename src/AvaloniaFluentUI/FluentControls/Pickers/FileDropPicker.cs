using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;

namespace AvaloniaFluentUI.Controls;

public class FileDropPicker : FolderDropPicker
{
   public static readonly StyledProperty<IReadOnlyList<FilePickerFileType>> FileTypeFilterProperty =
        AvaloniaProperty.Register<FileDropPicker, IReadOnlyList<FilePickerFileType>>(nameof(FileTypeFilter),
            defaultValue: [new FilePickerFileType("所有文件") { Patterns = ["*.*" ] }]);

    public IReadOnlyList<FilePickerFileType> FileTypeFilter
    {
        get => GetValue(FileTypeFilterProperty);
        set => SetValue(FileTypeFilterProperty, value);
    }

    protected override async Task<IReadOnlyList<string>> OpenSelectionDialog()
    {
        var toplevel =  TopLevel.GetTopLevel(this);
        if (toplevel != null)
        {
            var options = new FilePickerOpenOptions
            {
                Title = this.SelectionTitle,
                AllowMultiple = this.AllowMultiple,
                FileTypeFilter = this.FileTypeFilter,
            };
            if (SuggestedStartLocation != null)
            {
                options.SuggestedStartLocation = await toplevel.StorageProvider.TryGetFolderFromPathAsync(SuggestedStartLocation);
            }
            
            var values = await toplevel.StorageProvider.OpenFilePickerAsync(options);

            if (values.Count > 0)
            {
                var files = new List<string>();

                foreach (var item in values)
                {
                    var path = item.TryGetLocalPath();
                    if (path is null) continue;
                    
                    files.Add(path);
                } 
                return files;
            }
        }

        return null;
    }

    protected override void OnDrop(object? sender, DragEventArgs e)
    {
        var values = e.DataTransfer.TryGetFiles();
        if (values == null) return;

        var files = new List<string>();
        foreach (var item in values)
        {
            if (item is not IStorageFile) continue;
            
            var path = item.TryGetLocalPath();
            if (path == null) continue;
            
            files.Add(path);
        }
        
        if (files.Count == 0) return;
        
        var fe = new DroppedEventArgs(files);
        OnDropped(fe);
        Command?.Execute(fe.Values);  
    } 
}
