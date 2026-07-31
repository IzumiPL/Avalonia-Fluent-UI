namespace Gallery.Messages.MainWindowMessages;

public record EnabledBackgroundImageMessage(bool IsVisible, string? Path = null);
