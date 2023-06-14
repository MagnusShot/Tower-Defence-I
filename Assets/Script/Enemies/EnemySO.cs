using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Enemy",menuName ="Enemy/New Enemy")]
public class EnemySO : ScriptableObject
{
    public float maxHp;
    public float dmg;
    public float attaskSpeed;
    public float movementSpeed;
    public float attackRange;
}
