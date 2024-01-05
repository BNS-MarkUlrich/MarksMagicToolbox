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

    public WeaponType WeaponType => weaponType;
    public SerializableDictionary<DamageType, float> DamageTypes => damageTypes;
    public float WeaponLength => weaponLength;
    public float WeaponSpeed => weaponSpeed;
    public float WeaponWeight => weaponWeight;
}
