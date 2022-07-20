using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    private WorldTime _worldTime;
    
    [Range(0, 23)] [SerializeField] private int sunrise = 7;
    [Range(0, 23)] [SerializeField] private int sunset = 20;
    private float _additionMultiplier;
    private float _timeMultiplier;
    private float _rotationMultiplier;
    private float _dayDuration;
    
    private Vector3 _startRotation = Vector3.zero;
    private Vector3 _endRotation = new Vector3(-180, 0, 0);
    private float _sunRotation;
    private float _currentSunRotation;
    private float _stepAngle;

    private float _currentTimeMultiplier;

    private void Start()
    {
        InitWorldTime();
        CalculateRotation();
    }

    private void InitWorldTime()
    {
        _worldTime = FindObjectOfType<WorldTime>();
    }

    private void CalculateRotation()
    {
        _dayDuration = sunset - sunrise;
        _sunRotation = 360 - (_startRotation.x - _endRotation.x);
        _rotationMultiplier = _sunRotation / _dayDuration;
    }

    private void RotateSun()
    {
        var newSunRotation = Vector3.zero;
        
        _timeMultiplier = _worldTime.CurrentTime - sunrise;
        _additionMultiplier = _timeMultiplier * _rotationMultiplier;
        _currentSunRotation = _startRotation.x +  _additionMultiplier;

        newSunRotation.x = _currentSunRotation;

        transform.eulerAngles = newSunRotation;
    }

    private void FixedUpdate()
    {
        RotateSun();
    }
}