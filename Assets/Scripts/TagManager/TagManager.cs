using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagManager : MonoBehaviour
{
    [SerializeField] private List<string> tags = new List<string>();

    private void Awake()
    {
        TagManagerExtentions.Add(this);
    }

    private void OnDestroy()
    {
        TagManagerExtentions.Remove(this);
    }

    public void AddTag(string tag)
    {
        if (tags.Contains(tag)) return;
        tags.Add(tag);
    }

    public void RemoveTag(string tag)
    {
        if (!tags.Contains(tag)) return;
        tags.Remove(tag);
    }
    
    public void RemoveTag(int index)
    {
        if (!tags.Contains(tags[index])) return;
        tags.RemoveAt(index);
    }

    public bool HasTag(string tag)
    {
        return tags.Contains(tag);
    }
    
    public bool HasTag(List<string> tagList)
    {
        return tags.ContainsAny(tagList);
    }
    
    public bool HasTags(List<string> tagList)
    {
        return tags.Contains(tagList);
    }
}