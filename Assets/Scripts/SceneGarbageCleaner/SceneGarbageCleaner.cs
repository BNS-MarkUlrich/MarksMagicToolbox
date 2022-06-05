using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneGarbageCleaner : MonoBehaviour
{
    private List<GameObject> _garbageObjects = new List<GameObject>();
    [SerializeField] private int maxBodyCount = 50;

    private int _listIndex;

    private void Update()
    {
        UpdateDeadBodies();
    }

    private void UpdateDeadBodies()
    {
        _listIndex = _garbageObjects.Count;
        var allObjects = FindObjectsOfType<GameObject>();
        _garbageObjects = allObjects.Where(garbageObject => garbageObject.HasTag("Dead")).ToList();
        if (_garbageObjects.Count == _listIndex) return;

        CleanScene();
    }

    private void CleanScene()
    {
        var bodiesToDestroy = _garbageObjects.Count - maxBodyCount;
        if (bodiesToDestroy <= 0) return;

        for (var i = 0; i < bodiesToDestroy; i++)
        {
            Destroy(_garbageObjects.Last().gameObject);
            _garbageObjects.Remove(_garbageObjects.Last());
        }
    }
}