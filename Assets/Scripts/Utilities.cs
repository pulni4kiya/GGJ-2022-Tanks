using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static void Shuffle<T>(this IList<T> list) {
        for (int i = list.Count - 1; i > 0; i--) {
            list.Swap(i, Random.Range(0, i));
        }
    }

    public static void Swap<T>(this IList<T> list, int i, int j) {
        T value = list[i];
        list[i] = list[j];
        list[j] = value;
    }
}
