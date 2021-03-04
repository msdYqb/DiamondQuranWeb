using System;
using System.Collections.Generic;

namespace DiamondQuranWeb.SearchEngine
{
    public class Helpers
    {
        public static List<int> AllIndexesOf(string str, string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += keyword.Length)
            {
                index = str.IndexOf(keyword, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }
    }
}
