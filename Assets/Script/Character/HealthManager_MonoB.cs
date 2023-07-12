using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthManager_MonoB : MonoBehaviour
{
    [SerializeField] float _maxHealth;
    [SerializeField] float _currentHealth;

    public void Start()
    {
        _currentHealth = _maxHealth;
    }

    public void DamageApplication(DamageValues damageData)
    {
        _currentHealth -= damageData.damage;
        if (_currentHealth <= 0)
            Destruction();
    }

    public void Destruction()
    {
        Destroy(gameObject);
    }
}

[Serializable]
public struct DamageValues
{
    public float damage;
}