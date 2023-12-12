using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        => compareTarget.All(tag => target.Contains(tag));
    
    /// <summary>
    /// Checks if compareTarget includes any of target List.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="compareTarget"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool ContainsAny<T>(this IList<T> target, IList<T> compareTarget) 
        => compareTarget.Any(tag => target.Contains(tag));

    /// <summary>
    /// Returns a new list that excludes the specified elements from the original list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The original list.</param>
    /// <param name="excludeParams">The elements to exclude from the list.</param>
    /// <returns>A new list without the specified elements.</returns>
    public static List<T> GetListWithout<T>(List<T> list, params T[] excludeParams) 
        => list.Where(wp => !excludeParams.Contains(wp)).ToList();

    /// <summary>
    /// Returns a new list that contains all elements from the input list, excluding elements from the excludeList and excludeParams.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The input list.</param>
    /// <param name="excludeList">The list of elements to exclude.</param>
    /// <param name="excludeParams">The individual elements to exclude.</param>
    /// <returns>A new list that contains all elements from the input list, excluding elements from the excludeList and excludeParams.</returns>
    public static List<T> GetListWithout<T>(List<T> list, List<T> excludeList, params T[] excludeParams) 
        => list.Where(wp => !excludeList.Contains(wp) && !excludeParams.Contains(wp)).ToList();
}
