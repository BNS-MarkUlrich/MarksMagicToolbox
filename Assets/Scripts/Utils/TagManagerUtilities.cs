using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TagManagerUtilities
{
    private static bool HasTagManager(this GameObject gameObject)
    {
        var hasTagManager = gameObject.GetComponent<TagManager>() != null;
        return hasTagManager;
    }

    public static void AddTagManager(this GameObject gameObject)
    {
        if (HasTagManager(gameObject)) return;
        gameObject.AddComponent<TagManager>();
    }

    public static void AddTag(this GameObject gameObject, string tag)
    {
        if (!HasTagManager(gameObject)) AddTagManager(gameObject);

        var tagManager = gameObject.GetComponent<TagManager>();
        tagManager.AddNameTag(tag);
    }
    
    public static void RemoveTag(this GameObject gameObject, string tag)
    {
        if (!HasTagManager(gameObject)) return;

        var tagManager = gameObject.GetComponent<TagManager>();
        tagManager.RemoveNameTag(tag);
    }
    
    public static bool HasTag(this GameObject gameObject, string tag)
    {
        if (!HasTagManager(gameObject)) return false;

        var hasNameTag = gameObject.GetComponent<TagManager>().HasNameTag(tag);
        return hasNameTag;
    }
}