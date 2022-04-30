using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagManager : MonoBehaviour
{
    [SerializeField] private List<string> tags = new List<string>();

    public void AddNameTag(string tag)
    {
        if (tags.Contains(tag)) return;
        tags.Add(tag);
    }

    public void RemoveNameTag(string tag)
    {
        if (!tags.Contains(tag)) return;
        tags.Remove(tag);
    }

    public bool HasNameTag(string tag)
    {
        return tags.Contains(tag);
    }
}