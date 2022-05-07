using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public static List<GameObject> FindObjectsWithTag(this List<GameObject> gameObjects, string tag)
    {
        for (var i = 0; i < gameObjects.Count; i++)
        {
            if (!HasTagManager(gameObjects[i])) gameObjects.Remove(gameObjects[i]);

            var tagManager = gameObjects[i].GetComponent<TagManager>();
            var hasTag = tagManager.HasNameTag(tag);
            
            if (!hasTag) gameObjects.Remove(gameObjects[i]);
        }

        return gameObjects;
    }

    public static List<GameObject> FindObjectsWithTagInChildren(this GameObject parentObject, string tag)
    {
        var childObjectsWithTag = parentObject.GetComponentsInChildren<GameObject>().ToList();

        for (var i = 0; i < childObjectsWithTag.Count; i++)
        {
            if (!HasTagManager(childObjectsWithTag[i])) childObjectsWithTag.Remove(childObjectsWithTag[i]);

            var tagManager = childObjectsWithTag[i].GetComponent<TagManager>();
            var hasTag = tagManager.HasNameTag(tag);
            
            if (!hasTag) childObjectsWithTag.Remove(childObjectsWithTag[i]);
        }

        return childObjectsWithTag;
    }
}