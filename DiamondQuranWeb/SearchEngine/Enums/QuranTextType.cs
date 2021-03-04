using System.ComponentModel.DataAnnotations;

namespace DiamondQuranWeb.SearchEngine.Enums
{
    public enum QuranTextType
    {
        [Display(Name = "عادي بدون تشكيل")]
        NormalCleanest,

        [Display(Name = "عادي تشكيل قليل")]
        NormalClean,

        [Display(Name = "عادي مع تشكيل")]
        NormalEnhanced,

        [Display(Name = "عثماني بدون تشكيل")]
        UthmaniCleanest,

        [Display(Name = "عثماني تشكيل قليل")]
        UthmaniClean,

        [Display(Name = "عثماني مع التشكيل")]
        UthmaniFull,

    }
}