using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    /// <summary>
    /// Checks if compareTarget includes target List.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="compareTarget"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool Contains<T>(this IList<T> target, IList<T> compareTarget)
    {
        foreach (var tag in compareTarget)
        {
            if (!target.Contains(tag)) return false;
        }
            
        return true;
    }
    
    /// <summary>
    /// Checks if compareTarget includes any of target List.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="compareTarget"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool ContainsAny<T>(this IList<T> target, IList<T> compareTarget)
    {
        foreach (var tag in compareTarget)
        {
            if (target.Contains(tag)) return true;
        }
            
        return false;
    }
}
