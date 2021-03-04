using System.ComponentModel.DataAnnotations;

namespace DiamondQuranWeb.SearchEngine.Enums
{
    public enum TafsirsNames
    {
        [Display(Name = "تفسير السعدي")]
        Saady,

        [Display(Name = "تفسير الوسيط")]
        Waseet,

        [Display(Name = "تفسير البغوي")]
        Baghawy,

        [Display(Name = "تفسير ابن كثير")]
        Katheer,

        [Display(Name = "تفسير القرطبي")]
        Qortoby,

        [Display(Name = "تفسير الطبري")]
        Tabary,

        [Display(Name = "تفسير التحرير والتنوير")]
        Tanweer
    }
}