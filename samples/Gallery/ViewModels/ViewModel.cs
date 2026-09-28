using System.Collections.Generic;
using AvaloniaFluentUI.Locale;
using Gallery.Models;

namespace Gallery.ViewModels;

public partial class ViewModel : ViewModelBase
{
    public List<ButtonItemModel> ViewItemSource { get; }
    public override string Title => LocalizationService.Instance.GetString("View");
    
     public ViewModel()
    {
        ViewItemSource = ButtonItemModel.CreateList(
            ("FlipView", "FlipView", "CarouselView", "Carousel view, a control suitable for displaying multiple pictures"),
            ("ListBox", "ListBox", "List", "List box, can display multiple items"),
            ("TreeView", "TreeView", "TreeView", "A tree view"),
            ("PageTransition", "PageTransition", "CarouselView", "Page switching control with animation"),
            ("PersonPicture", "Avatar", "AvatarView", "Displays the picture of a person/contact.")
        );
    }

}
