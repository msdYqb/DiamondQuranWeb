using DiamondQuranWeb.SearchEngine.Enums;
using DiamondQuranWeb.SearchEngine.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DiamondQuranWeb.Helpers;
using DiamondQuranWeb.Models;

namespace DiamondQuranWeb.SearchEngine
{
    internal class ResultPainter
    {
        #region Properties
        public QuranTextType TextSearchType { get; private set; }
        public SearchOptions SelectedSearchOption { get; private set; }
        internal string Keyword
        {
            get
            {
                return _keyword;
            }
            set
            {
                keywordsArray = value.Split(' ');
                _keyword = value;
            }
        }
        public List<PaintPlace> PaintPositions { get; private set; }
        public bool PaintPlacesExtract { get; set; }
        #endregion

        #region Fields
        readonly HtmlGenericControl MatchSpan = new HtmlGenericControl("span");
        readonly HtmlGenericControl ResultParagraph = new HtmlGenericControl("p");
        string[] keywordsArray;
        string[] wordsarray;
        string _keyword;
        bool _paintPlacesExtract;
        short _surahNumer;
        short _ayahNumer;
        short _ayahId;
        short _positionCounter;
        string _result;
        ApplicationDbContext data = new ApplicationDbContext();
        MethodInfo ReflectionPainter
        {
            get => typeof(ResultPainter).GetMethod(SelectedSearchOption.ToString(), BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, null);
        }
        #endregion

        internal ResultPainter(QuranTextType textSearchType)
        {
            TextSearchType = textSearchType;
            MatchSpan.Attributes["class"] = "match";
            ResultParagraph.Attributes["class"] = "resultParagraph";
        }

        #region Paints Methods
        internal List<Quran> StartPainting(List<Quran> searchResult, SearchOptions searchOption)
        {
            SelectedSearchOption = searchOption;

            _paintPlacesExtract = PaintPlacesExtract;
            if (PaintPlacesExtract)
                PaintPositions = new List<PaintPlace>();

            foreach (var ayah in searchResult)
            {
                if (PaintPlacesExtract)
                {
                    _surahNumer = ayah.SurahNumber;
                    _ayahNumer = ayah.AyahNumber;
                    _ayahId = ayah.ID;
                }
                ResultParagraph.InnerHtml = ReflectionPainter.Invoke(this, new object[] { ayah.GetProperty(TextSearchType) }) as string;
                ResultParagraph.ID = $"{ayah.ID}";
                ayah.AyahLabel = new HtmlString(GenerateStringHtml(ResultParagraph));
            }
            return searchResult;
        }
        internal HtmlString StartPainting(string ayahText, SearchOptions searchOption, int ayahId)
        {
            SelectedSearchOption = searchOption;

            ResultParagraph.InnerHtml = ReflectionPainter.Invoke(this, new object[] { ayahText }) as string;
            ResultParagraph.ID = $"{ayahId}";
            return new HtmlString(GenerateStringHtml(ResultParagraph));
        }
        #endregion

        #region Essentials
        string GenerateStringHtml(HtmlGenericControl control)
        {
            StringBuilder generatedHtml = new StringBuilder();
            using (var htmlStringWriter = new StringWriter(generatedHtml))
            {
                using (var htmlTextWriter = new HtmlTextWriter(htmlStringWriter))
                {
                    control.RenderControl(htmlTextWriter);
                    return generatedHtml.ToString();
                }
            }
        }
        internal List<PaintPlace> PaintPlacesExtractor(List<string> paintedWords)
        {
            var versesSearch = new VersesSearch(TextSearchType);
            this._paintPlacesExtract = true;
            PaintPositions = new List<PaintPlace>();
            SelectedSearchOption = SearchOptions.MatchesNoncontiguous;

            foreach (var word in paintedWords)
            {
                Keyword = word;
                var searchResult = versesSearch.Search(word, SearchOptions.MatchesNoncontiguous, false);
                foreach (var ayah in searchResult)
                {
                    _surahNumer = ayah.SurahNumber;
                    _ayahNumer = ayah.AyahNumber;
                    _ayahId = ayah.ID;
                    ReflectionPainter.Invoke(this, new object[] { ayah.GetProperty(TextSearchType) });
                }
            }

            return PaintPositions;
        }
        void AddPosition(string word, short position)
        {
            PaintPositions.Add(new PaintPlace { AyahId = _ayahId, Word = word.Trim(), WordPosition = position, SurahNumber = _surahNumer, AyahNumber = _ayahNumer });
        }
        internal void PaintsExtender(ref List<Quran> searchResult, List<PaintPlace> paintPlaces, QuranTextType textResultType, bool addOldPaints)
        {
            foreach (var ayah in searchResult)
            {
                ayah.AyahText = data.Quran.Single(x => x.ID == ayah.ID).GetProperty(textResultType);
                _result = "";
                var row = searchResult.Where(s => s.SurahNumber == ayah.SurahNumber && s.AyahNumber == ayah.AyahNumber).First();
                var positions = paintPlaces.Where(s => s.SurahNumber == ayah.SurahNumber && s.AyahNumber == ayah.AyahNumber);
                int counter = 0;

                foreach (var word in ayah.AyahText.Split(' '))
                {
                    var contain = positions.Where(s => s.WordPosition == counter && s.SurahNumber == ayah.SurahNumber && s.AyahNumber == ayah.AyahNumber).FirstOrDefault();
                    if (contain != null)
                    {
                        MatchSpan.InnerHtml = word + " ";
                        _result += GenerateStringHtml(MatchSpan);
                    }
                    else _result += word + " ";
                    counter++;
                }
                ResultParagraph.InnerHtml = _result;
                row.AyahLabel = new HtmlString(GenerateStringHtml(ResultParagraph));
            }
        }
        internal List<string> GetPaintedWords(List<Quran> searchResult)
        {
            List<string> paintedWords = new List<string>();
            var htmlDoc = new HtmlDocument();
            foreach (var ayah in searchResult)
            {
                htmlDoc.LoadHtml(ayah.AyahLabel.ToHtmlString());
                var spansList = htmlDoc.DocumentNode.SelectNodes("//p/span");
                foreach (var span in spansList)
                {
                    if (paintedWords.FirstOrDefault(x => x == span.InnerHtml.Trim()) == null)
                        paintedWords.Add(span.InnerHtml.Trim());
                }
            }
            return paintedWords;
        }
        internal void PaintDuplicates(List<Quran> duplicatesList,List<QuranWords> answers, ref List<Quran> searchResult)
        {
            foreach (var ayah in duplicatesList)
            {
                _result = "";
                var ayahResult = searchResult.Where(x => x.ID == ayah.ID).First();
                foreach (var word in ayahResult.AyahText.Split(' '))
                {
                    if (answers.Where(s => s.Word == word.Trim()).FirstOrDefault() != null)
                    {
                        MatchSpan.InnerHtml = word + " ";
                        _result += GenerateStringHtml(MatchSpan);
                    }
                    else
                        _result += word + " ";
                }
                ResultParagraph.InnerHtml = _result;
                ayahResult.AyahLabel = new HtmlString(GenerateStringHtml(ResultParagraph));
            }
        }
        internal void AddPaint(ref List<Quran> searchResult)
        {
            foreach (var ayah in searchResult)
            {
                MatchSpan.InnerHtml = ayah.GetProperty(TextSearchType);
                ResultParagraph.InnerHtml = GenerateStringHtml(MatchSpan);
                ayah.AyahLabel = new HtmlString(GenerateStringHtml(ResultParagraph));
            }
        }
        internal List<Quran> RemoveFirstPaintedWords(List<Quran> searchResult, ref List<PaintPlace> paintPlaces)
        {
            foreach (var ayah in searchResult)
            {
                var verse = ayah.GetProperty(TextSearchType);
                var newVerse = "";
                var found = false;
                foreach (var (word, i) in verse.Split(' ').WithIndex())
                {
                    if (!found)
                    {
                        var painted = paintPlaces.FirstOrDefault(x => x.AyahId == ayah.ID && x.WordPosition == i);
                        if (painted == null)
                        {
                            newVerse += word + " ";
                        }

                        else
                        {
                            found = true;
                            paintPlaces.Remove(painted);
                        }
                    }
                    else newVerse += word + " ";
                }
                ayah.SetProperty(TextSearchType.ToString(), newVerse.Trim());
            }
            return searchResult;
        }
        #endregion

        #region Paints Queries
        internal string Contains(string ayahText)
        {
            _result = "";
            wordsarray = ayahText.Split(' ');
            for (int w = 0; w < wordsarray.Count(); w++)
            {
                bool contain = false;
                int containtCount = 0;
                int loopsCount = 0;
                for (int k = 0; k < keywordsArray.Count(); k++)
                {
                    if (keywordsArray.Count() <= 1)
                    {
                        if (wordsarray[w].Contains(keywordsArray[k]))
                        {
                            contain = true; break;
                        }
                    }
                    if (keywordsArray.Count() > 1)
                    {
                        if (w + 1 > wordsarray.Count()) { break; }
                        ++loopsCount;
                        if (wordsarray[w].Contains(keywordsArray[k]))
                        {
                            if (k == 0)
                            {
                                var wordCharacters = wordsarray[w].ToCharArray().ToList();
                                wordCharacters.Reverse();
                                var keywordCharacters = keywordsArray[k].ToCharArray().ToList();
                                keywordCharacters.Reverse();
                                int cCharacter = 0;
                                for (int c = 0; c < keywordCharacters.Count(); c++)
                                {
                                    if (keywordCharacters[c] == wordCharacters[c])
                                        ++cCharacter;
                                }
                                if (cCharacter == keywordCharacters.Count)
                                {
                                    ++containtCount;
                                }
                            }
                            else if (k > 0 && k + 1 < keywordsArray.Count())
                            {
                                if (wordsarray[w] == keywordsArray[k])
                                {
                                    ++containtCount;
                                }
                            }
                            else if (k + 1 == keywordsArray.Count())
                            {
                                var wordCharacters = wordsarray[w].ToCharArray().ToList();
                                var keywordCharacters = keywordsArray[k].ToCharArray().ToList();
                                int cCharacter = 0;
                                for (int c = 0; c < keywordCharacters.Count(); c++)
                                {
                                    if (keywordCharacters[c] == wordCharacters[c])
                                        ++cCharacter;
                                }
                                if (cCharacter == keywordCharacters.Count)
                                {
                                    ++containtCount;
                                }
                            }
                        }
                        if (containtCount == keywordsArray.Count())
                        {
                            contain = true; break;
                        }
                        ++w;
                    }
                }
                if (contain)
                {
                    if (keywordsArray.Count() > 1)
                    {
                        int count = containtCount;
                        for (int c = 0; c < containtCount; c++)
                        {
                            MatchSpan.InnerHtml = wordsarray[w - --count] + " ";
                            _result += GenerateStringHtml(MatchSpan);
                            if (_paintPlacesExtract)
                                AddPosition(wordsarray[w - count], (short)(w - count));
                        }
                    }
                    if (keywordsArray.Count() <= 1)
                    {
                        MatchSpan.InnerHtml = wordsarray[w] + " ";
                        _result += GenerateStringHtml(MatchSpan);
                        if (_paintPlacesExtract)
                            AddPosition(wordsarray[w], (short)w);
                    }
                }
                else
                {
                    if (keywordsArray.Count() > 1)
                    {
                        for (int i = 0; i < loopsCount; i++)
                            --w;
                    }
                    _result += wordsarray[w] + " ";
                }
            }

            //FormattedStringTrim(ref formattedString);
            //formattedString = CombineFormattedString(formattedString);
            return _result;
        }

        internal string ContainsNoncontiguous(string ayahText)
        {
            _result = "";
            _positionCounter = 0;
            wordsarray = ayahText.Split(' ');
            foreach (var word in wordsarray)
            {
                var found = false;
                foreach (var k in keywordsArray)
                {
                    if (word.Contains(k))
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    MatchSpan.InnerHtml = word + " ";
                    _result += GenerateStringHtml(MatchSpan);
                    if (_paintPlacesExtract)
                        AddPosition(word, _positionCounter);
                }
                else
                    _result += word + " ";
                _positionCounter++;
            }
            return _result;
        }
        internal string ContainsLettersUnArranged(string ayahText)
        {
            _result = "";
            _positionCounter = 0;
            wordsarray = ayahText.Split(' ');
            foreach (var word in wordsarray)
            {
                var found = false;
                foreach (var k in keywordsArray)
                {
                    if (!k.Except(word).Any())
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    MatchSpan.InnerHtml = word + " ";
                    _result += GenerateStringHtml(MatchSpan);
                    if (_paintPlacesExtract)
                        AddPosition(word, _positionCounter);
                }
                else
                    _result += word + " ";
                _positionCounter++;
            }
            return _result;
        }
        internal string ContainsLettersArranged(string ayahText)
        {
            _result = "";
            _positionCounter = 0;
            wordsarray = ayahText.Split(' ');
            foreach (var word in wordsarray)
            {
                var wordLetters = word.ToCharArray();
                bool found = false;
                foreach (var k in keywordsArray)
                {
                    int position = 0;
                    foreach (var letter in wordLetters)
                    {
                        if (position + 1 <= k.Length)
                        {
                            if (letter == k[position])
                            {
                                if (position + 1 == k.Length) { found = true; break; }
                                position++;
                            }
                        }
                    }
                }
                if (found)
                {
                    MatchSpan.InnerHtml = word + " ";
                    _result += GenerateStringHtml(MatchSpan);
                    if (_paintPlacesExtract)
                        AddPosition(word, _positionCounter);
                }
                else
                {
                    _result += word + " ";
                }
                _positionCounter++;
            }
            return _result;
        }
        internal string Matches(string ayahText) //TODO آمنوا آمنوا لا يلون
        {
            _result = "";
            _positionCounter = 0;
            wordsarray = ayahText.Split(' ');
            var values = new List<string>();
            int pos = 0;
            foreach (Match m in Regex.Matches(ayahText, "(^| )" + Keyword + "($| )"))
            {
                values.Add(ayahText.Substring(pos, m.Index - pos));
                values.Add(m.Value.Trim());
                pos = m.Index + m.Length;
            }
            values.Add(ayahText.Substring(pos));
            foreach (var a in values)
            {
                if (a == Keyword)
                {
                    MatchSpan.InnerHtml = a + " ";
                    _result += GenerateStringHtml(MatchSpan);
                    if (_paintPlacesExtract)
                    {
                        for (int i = 0; i < a.Trim().Split(' ').Count(); i++)
                            AddPosition(a, _positionCounter);
                    }
                }
                else
                {
                    _result += a + " ";
                }
                _positionCounter += (short)a.Trim().Split(' ').Count();
            }
            return _result;
        }
        internal string MatchesNoncontiguous(string ayahText)
        {
            _result = "";
            _positionCounter = 0;
            wordsarray = ayahText.Split(' ');
            foreach (var word in wordsarray)
            {
                var found = false;
                foreach (var k in keywordsArray)
                {
                    if (word == k)
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    MatchSpan.InnerHtml = word + " ";
                    _result += GenerateStringHtml(MatchSpan);
                    if (_paintPlacesExtract)
                        AddPosition(word, _positionCounter);
                }
                else
                {
                    _result += word + " ";
                }
                _positionCounter++;
            }
            return _result;
        }
        internal string FirstInAyahContains(string ayahText)
        {
            _result = "";
            _positionCounter = 0;
            wordsarray = ayahText.Split(' ');
            for (int i = 0; i < keywordsArray.Length; i++)
            {
                MatchSpan.InnerHtml = wordsarray[i] + " ";
                _result += GenerateStringHtml(MatchSpan);
                if (_paintPlacesExtract)
                    AddPosition(wordsarray[i], _positionCounter);
                _positionCounter++;
            }
            for (int i = 0; i < wordsarray.Length; i++)
            {
                if (i > keywordsArray.Length - 1)
                {
                    _result += wordsarray[i] + " ";
                    _positionCounter++;
                }
            }
            return _result;
        }
        internal string LastInAyahContains(string ayahText)
        {
            _result = "";
            _positionCounter = 0;
            wordsarray = ayahText.Split(' ');
            Array.Reverse(wordsarray);
            var matchList = new List<HtmlGenericControl>();
            var notMatchList = new List<string>();

            for (int i = 0; i < keywordsArray.Length; i++)
            {
                MatchSpan.InnerHtml = wordsarray[i] + " ";
                matchList.Add(MatchSpan);
            }
            for (int i = 0; i < wordsarray.Length; i++)
            {
                if (i > keywordsArray.Length - 1)
                {
                    notMatchList.Add(wordsarray[i] + " ");
                }
            }

            notMatchList.Reverse();
            matchList.Reverse();
            foreach (var notMatchWord in notMatchList)
            {
                _result += notMatchWord;
                _positionCounter++;
            }
            foreach (var match in matchList)
            {
                _result += GenerateStringHtml(match);
                if (_paintPlacesExtract)
                    AddPosition(match.InnerHtml.Trim(), _positionCounter);
                _positionCounter++;
            }

            return _result;
        }
        internal string FirstInSurahContains(string ayahText)
        {
            _result = "";
            _positionCounter = 0;
            wordsarray = ayahText.Split(' ');
            for (int i = 0; i < keywordsArray.Length; i++)
            {
                MatchSpan.InnerHtml = wordsarray[i] + " ";
                _result += GenerateStringHtml(MatchSpan);
                if (_paintPlacesExtract)
                    AddPosition(wordsarray[i], _positionCounter);
                _positionCounter++;
            }
            for (int i = 0; i < wordsarray.Length; i++)
            {
                if (i > keywordsArray.Length - 1)
                {
                    _result += wordsarray[i] + " ";
                    _positionCounter++;
                }
            }
            return _result;
        }
        internal string LastInSurahContains(string ayahText)
        {
            _result = "";
            _positionCounter = 0;
            wordsarray = ayahText.Split(' ');
            Array.Reverse(wordsarray);
            var matchList = new List<HtmlGenericControl>();
            var notMatchList = new List<string>();

            for (int i = 0; i < keywordsArray.Length; i++)
            {
                MatchSpan.InnerHtml = wordsarray[i] + " ";
                matchList.Add(MatchSpan);
            }
            for (int i = 0; i < wordsarray.Length; i++)
            {
                if (i > keywordsArray.Length - 1)
                {
                    notMatchList.Add(wordsarray[i] + " ");
                }
            }

            notMatchList.Reverse();
            matchList.Reverse();
            foreach (var notMatchWord in notMatchList)
            {
                _result += notMatchWord;
                _positionCounter++;
            }
            foreach (var match in matchList)
            {
                _result += GenerateStringHtml(match);
                if (_paintPlacesExtract)
                    AddPosition(match.InnerHtml.Trim(), _positionCounter);
                _positionCounter++;
            }

            return _result;
        }
        internal string FirstInWord(string ayahText)
        {
            _result = "";
            _positionCounter = 0;
            wordsarray = ayahText.Split(' ');
            foreach (var word in wordsarray)
            {
                var wordC = word.ToCharArray();
                var keywordC = keywordsArray[0].ToCharArray();
                int found = 0;
                for (int i = 0; i < keywordsArray[0].ToCharArray().Count(); i++)
                {
                    if (i <= wordC.Count() - 1)
                        if (keywordC[i] == wordC[i])
                            found++;
                }
                if (found == keywordsArray[0].ToCharArray().Count())
                {
                    MatchSpan.InnerHtml = word + " ";
                    _result += GenerateStringHtml(MatchSpan);
                    if (_paintPlacesExtract)
                        AddPosition(word, _positionCounter);
                }
                else
                    _result += word + " ";
                _positionCounter++;
            }
            return _result;
        }
        internal string LastInWord(string ayahText)
        {
            _result = "";
            _positionCounter = 0;
            wordsarray = ayahText.Split(' ');
            foreach (var word in wordsarray)
            {
                var wordC = word.ToCharArray().Reverse().ToList();
                var keywordC = keywordsArray[0].ToCharArray().Reverse().ToList();
                int found = 0;
                for (int i = 0; i < keywordsArray[0].ToCharArray().Count(); i++)
                {
                    if (i <= wordC.Count() - 1)
                        if (keywordC[i] == wordC[i])
                            found++;
                }
                if (found == keywordsArray[0].ToCharArray().Count())
                {
                    MatchSpan.InnerHtml = word + " ";
                    _result += GenerateStringHtml(MatchSpan);
                    if (_paintPlacesExtract)
                        AddPosition(word, _positionCounter);
                }
                else
                    _result += word + " ";
                _positionCounter++;
            }
            return _result;
        }
        #endregion
    }
}
