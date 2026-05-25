using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace AvaloniaFluentUI.Controls;

[TemplatePart(Name = PART_EDIT_TEXT_BOX, Type = typeof(TextBox))]
[TemplatePart(Name = PART_CONTENT_PRESENTER, Type = typeof(ContentPresenter))]
public class RoundListBoxItem : ListBoxItem
{
    private TextBox _editTextBox;
    private ContentPresenter _presenter;
    
    private const string PART_EDIT_TEXT_BOX = "PART_EditTextBox";
    private const string PART_CONTENT_PRESENTER = "PART_ContentPresenter";

    public RoundListBoxItem()
    {
        DoubleTapped += OnDoubleTapped;
    }

    private void DisplayEditTextBox()
    {
        if (_editTextBox != null && Content is String)
        {
            _presenter?.IsVisible = false;
            _editTextBox.Text = Content.ToString();
            _editTextBox.IsVisible = true;
            _editTextBox.Focus();
            _editTextBox.SelectAll();
        }
    }

    private void OnDoubleTapped(object sender, TappedEventArgs e)
    {
        DisplayEditTextBox();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsSelectedProperty)
        {
            if (Content is String && _editTextBox.IsVisible)
            {
                _editTextBox.IsVisible = false;
                Content = _editTextBox.Text;
                _presenter?.IsVisible = true;
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_editTextBox != null)
        {
            _editTextBox.KeyDown -= OnTextBoxEnterKeyDown;
            // _editTextBox.LostFocus -= OnTextBoxLostFocus;
        }
        
        _editTextBox = e.NameScope.Find<TextBox>(PART_EDIT_TEXT_BOX);
        _presenter = e.NameScope.Find<ContentPresenter>(PART_CONTENT_PRESENTER);

        if (_editTextBox != null)
        {
            _editTextBox.KeyDown += OnTextBoxEnterKeyDown;
            // _editTextBox.LostFocus += OnTextBoxLostFocus;
        }
    }

    // private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
    // {
    //     Console.WriteLine($"Lost Focus, Content: {Content?.GetType()}");
    //     
    //     if (Content is String)
    //     {
    //         _editTextBox.IsVisible = false;
    //         Content = _editTextBox.Text;
    //         _presenter?.IsVisible = true;
        // }
    // }

    private void OnTextBoxEnterKeyDown(object sender, KeyEventArgs e)
    {
        if (Content is String)
        {
            if (e.Key == Key.Enter)
            {
                _editTextBox.IsVisible = false;
                Content = _editTextBox.Text;
                _presenter?.IsVisible = true;
            }
        }
    }
}
