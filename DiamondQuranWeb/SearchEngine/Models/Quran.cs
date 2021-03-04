using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace DiamondQuranWeb.SearchEngine.Models
{
    public class Quran
    {
        public short ID { get; set; }
        public string SurahName { get; set; }
        public short SurahNumber { get; set; }
        public short AyahNumber { get; set; }
        public short Page { get; set; }
        public string NormalCleanest { get; set; }
        public string NormalClean { get; set; }
        public string NormalEnhanced { get; set; }
        public string UthmaniFull { get; set; }
        public string UthmaniClean { get; set; }
        public string UthmaniCleanest { get; set; }

        [NotMapped]
        public string AyahText { get; set; }
        [NotMapped]
        public bool InFavorite { get; set; }
        [NotMapped]
        public bool HasComment { get; set; }
        [NotMapped]
        public HtmlString AyahLabel { get; set; }
    }
    public class QuranWords
    {
        public int ID { get; set; }
        public string SurahName { get; set; }
        public int SurahNumber { get; set; }
        public int AyahNumber { get; set; }
        public string Word { get; set; }
        public string WordWithGrammers { get; set; }
        public List<WordsWithGrammers> WordsWithGrammersList { get; set; } = new List<WordsWithGrammers>();
    }
    public class WordsWithGrammers
    {
        public int ID { get; set; }
        public string SurahName { get; set; }
        public int SurahNumber { get; set; }
        public int AyahNumber { get; set; }
        public string Word { get; set; }
    }
}