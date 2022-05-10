using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TagManagerUtilities
{
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
        tagManager.AddNameTag(tag);
    }
    
    /// <summary>
    /// Removes a tag to the gameobject.
    /// </summary>
    public static void RemoveTag(this GameObject gameObject, string tag)
    {
        if (!HasTagManager(gameObject)) return;

        var tagManager = gameObject.GetComponent<TagManager>();
        tagManager.RemoveNameTag(tag);
    }
    
    /// <summary>
    /// Returns true of object has tag.
    /// </summary>
    /// <returns></returns>
    public static bool HasTag(this GameObject gameObject, string tag)
    {
        if (!HasTagManager(gameObject)) return false;

        var hasNameTag = gameObject.GetComponent<TagManager>().HasNameTag(tag);
        return hasNameTag;
    }
}