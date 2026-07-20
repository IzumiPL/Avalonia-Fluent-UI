using System.Collections.Generic;
using System.ComponentModel;
using AvaloniaFluentUI.Locale;

namespace Gallery.ViewModels;

public partial class ListPageViewModel : ViewModelBase
{
    public override string Title => LocalizationService.Instance.GetString("List");

    public string[] ItemSource =>
    [
        "Lost in the Wind", "Shining Stars", "Dream of Tomorrow", "Ocean Whisper", "Lonely Road", "Dancing Shadows",
        "Moonlight Journey", "Silent Tears", "Endless Summer", "Midnight Echo", "Wings of Freedom", "Crystal Sky",
        "Burning Heart", "Falling Snow", "Golden Horizon", "Echoes of Time", "Rising Flame", "Secret Garden",
        "Stormy Night", "Peaceful Dawn"
    ];

    public List<TableData> TableDatas { get; }

    public ListPageViewModel()
    {
        TableDatas = new List<TableData>()
        {
            new("1", "林浩然", 24, "男", "18652341782", true),
            new("2", "陈雨桐", 19, "女", "13892745106", false),
            new("3", "王嘉豪", 31, "男", "15934182765", true),
            new("4", "刘思涵", 27, "女", "18764231590", true),
            new("5", "赵子轩", 22, "男", "13691458237", false),
            new("6", "黄诗雅", 35, "女", "15567382419", true),
            new("7", "杨俊杰", 29, "男", "18892457163", false),
            new("8", "吴欣怡", 20, "女", "13751246839", true),
            new("9", "周宇航", 41, "男", "15827491356", false),
            new("10", "徐梦琪", 26, "女", "13986521437", true),
            new("11", "孙浩宇", 18, "男", "18739164528", true),
            new("12", "马佳宁", 30, "女", "13648729315", false),
            new("13", "朱晨曦", 23, "男", "15973124856", true),
            new("14", "胡婉清", 38, "女", "18546297103", false),
            new("15", "郭文博", 28, "男", "13827516490", true),
            new("16", "何语嫣", 25, "女", "18671352489", true),
            new("17", "高子墨", 33, "男", "13792561834", false),
            new("18", "林依诺", 21, "女", "15814693725", true),
            new("19", "罗浩天", 40, "男", "13956281470", false),
            new("20", "郑欣妍", 32, "女", "18835742169", true),
            new("21", "梁梓轩", 27, "男", "15579246381", true),
            new("22", "谢可欣", 24, "女", "13624891537", false),
            new("23", "宋嘉诚", 36, "男", "18763524910", true),
            new("24", "唐若曦", 22, "女", "15982467351", false),
            new("25", "冯奕辰", 29, "男", "13851629784", true),
            new("26", "韩梦瑶", 34, "女", "18639482571", false),
            new("27", "曹宇轩", 20, "男", "13768429351", true),
            new("28", "邓静怡", 26, "女", "15891362487", true),
            new("29", "彭嘉豪", 37, "男", "13925741836", false),
            new("30", "曾雅婷", 23, "女", "18873642519", true),
            new("31", "许天宇", 42, "男", "15583497261", false),
            new("32", "吕诗涵", 28, "女", "13659284713", true),
            new("33", "魏泽宇", 31, "男", "18741369528", true),
            new("34", "蒋雨萱", 19, "女", "15928674153", false),
            new("35", "叶浩轩", 27, "男", "13847193682", true),
            new("36", "杜若琳", 35, "女", "18691527348", false),
            new("37", "苏文轩", 24, "男", "13732864915", true),
            new("38", "程诗雨", 30, "女", "15867429135", true),
            new("39", "潘宇辰", 39, "男", "13984651273", false),
            new("40", "姜依晨", 22, "女", "18832571946", true),
            new("41", "沈浩铭", 26, "男", "15591682437", false),
            new("42", "陆思雨", 33, "女", "13678542190", true),
            new("43", "顾嘉乐", 21, "男", "18752963418", true),
            new("44", "夏欣然", 29, "女", "15934827165", false),
            new("45", "钟文轩", 38, "男", "13896371524", true),
            new("46", "黎语彤", 25, "女", "18627495813", false),
            new("47", "谭俊逸", 32, "男", "13745192867", true),
            new("48", "侯梓萱", 20, "女", "15869237145", true),
            new("49", "白浩宇", 41, "男", "13971826453", false),
            new("50", "崔可馨", 27, "女", "18845629137", true),
            new("51", "石宇航", 23, "男", "15563841792", false),
            new("52", "尹诗琪", 36, "女", "13692715486", true),
            new("53", "贺嘉铭", 30, "男", "18761492835", true),
            new("54", "康欣悦", 18, "女", "15983526174", false),
            new("55", "邵泽楷", 28, "男", "13874261539", true),
            new("56", "万若彤", 34, "女", "18659134728", false),
            new("57", "段博文", 22, "男", "13728461953", true),
            new("58", "孔梦涵", 31, "女", "15891735246", true),
            new("59", "武子豪", 37, "男", "13956324871", false),
            new("60", "赖欣妤", 24, "女", "18827163954", true),
            new("61", "乔浩楠", 26, "男", "15584279361", false),
            new("62", "易语晴", 29, "女", "13645872931", true),
            new("63", "莫嘉轩", 40, "男", "18739612548", true),
            new("64", "温依婷", 21, "女", "15972468135", false),        
        };
    }

    public class TableData
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Phone  { get; set; }
        public bool IsActive { get; set; }

        public TableData(string id, string name, int age, string gender, string phone, bool isActive)
        {
            ID = id;
            Name = name;
            Age = age;
            Gender = gender;
            Phone = phone;
            IsActive = isActive;
        }
    }
}
