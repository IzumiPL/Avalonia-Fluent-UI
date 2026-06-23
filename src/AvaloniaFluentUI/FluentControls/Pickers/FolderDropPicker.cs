using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace AvaloniaFluentUI.Controls;

public class FolderDropPicker : TemplatedControl
{
   public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<FolderDropPicker, ICommand?>(nameof(Command));
    
    public static readonly StyledProperty<bool> AllowMultipleProperty =
        AvaloniaProperty.Register<FolderDropPicker, bool>(nameof(AllowMultiple), defaultValue: true);

    public static readonly StyledProperty<string> SelectionTitleProperty =
        AvaloniaProperty.Register<FolderDropPicker, string>(nameof(SelectionTitle));
    
    public static readonly StyledProperty<string?> SuggestedStartLocationProperty =
        AvaloniaProperty.Register<FolderDropPicker, string?>(nameof(SuggestedStartLocation), defaultValue: null);

    public static readonly StyledProperty<bool> SelectedButtonIsVisibleProperty =
        AvaloniaProperty.Register<FolderDropPicker, bool>(nameof(SelectedButtonIsVisible));

    public static readonly StyledProperty<string> HeaderProperty =
        AvaloniaProperty.Register<FolderDropPicker, string>(nameof(Header));

    public static readonly StyledProperty<string> OrTextProperty =
        AvaloniaProperty.Register<FolderDropPicker, string>(nameof(OrText));

    public string OrText
    {
        get => GetValue(OrTextProperty);
        set => SetValue(OrTextProperty, value);
    }
    
    public string Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public bool SelectedButtonIsVisible
    {
        get => GetValue(SelectedButtonIsVisibleProperty);
        set => SetValue(SelectedButtonIsVisibleProperty, value);
    }

    public string? SuggestedStartLocation
    {
        get => GetValue(SuggestedStartLocationProperty);
        set => SetValue(SuggestedStartLocationProperty, value);
    }

    public string SelectionTitle
    {
        get => GetValue(SelectionTitleProperty);
        set => SetValue(SelectionTitleProperty, value);
    }
    
    public bool AllowMultiple
    {
        get => GetValue(AllowMultipleProperty);
        set => SetValue(AllowMultipleProperty, value);
    }

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public event EventHandler<DroppedEventArgs>? Dropped;

    private Button? _selectedButton;
    
    private const string PART_SELECTED_BUTTON = "PART_SelectedButton";

    public FolderDropPicker()
    {
        DragDrop.SetAllowDrop(this, true);
        DragDrop.AddDropHandler(this, OnDrop);
        DragDrop.AddDragOverHandler(this, OnDragOver);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _selectedButton?.Click -= OnSelectedClicked;
       
        _selectedButton = e.NameScope.Find<Button>(PART_SELECTED_BUTTON);
        
        _selectedButton?.Click += OnSelectedClicked;
    }

    private async void OnSelectedClicked(object? sender, RoutedEventArgs e)
    {
        var result = await OpenSelectionDialog();
        if (result != null)
        {
            var args = new DroppedEventArgs(result);
            OnDropped(args);
            Command?.Execute(args.Values);
        }
    }
    
    internal void OnDropped(DroppedEventArgs e)
    {
        Dropped?.Invoke(this, e);
    }

    public void Open()
    {
        OnSelectedClicked(null, null);
    }

    protected virtual async Task<IReadOnlyList<string>> OpenSelectionDialog()
    {
        var toplevel = TopLevel.GetTopLevel(this);
        if (toplevel != null)
        {
           var options = new FolderPickerOpenOptions
            {
                Title = this.SelectionTitle,
                AllowMultiple = this.AllowMultiple
            };
            if (SuggestedStartLocation != null)
            {
                options.SuggestedStartLocation = await toplevel.StorageProvider.TryGetFolderFromPathAsync(SuggestedStartLocation);
            }
            
            var values = await toplevel.StorageProvider.OpenFolderPickerAsync(options);

            if (values.Count > 0)
            {
                var folders = new List<string>();

                foreach (var item in values)
                {
                    var path = item.TryGetLocalPath();
                    if (path is null) continue;
                    
                    folders.Add(path);
                } 
                return folders;
            }
        }

        return null;
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.DataTransfer.Contains(DataFormat.File)
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    protected virtual void OnDrop(object? sender, DragEventArgs e)
    {
        var values = e.DataTransfer.TryGetFiles();
        if (values == null) return;
        
        var folders = new List<string>();
        foreach (var item in values)
        {
            if (item is not IStorageFolder) continue;
            
            var path = item.TryGetLocalPath();
            if (path == null) continue;
            
            folders.Add(path);
        }
        
        if (folders.Count == 0) return;
        
        var fe = new DroppedEventArgs(folders);
        OnDropped(fe);
        Command?.Execute(fe.Values);    
    } 
}
