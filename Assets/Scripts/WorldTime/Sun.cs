using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    private WorldTime _worldTime;
    
    [Range(0, 23)] [SerializeField] private float currentTime = 4f;
    private float _additionMultiplier;
    private float _timeMultiplier;
    private float _rotationMultiplier;
    [Space]
    [Range(0, 23)] [SerializeField] private int sunrise;
    [Range(0, 23)] [SerializeField] private int sunset;
    private float _dayDuration;
    
    [SerializeField] private Vector3 startRotation = new Vector3(330, 0, 0);
    [SerializeField] private Vector3 endRotation = new Vector3(210, 0, 0);
    private float _sunRotation;
    private float _currentSunRotation;
    private float _stepAngle;

    private float _currentTimeMultiplier;

    private void Start()
    {
        InitWorldTime();
        ResetSunPosition();
        InitSun();
    }

    private void InitWorldTime()
    {
        _worldTime = FindObjectOfType<WorldTime>();
    }

    private void InitSun()
    {
        //_dayDuration = (sunset - sunrise) * 6;
        _dayDuration = _worldTime.DayTimeInMinutes * 60;
        _sunRotation = startRotation.x - endRotation.x;
        _stepAngle = _sunRotation / _dayDuration;
        //_stepAngle *= 1.5f;
    }
    
    public void RotateSun()
    {
        _dayDuration = sunset - sunrise;
        _sunRotation = 360 - (startRotation.x - endRotation.x);
        _rotationMultiplier = _sunRotation / _dayDuration;
        _timeMultiplier = currentTime - sunrise;
        _additionMultiplier = _timeMultiplier * _rotationMultiplier;
        _currentSunRotation = startRotation.x +  _additionMultiplier;

        transform.eulerAngles = new Vector3(_currentSunRotation, 0,0);
    }
    
    public void ResetSunPosition()
    {
        transform.eulerAngles = startRotation;
    }

    private void FixedUpdate()
    {
        /*_currentTimeMultiplier = _worldTime.CurrentTime / 24;
        _sunRotation = startRotation.x / _currentTimeMultiplier;
        transform.eulerAngles = new Vector3(_sunRotation, 0, 0);*/
        RotateSun();
        
        /*var currentTime = _worldTime.CurrentTime;
        if (currentTime >= sunrise)
        {
            transform.RotateAround(transform.position, Vector3.right, _stepAngle * Time.fixedDeltaTime);
        }*/
    }
}