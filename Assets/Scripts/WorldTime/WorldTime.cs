using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldTime : MonoBehaviour
{
    private int _day;
    private float _time;
    [Range(0, 23)] [SerializeField] private int startHour = 12;

    [Header("Day/Night Cycle Settings")] 
    [Range(0, 23)] [SerializeField] private int dayStartHour = 6;
    [Range(0, 23)] [SerializeField] private int dayEndHour = 18;
    
    [Space]
    [Range(1, 120)] [SerializeField] private int dayTimeInMinutes = 30;
    [Range(1, 120)] [SerializeField] private int nightTimeInMinutes = 15;
    private int _timeMultiplier;
    [SerializeField] private bool useRealTime;

    [Header("UI Settings")]
    [SerializeField] private bool useUI = true;
    [SerializeField] private Text worldClock;
    [SerializeField] private bool showSeconds;

    private void Start()
    {
        InitClock();
    }

    private void InitClock()
    {
        _time = startHour;
        if (!useUI) return;
        worldClock = GameObject.Find("WorldClock").GetComponent<Text>();
    }

    public int CurrentDay => _day;
    public float CurrentTime => _time;
    
    public bool IsDayTime => _time >= dayStartHour && _time <= dayEndHour;

    private void FixedUpdate()
    {
        if (_time >= 24) AddDay();
        
        if (useRealTime) ScaleTime();
        else ScaleTime(IsDayTime? dayTimeInMinutes : nightTimeInMinutes);


        FillUI();
        _time += Time.deltaTime / 3600 * _timeMultiplier;
    }

    private void ScaleTime(int timeInMinutes = 720)
    {
        _timeMultiplier = 720 / timeInMinutes;
    }

    private string FormatTime()
    {
        var hours = _time;
        var contextHours = Mathf.FloorToInt(hours);
        
        var minutes = (hours - contextHours) * 60;
        var contextMinutes = Mathf.FloorToInt(minutes);

        if (!showSeconds) return $"{contextHours:00}:{contextMinutes:00}";
        
        var seconds = (minutes - contextMinutes) * 60;
        var contextSeconds = Mathf.FloorToInt(seconds);
        
        return $"{contextHours:00}:{contextMinutes:00}:{contextSeconds:00}";
    }

    private void FillUI()
    {
        if (!useUI) return;
        worldClock.text = "Day: " + _day + "\n" + FormatTime();
    }

    private void ResetTime()
    {
        _time = 0;
    }

    private void AddDay()
    {
        ResetTime();
        ++_day;
    }
}