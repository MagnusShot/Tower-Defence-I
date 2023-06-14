using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemySO enemyConfig;
    public Vector3 target;
    public float navMeshAgentRadius;
    NavMeshAgent nav;
    bool startedMoving;
    float attackTimer;
    bool isInRange;
    bool reachedCore=false;
    float currentHp;
    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        nav.stoppingDistance = enemyConfig.attackRange;
        nav.speed = enemyConfig.movementSpeed;
        currentHp=enemyConfig.maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        HandleTimers();
        if (Vector3.Magnitude(target-transform.position)<enemyConfig.attackRange) {
            isInRange=true;
        } else {
            isInRange = false;
            attackTimer=0;
        }

    }

    void HandleTimers() {
        if (!isInRange) return;
        attackTimer+=Time.deltaTime;
        if (attackTimer>1/enemyConfig.attaskSpeed) Attack();
    }

    public void SetTarget(Vector3 targetPosition) {
        target=targetPosition;
        if (nav==null) nav = GetComponent<NavMeshAgent>();
        nav.SetDestination(new Vector3(target.x,transform.position.y,target.z));
    }

    public void ReachedCore() {
        reachedCore=true;
        nav.stoppingDistance=0.2f;
        nav.SetDestination(transform.position);
    }

    void Attack() {
        BaseManager.instance.TakeDamage(enemyConfig.dmg);
        attackTimer=0;
    }

    void TakeDamage(float dmg) {
        currentHp-=dmg;
        if (currentHp<=0) Die();
    }

    void Die() {
        Debug.Log("I died");
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawSphere(nav.destination, 1);
    }
}
