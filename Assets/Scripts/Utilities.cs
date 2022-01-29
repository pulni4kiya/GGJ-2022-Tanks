using System;
using System.Collections.Generic;

public static class Utilities
{
    private static Random rng = new Random();

    public static void Shuffle<T>(this IList<T> list) {
        for (int i = list.Count - 1; i > 0; i--) {
            list.Swap(i, rng.Next(0, i));
        }
    }

    public static void Swap<T>(this IList<T> list, int i, int j) {
        T value = list[i];
        list[i] = list[j];
        list[j] = value;
    }
}
