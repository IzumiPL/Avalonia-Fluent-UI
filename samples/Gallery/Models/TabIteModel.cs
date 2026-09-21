namespace Gallery.Models;

public class TabIteModel
{
    public string Title { get; set; } = string.Empty;
    public bool IsClosable { get; set; } = true;
    public object? Tag { get; set; }
    public object? Content { get; set; }
    public object? Icon { get; set; }

    public TabIteModel() { }

    public TabIteModel(string title, bool isClosable, object? tag, object? content, object? icon)
    {
        Title = title;
        IsClosable = isClosable;
        Tag = tag;
        Content = content;
        Icon = icon;
    }
}
