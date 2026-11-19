using System.Collections.Generic;
using UnityEngine;

namespace Libraries.DNV.Extensions
{
    public static class ListExtensions
    {
        public static T GetRandom<T>(this List<T> aSource)
        {
            return (aSource.Count > 0)
                ? aSource[Random.Range(0, aSource.Count)]
                : default(T);
        }

        public static T PopRandom<T>(this List<T> aSource)
        {
            T result = default(T);
            if (aSource.Count > 0)
            {
                int i = Random.Range(0, aSource.Count);
                result = aSource[i];
                aSource.RemoveAt(i);
            }

            return result;
        }

        public static T PeekFirst<T>(this List<T> aSource)
        {
            return (aSource.Count > 0) ? aSource[0] : default(T);
        }

        public static T PeekLast<T>(this List<T> aSource)
        {
            return (aSource.Count > 0) ? aSource[aSource.Count - 1] : default(T);
        }

        public static T PopFirst<T>(this List<T> aSource)
        {
            T result = default(T);
            if (aSource.Count > 0)
            {
                result = aSource[0];
                aSource.RemoveAt(0);
            }

            return result;
        }

        public static T PopLast<T>(this List<T> aSource)
        {
            T result = default(T);
            if (aSource.Count > 0)
            {
                int i = aSource.Count - 1;
                result = aSource[i];
                aSource.RemoveAt(i);
            }

            return result;
        }

        public static T Pop<T>(this IList<T> aSource, int aIndex)
        {
            T result = default(T);
            if (aIndex >= 0 && aIndex < aSource.Count)
            {
                result = aSource[aIndex];
                aSource.RemoveAt(aIndex);
            }

            return result;
        }

        public static List<T> Clone<T>(this List<T> aSource)
        {
            return new List<T>(aSource);
        }

        public static void Shuffle<T>(this List<T> aList)
        {
            T temp;
            int newPos;
            int index = aList.Count;
            while (index > 1)
            {
                index--;
                newPos = Random.Range(0, index);
                temp = aList[newPos];
                aList[newPos] = aList[index];
                aList[index] = temp;
            }
        }

        public static void Swap<T>(this List<T> aList, int aIndexA, int aIndexB)
        {
            if (aIndexA < 0 || aIndexB < 0 ||
                aIndexA == aIndexB ||
                aIndexA >= aList.Count || aIndexB >= aList.Count)
            {
                return;
            }

            T temp = aList[aIndexA];
            aList[aIndexA] = aList[aIndexB];
            aList[aIndexB] = temp;
        }


        public static bool TryAdd<T>(this List<T> list, T item)
        {
            if(list.Contains(item))
            {
                return false;
            }

            list.Add(item);
            return true;
        }
    }
}
