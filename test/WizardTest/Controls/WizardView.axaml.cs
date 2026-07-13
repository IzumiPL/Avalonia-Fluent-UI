using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using WizardTest.EventArgs;

namespace WizardTest.Controls;

[TemplatePart(Name = PART_BACK_BUTTON, Type = typeof(Button))]
[TemplatePart(Name = PART_NEXT_BUTTON, Type = typeof(Button))]
public class WizardView : Carousel
{
    public static readonly StyledProperty<bool> BackButtonIsVisibleProperty =
        AvaloniaProperty.Register<WizardView, bool>(nameof(BackButtonIsVisible), true);

    public static readonly StyledProperty<bool> NextButtonIsVisibleProperty =
        AvaloniaProperty.Register<WizardView, bool>(nameof(NextButtonIsVisible), true);

    public bool NextButtonIsVisible
    {
        get => GetValue(NextButtonIsVisibleProperty);
        set => SetValue(NextButtonIsVisibleProperty, value);
    }

    public bool BackButtonIsVisible
    {
        get => GetValue(BackButtonIsVisibleProperty);
        set => SetValue(BackButtonIsVisibleProperty, value);
    }
    
    private Button? _backButton;
    private Button? _nextButton;
    
    public event EventHandler<WizardNextingEventArgs>? Nexting;
    public event EventHandler? Nexted;

    public event EventHandler<WizardFinishingEventArgs>? Finishing;
    public event EventHandler? Finished;
    
    private const string PART_BACK_BUTTON = "PART_BackButton";
    private const string PART_NEXT_BUTTON = "PART_NextButton";

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _backButton?.Click -= OnBackButtonClick;
        _nextButton?.Click -= OnNextButtonClick;
        
        base.OnApplyTemplate(e);
        
        _backButton = e.NameScope.Find<Button>(PART_BACK_BUTTON);
        _nextButton = e.NameScope.Find<Button>(PART_NEXT_BUTTON);
        
        _backButton?.Click += OnBackButtonClick;
        _nextButton?.Click += OnNextButtonClick;
    }

    private void OnNextButtonClick(object? sender, RoutedEventArgs e)
    {
        Forward();
    }

    private void OnBackButtonClick(object? sender, RoutedEventArgs e)
    {
        Back();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SelectedIndexProperty)
        {
            BackButtonIsVisible = change.GetNewValue<int>() != 0;
            UpdateButtonText();
        }
    }

    private void Back()
    {
        if (SelectedIndex <= 0) { return; }

        SelectedIndex--;
    }

    private void Forward()
    {
        if (SelectedIndex >= ItemCount - 1)
        {
            OnFinished(new WizardFinishingEventArgs());
            return;
        }

        var targetIndex = SelectedIndex + 1;

        if (!OnNexting(new WizardNextingEventArgs(SelectedIndex, targetIndex)))
            return;

        SelectedIndex = targetIndex;
        OnNexted(System.EventArgs.Empty);
    }

    private void UpdateButtonText()
    {
        _nextButton?.Content = SelectedIndex == ItemCount - 1 ? "完成" : "下一步";
    }

    protected virtual bool OnNexting(WizardNextingEventArgs e)
    {
        Nexting?.Invoke(this, e);

        return !e.Cancel;
    }
    
    protected virtual void OnNexted(System.EventArgs e)
    {
        Nexted?.Invoke(this, e);
    }

    protected virtual void OnFinished(WizardFinishingEventArgs e)
    {
        Finishing?.Invoke(this, e);

        if (e.Cancel)
        {
            return;
        }
        
        Finished?.Invoke(this, System.EventArgs.Empty);
    }
}

