using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class EnumerableHelper {

    public static int GetRandomIndexExcept(int count, int previous) {
        int random = Random.Range(0, count - 1);

        if (random >= previous) {
            random++;
        }

        return random;
    }

    public static List<int> GetNewRandomNumberSet(int count) {
        List<int> available = new List<int>();

        for (int i = 0; i < count; i++) {
            available.Add(i);
        }

        List<int> result = new List<int>();

        count = Mathf.Min(count, available.Count);

        for (int i = 0; i < count; i++) {
            int r = Random.Range(0, available.Count);
            result.Add(available[r]);
            available.RemoveAt(r);
        }

        return result;
    }

    public static T GetRandomElementInListWhere<T>(List<T> list, Func<T, bool> predicate) {
        int startIndex = Random.Range(0, list.Count);
        for (int i = 0; i < list.Count; i++) {
            int nextIndex = (startIndex + i) % list.Count;
            T element = list[nextIndex];
            if (predicate(element)) {
                return element;
            }
        }
        return list[startIndex];
    }
    
    
    

}
