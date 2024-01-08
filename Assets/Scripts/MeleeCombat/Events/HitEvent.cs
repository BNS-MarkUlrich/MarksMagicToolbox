using System;
using UnityEngine;

public class HitEvent : EventArgs
{
    public HitEventType type;
    public Agent aggressor;
    public Agent opponent;
    public BaseWeapon weaponUsed;
    public Vector3 hitPoint;
    public CardinalDirections stanceDirection;
    public Vector2 attackDirection;
}
