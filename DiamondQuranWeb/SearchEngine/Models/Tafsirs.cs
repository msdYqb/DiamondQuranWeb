namespace DiamondQuranWeb.SearchEngine.Models
{
    public class Tafsirs
    {
        public short ID { get; set; }
        public virtual Quran QuranCleaner { get; set; }
        public string Saady { get; set; }
        public string Baghawy { get; set; }
        public string Katheer { get; set; }
        public string Qortoby { get; set; }
        public string Tabary { get; set; }
        public string Tanweer { get; set; }
        public string Waseet { get; set; }
    }
}