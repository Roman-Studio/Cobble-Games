using System;
using System.Collections.Generic;
using UnityEngine;

namespace CobbleGames.Core
{
    public static class CGExtensionMethods
    {
        public static float RemapValue(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
        
        public static bool IsInRangeInclusive(this float value, float rangeMin, float rangeMax)
        {
            return value >= rangeMin && value <= rangeMax;
        }
        
        public static void AddSorted<T>(this List<T> list, T item) 
            where T : IComparable<T>
        {
            if (list.Count == 0)
            {
                list.Add(item);
                return;
            }

            if (list[^1].CompareTo(item) <= 0)
            {
                list.Add(item);
                return;
            }

            if (list[0].CompareTo(item) >= 0)
            {
                list.Insert(0, item);
                return;
            }

            var index = list.BinarySearch(item);
            
            if (index < 0)
                index = ~index;
            
            list.Insert(index, item);
        }
    }
}