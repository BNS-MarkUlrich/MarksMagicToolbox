using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TagManagerExtentions
{
    public static List<TagManager> _tagManagers = new List<TagManager>();
    
    /// <returns>a list of all Tag Managers including this tag.</returns>
    public static List<TagManager> FindAllWithTag(string tag)
    {
        var tagManagers = new List<TagManager>();
        foreach (var tagManager in _tagManagers)
        {
            if (tagManager.HasTag(tag)) tagManagers.Add(tagManager);
        }

        return tagManagers;
    }
    
    /// <returns>a list of all Tag Managers including this collection of tags.</returns>
    public static List<TagManager> FindAllWithTags(List<string> tags)
    {
        var tagManagers = new List<TagManager>();
        foreach (var tagManager in _tagManagers)
        {
            if (tagManager.HasTags(tags)) tagManagers.Add(tagManager);
        }

        return tagManagers;
    }

    public static void Add(TagManager tagManager)
    {
        _tagManagers.Add(tagManager);
    }

    public static void Remove(TagManager tagManager)
    {
        _tagManagers.Remove(tagManager);
    }

    /// <summary>
    /// Checks if the object has a tagmanager.
    /// </summary>
    /// <returns></returns>
    private static bool HasTagManager(this GameObject gameObject)
    {
        var hasTagManager = gameObject.GetComponent<TagManager>() != null;
        return hasTagManager;
    }

    /// <summary>
    /// Adds the tagmanager component to the object.
    /// </summary>
    private static void AddTagManager(this GameObject gameObject)
    {
        if (HasTagManager(gameObject)) return;
        gameObject.AddComponent<TagManager>();
    }

    /// <summary>
    /// Adds a tag to the gameobject.
    /// </summary>
    public static void AddTag(this GameObject gameObject, string tag)
    {
        if (!HasTagManager(gameObject)) AddTagManager(gameObject);

        var tagManager = gameObject.GetComponent<TagManager>();
        tagManager.AddTag(tag);
    }
    
    /// <summary>
    /// Removes a tag from the gameobject.
    /// </summary>
    public static void RemoveTag(this GameObject gameObject, string tag)
    {
        if (!HasTagManager(gameObject)) return;

        var tagManager = gameObject.GetComponent<TagManager>();
        tagManager.RemoveTag(tag);
    }
    
    /// <summary>
    /// Removes a tag from the gameobject at index.
    /// </summary>
    public static void RemoveTag(this GameObject gameObject, int index)
    {
        if (!HasTagManager(gameObject)) return;

        var tagManager = gameObject.GetComponent<TagManager>();
        tagManager.RemoveTag(index);
    }
    
    /// <summary>
    /// Returns true if object has tag.
    /// </summary>
    /// <returns></returns>
    public static bool HasTag(this GameObject gameObject, string tag)
    {
        if (!HasTagManager(gameObject)) return false;

        var hasNameTag = gameObject.GetComponent<TagManager>().HasTag(tag);
        return hasNameTag;
    }
    
    /// <summary>
    /// Returns true if object has any of the tags.
    /// </summary>
    /// <returns></returns>
    public static bool HasTag(this GameObject gameObject, List<string> tagList)
    {
        if (!HasTagManager(gameObject)) return false;

        var hasNameTag = gameObject.GetComponent<TagManager>().HasTag(tagList);
        return hasNameTag;
    }
    
    /// <summary>
    /// Returns true if the tagmanager contains all the tags in the list.
    /// </summary>
    /// <returns></returns>
    public static bool HasTags(this GameObject gameObject, List<string> tagList)
    {
        if (!HasTagManager(gameObject)) return false;
        
        var hasNameTags = gameObject.GetComponent<TagManager>().HasTags(tagList);
        return hasNameTags;
    }
}