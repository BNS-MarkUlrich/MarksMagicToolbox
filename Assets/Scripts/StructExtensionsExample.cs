using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructExtensionsExample : MonoBehaviour
{
    [SerializeField]
    private ExampleStruct _exampleStruct;

    [SerializeField]
    private Transform _target;

    private void Start()
    {
        print($"Before: {_exampleStruct.examplePosition}");
        print($"Before: {_exampleStruct.exampleRadius}");

        _exampleStruct.SetField(nameof(_exampleStruct.examplePosition), _target.position);
        _exampleStruct.SetField(nameof(_exampleStruct.exampleRadius), 10f);
        
        print($"After: {_exampleStruct.examplePosition}");
        print($"After: {_exampleStruct.exampleRadius}");
    }
}
