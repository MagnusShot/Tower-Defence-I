using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicPistolShot_AttackObject", menuName = "ScriptableObjects/Attacks/BasicPistolShot_AttackInstance")]
public class BasicPistolShot_AttackInstance : AttackInstance_Scriptable
{
    [SerializeField] int _maxBullets = 5;
    [SerializeField] float _maxCooldown = 0.2f;

    [SerializeField] ProjectileManager_MonoB _projectile;
    [SerializeField] float _projectileSpeed;

    [SerializeField] DamageValues _damageValues;

    public override int maxBullets => _maxBullets;
    public override float maxCooldown => _maxCooldown;

    public override AttackData SetupAttack()
    {
        AttackData attackData = new AttackData();
        attackData.currentBullets = _maxBullets;
        attackData.currentCooldown = 0;
        attackData.dmgMod = 1;

        return attackData;
    }

    public override bool CallAttack(AttackData data, Transform gunOutput, out AttackData newData)
    {

        Debug.Log(data.currentBullets);
        Debug.Log(data.currentCooldown);
        if (data.currentCooldown <= 0 && data.currentBullets > 0)
        {
            ProjectileManager_MonoB instance = Instantiate(_projectile, gunOutput.position, gunOutput.rotation);
            instance.ApplyValues(1, _projectileSpeed, _damageValues);
            data.currentBullets--;
            Debug.Log("chamado");

            newData = data;

            return true;
        }
        else
        {
            newData = data;
            return false;
        }
    }

    public override bool CallReload(AttackData data, out AttackData newData)
    {
        newData = data;
        if (data.currentBullets == maxBullets)
            return false;

        newData.currentBullets = maxBullets;
        return true;
    }

    public override void UpdateAttack(AttackData data, out AttackData newData)
    {
        if (data.currentCooldown > 0)
        {
            data.currentCooldown -= Time.deltaTime;
        }

        newData = data;
    }


}
