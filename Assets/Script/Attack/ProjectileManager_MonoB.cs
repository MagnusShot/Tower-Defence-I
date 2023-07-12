using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager_MonoB : MonoBehaviour
{
    [SerializeField] float _radius;
    [SerializeField] LayerMask _collisionMask;

    float _speed;
    DamageValues _damageToApply;

    Vector3 _lastPosition;
    bool _stop;



    List<HealthManager_MonoB> hitted = new List<HealthManager_MonoB>();


    public void ApplyValues(float scale, float speed, DamageValues damageToApply)
    {
        _damageToApply = damageToApply;
        _speed = speed;
    }


    // Start is called before the first frame update
    void Start()
    {
        _lastPosition = transform.position;
        _stop = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_stop)
            return;

        //Cria o colisor que pega tudo atingido no frame
        Collider[] results = Physics.OverlapCapsule(_lastPosition, transform.position, _radius, _collisionMask);

        //Adiciona os colisores que possam ter dano aplicado no Hitted
        foreach(Collider r in results)
        {
            _stop = true;
            if (r.TryGetComponent<HealthManager_MonoB>(out HealthManager_MonoB currentHealth))
            {
                hitted.Add(currentHealth);
            }
        }

        //Verificar qual dos personagens atingidos estava mais proximo e aplica dano nele
        if (_stop)
        {
            float minDistance = 100;
            HealthManager_MonoB nearestHitted = null;
            foreach (HealthManager_MonoB hit in hitted)
            {
                if (Vector3.Distance(hit.transform.position, _lastPosition) < minDistance)
                {
                    nearestHitted = hit;
                    minDistance = Vector3.Distance(hit.transform.position, _lastPosition);
                }
            }
            if(nearestHitted != null)
                nearestHitted.DamageApplication(_damageToApply);

            AutoDestroy();
        }

        _lastPosition = transform.position;
        this.transform.position += transform.forward * _speed * Time.deltaTime;
    }

    void AutoDestroy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

}
