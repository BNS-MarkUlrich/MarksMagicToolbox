using System;
using UnityEngine;

[Serializable]
public struct WeaponAttributes
{
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private SerializableDictionary<DamageType, float> damageTypes;
    [SerializeField] private float weaponLength;
    [SerializeField] private float weaponSpeed;
    [SerializeField] private float weaponWeight;
    [SerializeField] private float hiltPoint;

    public readonly WeaponType WeaponType => weaponType;
    public readonly SerializableDictionary<DamageType, float> DamageTypes => damageTypes;
    public readonly float WeaponLength => weaponLength;
    public readonly float WeaponSpeed => weaponSpeed;
    public readonly float WeaponWeight => weaponWeight;
    public readonly float HiltPoint => hiltPoint;
}
