using DiamondQuranWeb.SearchEngine.Models;
using System.Collections.Generic;

namespace DiamondQuranWeb.SearchEngine
{
    internal class ShowTashkel
    {
        public List<Quran> ReplaceWithTashkel(ref List<Quran> searchResult)
        {
            //var quranGrammersDb = new ApplicationDbContext().AllQuranCleaner;

            //ResultPainter resultPainter = new ResultPainter();
            //var PaintPlaces = resultPainter.PaintPlacesExtractor(in searchResult);

            //foreach (var ayah in searchResult)
            //    ayah.AyahText = quranGrammersDb.First(x => x.SurahNumber == ayah.SurahNumber && x.AyahNumber == ayah.AyahNumber).AyahText;

            //resultPainter.AddPaint(ref searchResult, Color.Teal);
            //resultPainter.PaintsExtender(ref searchResult, PaintPlaces, false);
            //App.AllQuranCleaner = new QuranCleaner().Qurans.ToList();
            return searchResult;
        }
        public List<Quran> ReplaceWithNormal(ref List<Quran> searchResult)
        {
            //ResultPainter resultPainter = new ResultPainter();
            //var PaintPlaces = resultPainter.PaintPlacesExtractor(in searchResult);

            //foreach (var ayah in searchResult)
            //{
            //    //var AyahText = App.AllQuranCleaner.Single(x => x.SurahNumber == ayah.SurahNumber && x.AyahNumber == ayah.AyahNumber).AyahText;
            //    //ayah.AyahText = AyahText;
            //}

            ////resultPainter.AddPaint(ref searchResult, Color.Teal);
            //resultPainter.PaintsExtender(ref searchResult, PaintPlaces, false);
            return searchResult;
        }
    }
}
