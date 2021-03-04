using DiamondQuranWeb.SearchEngine.Enums;
using System.Collections.Generic;

namespace DiamondQuranWeb.SearchEngine.Models
{
    //public class LogicsSearchItems
    //{
    //    public ObservableCollection<SearchOptionsRadiosButtons> GetSearchOptionsList { get => Services.Lists.GetSearchOptionsList; }
    //    string _classId;
    //    public string ClassId
    //    {
    //        get => _classId;
    //        set => SetProperty(ref _classId, value);
    //    }
    //    int? _selectedIndex;
    //    public int? SelectedIndex
    //    {
    //        get => _selectedIndex;
    //        set => SetProperty(ref _selectedIndex, value);
    //    }
    //}
    public class LogicsSearchSelectedOption//LogicsSearchSettings
    {
        public string Keyword { get; set; }
        public SearchOptions SearchOption { get; set; }
    }
    public class LogicsSearchSetting
    {
        public LogicsSearchChoice LogicSearchChoice { get; set; }
        public List<LogicsSearchSelectedOption> SelectedOptions1 { get; set; } = new List<LogicsSearchSelectedOption>();
        public IEnumerable<LogicsSearchSelectedOption> SelectedOptions2 { get; set; }
        public bool Within { get; set; }
    }
    public class SearchSetting
    {
        public string Keyword { get; set; }
        public SearchOptions SelectedSearchOption { get; set; }
        public QuranTextType QuranTextResultType { get; set; }
        public bool PaintPlacesExtract { get; set; } = true;
    }
    public class WordsSearchSetting
    {
        public string Keyword { get; set; }
        public SearchOptions SelectedSearchOption { get; set; }
        public bool WordsListSearch { get; set; }
        public List<QuranWords> SelectedWords { get; set; }
    }
}
