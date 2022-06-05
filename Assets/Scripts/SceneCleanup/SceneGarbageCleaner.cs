using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneGarbageCleaner : MonoBehaviour
{
    private List<GameObject> _deadObjects = new List<GameObject>();
    [SerializeField] private int maxBodyCount = 50;

    private int _listIndex;

    void Update()
    {
        UpdateDeadBodies();
    }

    private void UpdateDeadBodies()
    {
        _listIndex = _deadObjects.Count;
        var allObjects = FindObjectsOfType<GameObject>();
        _deadObjects = allObjects.Where(deadObject => deadObject.HasTag("Dead")).ToList();
        if (_deadObjects.Count == _listIndex) return;

        CleanScene();
    }

    private void CleanScene()
    {
        var bodiesToDestroy = _deadObjects.Count - maxBodyCount;
        if (bodiesToDestroy <= 0) return;

        for (var i = 0; i < bodiesToDestroy; i++)
        {
            Destroy(_deadObjects.Last().gameObject);
            _deadObjects.Remove(_deadObjects.Last());
        }
    }
}
