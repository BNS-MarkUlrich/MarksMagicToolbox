using UnityEngine;

public class HitEvent : MonoBehaviour
{
    public HitEventType type;
    public Agent aggressor;
    public Agent opponent;
    public BaseWeapon weaponUsed;
    public Vector3 hitPoint;
    public Vector2 attackDirection;
}
