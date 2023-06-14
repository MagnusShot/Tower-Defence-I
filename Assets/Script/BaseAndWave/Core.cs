using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision) {
        GameObject objectCollided = collision.gameObject;
        Enemy enemy = objectCollided.GetComponent<Enemy>();
        if (enemy==null) return;
        Debug.Log("alo");
        enemy.ReachedCore();
    }
}
