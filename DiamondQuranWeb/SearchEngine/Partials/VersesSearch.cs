using DiamondQuranWeb.SearchEngine.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace DiamondQuranWeb.SearchEngine
{
    internal partial class VersesSearch
    {
        public IQueryable Database { get; set; }
        internal List<Quran> ConvertToList(IQueryable qurans) => JsonConvert.DeserializeObject<List<Quran>>(JsonConvert.SerializeObject(qurans));
        internal List<Quran> ConvertToList(List<object> qurans) => JsonConvert.DeserializeObject<List<Quran>>(JsonConvert.SerializeObject(qurans));
        internal List<Quran> ConvertToList(List<Quran> qurans) => JsonConvert.DeserializeObject<List<Quran>>(JsonConvert.SerializeObject(qurans));

    }
}