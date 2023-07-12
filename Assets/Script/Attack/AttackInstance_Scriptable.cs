using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackInstance_Scriptable : ScriptableObject
{
    public abstract int maxBullets { get; }
    public abstract float maxCooldown { get; }
    /// <summary>
    /// Aplica o Setup basico da habilidade
    /// </summary>
    /// <param name="dmgMod"></param>
    /// <param name="rangeMod"></param>
    /// <returns></returns>
    public abstract AttackData SetupAttack();
    /// <summary>
    /// Chama um ataque com o modificador de dano e range
    /// </summary>
    /// <param name="data"></param>
    /// <param name="gunOutput"></param>
    /// <param name="newData"></param>
    /// <returns></returns>
    public abstract bool CallAttack(AttackData data, Transform gunOutput, out AttackData newData);
    /// <summary>
    /// Chama a recarga de balas
    /// </summary>
    /// <param name="data"></param>
    /// <param name="newData"></param>
    /// <returns></returns>
    public abstract bool CallReload(AttackData data, out AttackData newData);
    /// <summary>
    /// Atualiza Valores do Ataque
    /// </summary>
    /// <param name="cooldownSpeed"></param>
    public abstract void UpdateAttack(AttackData data, out AttackData newData);

}


/// <summary>
/// valores salvos de cada habilidade
/// </summary>
public struct AttackData
{
    public float currentCooldown;
    public int currentBullets;

    public float dmgMod;
}

