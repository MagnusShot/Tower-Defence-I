using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager : MonoBehaviour
{
    public const float STARTING_HEALTH = 10000f;
    float health;
    public static BaseManager instance;

    void Start() {
        if (instance!=null) Destroy(gameObject);
        instance=this;

        health=STARTING_HEALTH;
    }


    public void TakeDamage(float damage) {
        health-=damage;
        if (health<=0) {
            LoseGame();
        }
    }

    void LoseGame() {
        Debug.Log("You've Lost!!");
        WaveManager.instance.RestartWaves();
        health=STARTING_HEALTH;
    }
    
    void OnGUI() {
        GUI.Label(new Rect(200,64,100,100),"Base Health: "+ health.ToString());
    }
}
