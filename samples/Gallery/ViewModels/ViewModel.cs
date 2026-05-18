using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using AvaloniaFluentUI.UI.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gallery.Models;

namespace Gallery.ViewModels;

public partial class ViewModel : ViewModelBase
{
    public string[] ItemSource =>
    [
        "Lost in the Wind", "Shining Stars", "Dream of Tomorrow", "Ocean Whisper", "Lonely Road", "Dancing Shadows",
        "Moonlight Journey", "Silent Tears", "Endless Summer", "Midnight Echo", "Wings of Freedom", "Crystal Sky",
        "Burning Heart", "Falling Snow", "Golden Horizon", "Echoes of Time", "Rising Flame", "Secret Garden",
        "Stormy Night", "Peaceful Dawn"
    ];

    [ObservableProperty]
    private Person[] _people;

    public ViewModel()
    {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += OnCarouselAutoPlay;
        _ = InitAsync();
    }

    private void OnCarouselAutoPlay(object? sender, EventArgs e)
    {
        if (CarouselCurrentIndex == CarouselAllCount - 1)
        {
            _target = -1;
        }
        if (CarouselCurrentIndex == 0)
        {
            _target = 1;
        }

        CarouselCurrentIndex += _target;
    }

    private async Task InitAsync()
    {
        await Task.Run(async () =>
        {
            People = new Person[]
            {
                new Person("Neil", "Armstrong"),
                new Person("Buzz", "Lightyear"), 
                new Person("James", "Kirk"),
                new Person("Peaceful", "Echoes"),
                new Person("Falling", "Whisper"),
                new Person("Horizon", "Dancing"),
                new Person("Silent", "Stars"),
                new Person("Night", "Heart"),
                new Person("Flame", "Summer"),
                new Person("Secret", "Shining"),
                new Person("Neil", "Armstrong"),
                new Person("Buzz", "Lightyear"),
                new Person("James", "Kirk"),
                new Person("Peaceful", "Echoes"),
                new Person("Falling", "Whisper"), 
                new Person("Horizon", "Dancing"), 
                new Person("Silent", "Stars"),
                new Person("Night", "Heart"), 
                new Person("Flame", "Summer"), 
                new Person("Secret", "Shining"),
                new Person("Neil", "Armstrong"),
                new Person("Buzz", "Lightyear"), 
                new Person("James", "Kirk"),
                new Person("Peaceful", "Echoes"),
                new Person("Falling", "Whisper"),
                new Person("Horizon", "Dancing"),
                new Person("Silent", "Stars"), 
                new Person("Night", "Heart"),
                new Person("Flame", "Summer"), 
                new Person("Secret", "Shining"),
                new Person("Neil", "Armstrong"),
                new Person("Buzz", "Lightyear"), 
                new Person("James", "Kirk"),
                new Person("Peaceful", "Echoes"),
                new Person("Falling", "Whisper"),
                new Person("Horizon", "Dancing"),
                new Person("Silent", "Stars"),
                new Person("Night", "Heart"), 
                new Person("Flame", "Summer"), 
                new Person("Secret", "Shining"),
            };
            
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
            
            CarouselData[] pages = new CarouselData[CarouselAllCount];
            for (int i = 1; i <= CarouselAllCount; i++)
            {
                pages[i - 1] = new CarouselData($"Page {i}", GetRandomHexColor());
            }

            CarouselItems = new ObservableCollection<CarouselData>(pages);
        });

            FlipViewItems = new ObservableCollection<string>()
            {
                                "avares://Gallery/Assets/Images/mc.jpg",

                                "avares://Gallery/Assets/Images/1.jpg",

                            "avares://Gallery/Assets/Images/2.jpg",
     
                            "avares://Gallery/Assets/Images/3.jpg",
          
                            "avares://Gallery/Assets/Images/4.jpg",
           
                                "avares://Gallery/Assets/Images/bg.jpg",
            };
    }

    private int _target = 1;

    [ObservableProperty]
    private bool _dataGridLinesIsVisibility = true;
    
    [ObservableProperty]
    private bool _dataGridIsReadOnly;

    [ObservableProperty]
    private bool _flipViewIsAutoPlay;

    [ObservableProperty]
    private double _flipViewAutoPlayInterval = 1500;

    public double[] Intervals => [100, 200, 300, 400, 500, 1000, 1500, 2000, 5000];

    public DataGridGridLinesVisibility[] DataGridLineVisibilityModes =>
    [
        DataGridGridLinesVisibility.None,
        DataGridGridLinesVisibility.All,
        DataGridGridLinesVisibility.Horizontal,
        DataGridGridLinesVisibility.Vertical
    ];
    
    [ObservableProperty]
    private DataGridGridLinesVisibility _dataGridLinesVisibilityMode = DataGridGridLinesVisibility.All;

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

    [ObservableProperty]
    private ObservableCollection<CarouselData> _carouselItems;

    [RelayCommand]
    private void AddCarousel()
    {
        CarouselItems.Add(
            new CarouselData(
                $"Page {CarouselItems.Count + 1}", 
                GetRandomHexColor())
            );
        CarouselAllCount++;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CarouselCurrentIndexFormat))]
    private int _carouselCurrentIndex = 0;

    public string CarouselCurrentIndexFormat => $"当前页面: " + (CarouselCurrentIndex + 1);

    public string CarouselAllCountFormat => "页面数量: " + CarouselAllCount;

    [ObservableProperty]
    private ObservableCollection<string> _flipViewItems;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CarouselAllCountFormat))]
    private int _carouselAllCount = 5;

    [ObservableProperty]
    private bool _isAutoPlay;

    private readonly DispatcherTimer _timer;

    partial void OnIsAutoPlayChanged(bool value)
    {
        if (value)
        {
            _timer.Start();
            return;
        } 
        _timer.Stop();
    }

    private IBrush GetRandomHexColor()
    {
        var random = new Random();
        int rgb = random.Next(0x1000000);
        return Brush.Parse($"#{rgb:X6}");
    }
    
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Person(string fn, string ln)
        {
            FirstName = fn;
            LastName = ln;
        }
    }
    
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
