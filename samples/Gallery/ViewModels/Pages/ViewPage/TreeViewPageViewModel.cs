using System.ComponentModel;
using Avalonia.Controls;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Gallery.ViewModels;

public partial class TreeViewPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("TreeView");

    public TreeViewPageViewModel()
    {
        TreeViewItems = new Node[] 
            { 
                new Node("Technology",
                new Node[]
                {
                    new Node("Programming",
                        new Node[]
                        {
                            new Node("C#"), new Node("Python"), new Node("Rust"), new Node("Go")
                        }),
                    new Node("Frontend",
                        new Node[]
                        {
                            new Node("React"), new Node("Vue"), new Node("Avalonia"), new Node("WPF")
                        })
                }),
                new Node("Games",
                    new Node[]
                {
                    new Node("RPG",
                        new Node[]
                        {
                            new Node("Genshin Impact"), new Node("Honkai Star Rail"), new Node("Persona 5")
                        }),
                    new Node("Sandbox",
                        new Node[]
                        {
                            new Node("Minecraft"), new Node("Terraria"), new Node("Roblox")
                        })
                }),
                new Node("Music",
                    new Node[]
                {
                    new Node("Pop",
                        new Node[]
                        {
                            new Node("Taylor Swift"), new Node("Ariana Grande"), new Node("Ed Sheeran")
                        }),
                    new Node("Anime Songs",
                        new Node[]
                        {
                            new Node("YOASOBI"), new Node("Aimer"), new Node("EGOIST")
                        })
                }),
                new Node("Movies",
                    new Node[]
                {
                    new Node("Sci-Fi",
                        new Node[]
                        {
                            new Node("Interstellar"), new Node("The Matrix"), new Node("Blade Runner 2049")
                        }),
                    new Node("Animation",
                        new Node[]
                        {
                            new Node("Your Name"), new Node("Spirited Away"), new Node("Suzume")
                        })
                })
            };
    }

    [ObservableProperty]
    private Node[] _treeViewItems;

    public SelectionMode[] TreeViewSelectedModes => 
    [
        SelectionMode.Single,
        SelectionMode.Toggle,
        SelectionMode.Multiple,
        SelectionMode.AlwaysSelected
    ];

    [ObservableProperty]
    private SelectionMode _treeViewSelectedMode = SelectionMode.Multiple;
    
    public class Node
    {
        public Node[]? SubNodes { get; }
        public string Title { get; }
  
        public Node(string title)
        {
            Title = title;
        }

        public Node(string title, Node[] subNodes)
        {
            Title = title;
            SubNodes = subNodes;
        }
    }
}
