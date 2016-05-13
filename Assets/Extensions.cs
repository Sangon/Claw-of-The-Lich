using UnityEngine;
using System;

public static class Vector2Extension
{
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = v.x;
        float ty = v.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }
}

public static class CombineExtension
{
    public static void AppendSecondArrayToFirst<T>(ref T[] first, T[] second)
    {
        int arrayOriginalSize = first.Length;
        Array.Resize<T>(ref first, first.Length + second.Length);
        Array.Copy(second, 0, first, arrayOriginalSize, second.Length);
    }
    public static T[] CreateCombinedArrayFrom<T>(T[] first, T[] second)
    {
        T[] result = new T[first.Length + second.Length];
        Array.Copy(first, 0, result, 0, first.Length);
        Array.Copy(second, 0, result, first.Length, second.Length);
        return result;
    }
}