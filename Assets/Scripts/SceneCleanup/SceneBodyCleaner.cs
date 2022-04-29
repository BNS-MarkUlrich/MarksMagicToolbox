using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneBodyCleaner : MonoBehaviour
{
    [SerializeField] private List<GameObject> deadObjects = new List<GameObject>(); // Mark: Serialised for debugging
    [SerializeField] private int maxBodyCount = 50;

    private int listIndex;

    void Update()
    {
        UpdateDeadBodies();
    }

    private void UpdateDeadBodies()
    {
        listIndex = deadObjects.Count;
        var allObjects = FindObjectsOfType<GameObject>();
        deadObjects = allObjects.Where(deadObject => deadObject.GetComponent<TagManager>().HasNameTag("Dead")).ToList();
        if (deadObjects.Count == listIndex) return;

        CleanScene();
    }

    private void CleanScene()
    {
        var bodiesToDestroy = deadObjects.Count - maxBodyCount;
        if (bodiesToDestroy <= 0) return;

        for (var i = 0; i < bodiesToDestroy; i++)
        {
            Destroy(deadObjects.Last().gameObject);
            deadObjects.Remove(deadObjects.Last());
        }
    }
}
