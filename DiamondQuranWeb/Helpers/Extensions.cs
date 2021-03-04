using DiamondQuranWeb.SearchEngine.Models;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace DiamondQuranWeb.Helpers
{
    public static class ListExtensions
    {
        public static List<T> RepeatedDefault<T>(int count)
        {
            return Repeated(default(T), count);
        }

        public static List<T> Repeated<T>(T value, int count)
        {
            List<T> ret = new List<T>(count);
            ret.AddRange(Enumerable.Repeat(value, count));
            return ret;
        }

        public static List<Quran> ConvertToModel(this List<dynamic> entities)
        {
            return entities.ConvertAll(e => (Quran)e);
        }
    }
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .GetName();
        }
        public static string GetShortName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .GetShortName();
        }
    }
    public static class ObjectExtensions
    {
        public static object GetPropValue(this object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        public static void SetProperty(this object src, string propName,string propValue)
        {
             src.GetType().GetProperty(propName).SetValue(src, propValue,null);
        }

        public static string GetProperty(this object target, Enum name)
        {
            var nameStr = name.ToString();
            return Microsoft.VisualBasic.CompilerServices.Versioned.CallByName(target, nameStr, CallType.Get) as string;
        }
        public static object GetProperty(this object target, string name)
        {
            return Microsoft.VisualBasic.CompilerServices.Versioned.CallByName(target, name, CallType.Get);
        }
        public static object Property(this object obj, string propertyName)
        {
            Type t = obj.GetType();
            PropertyInfo property = t.GetProperty(propertyName);
            return property.GetValue(obj, null);
        }

    }
    public static class Extensions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self) => self.Select((item, index) => (item, index));
    }
}