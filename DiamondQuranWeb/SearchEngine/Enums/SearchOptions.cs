using System.ComponentModel.DataAnnotations;

namespace DiamondQuranWeb.SearchEngine.Enums
{
    public enum SearchOptions
    {
        [Display(Name = "Contains", ResourceType = typeof(Resources.Resource))]
        Contains,

        [Display(Name = "ContainsNoncontiguous", ResourceType = typeof(Resources.Resource))]
        ContainsNoncontiguous,

        [Display(Name = "ContainsLettersUnArranged", ResourceType = typeof(Resources.Resource))]
        ContainsLettersUnArranged,

        [Display(Name = "ContainsLettersArranged", ResourceType = typeof(Resources.Resource))]
        ContainsLettersArranged,

        [Display(Name = "Matches", ResourceType = typeof(Resources.Resource))]
        Matches,

        [Display(Name = "MatchesNoncontiguous", ResourceType = typeof(Resources.Resource))]
        MatchesNoncontiguous,

        [Display(Name = "FirstInAyahContains", ResourceType = typeof(Resources.Resource))]
        FirstInAyahContains,

        [Display(Name = "LastInAyahContains", ResourceType = typeof(Resources.Resource))]
        LastInAyahContains,

        [Display(Name = "FirstInSurahContains", ResourceType = typeof(Resources.Resource))]
        FirstInSurahContains,

        [Display(Name = "LastInSurahContains", ResourceType = typeof(Resources.Resource))]
        LastInSurahContains,

        [Display(Name = "FirstInWord", ResourceType = typeof(Resources.Resource))]
        FirstInWord,

        [Display(Name = "LastInWord", ResourceType = typeof(Resources.Resource))]
        LastInWord
    }
}
