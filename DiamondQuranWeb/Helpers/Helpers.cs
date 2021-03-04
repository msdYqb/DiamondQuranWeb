using System;

namespace DiamondQuranWeb.Helpers
{
    public class Helpers
    {
        public static string CleanKeyword(string keyword)
        {
            var cleanKeyword = "";
            foreach (var word in keyword.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                cleanKeyword += word + " ";
            }
            cleanKeyword = cleanKeyword.Replace('أ', 'ا').Replace('آ', 'ا').Replace('إ', 'ا');
            return cleanKeyword.Trim();
        }
    }
}