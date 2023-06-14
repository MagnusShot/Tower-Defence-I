using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    float searchEnemiesTimer;
    const float SEARCH_ENEMIES_COUNTDOWN = 1f;
    float currentWaveTimer = 0;
    int wave = -1;
    public enum spawnState { RUNNING,OFF,WAITING_CLEAR };
    private spawnState state = spawnState.OFF;

    public WaveConfigs[] waveArray;

    void Start() {
        if (instance!=null) Destroy(gameObject);
        instance = this;
    }

    void Update()
    {
        if (state==spawnState.OFF) {
            if (Input.GetKeyDown(KeyCode.Q)) StartWave();
        }
        else if (state==spawnState.RUNNING) {
            currentWaveTimer+=Time.deltaTime;
            SpawnWaveContents(waveArray[wave]);
            if (waveArray[wave].FinishedSpawningEnemies()) {
                state=spawnState.WAITING_CLEAR;
            }
        } else if (state==spawnState.WAITING_CLEAR) {
            if (!isEnemiesAlive()) {
                FinishWave();
            }
        }
        
    }

    public void RestartWaves() {
        state = spawnState.OFF;
        wave=-1;
        currentWaveTimer=0;
        KillAllEnemies();
    }

    void KillAllEnemies() {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(go);
        }
    }

    void FinishWave() {
        currentWaveTimer=0;
        state=spawnState.OFF;
    }

    void StartWave() {
        wave++;
        if (wave>=waveArray.Length) wave=0;
        state=spawnState.RUNNING;

        foreach (var waveContent in waveArray[wave].GetWaveContents())
        {
            waveContent.PrepareWaveContent();
        }
    }

    void SpawnWaveContents(WaveConfigs wave) {
        foreach (var waveContent in wave.GetWaveContents())
        {
            if (!(waveContent.startSpawningDelay<currentWaveTimer)) continue;
            if (!waveContent.CanStillSpawnEnemy()) continue;
            if (!waveContent.CanSpawnEnemyDelay(currentWaveTimer)) continue;
            SpawnEnemy(waveContent.enemy);
            waveContent.SpawnEnemy(currentWaveTimer);
        }
    }

    void SpawnEnemy(GameObject enemny) {
        GenerateMap.instance.SpawnEnemy(enemny);
    }

    string WaveNumberDisplay(){ return (wave + 1).ToString();} 

    bool isEnemiesAlive() {
        searchEnemiesTimer-=Time.deltaTime;
        if (searchEnemiesTimer<=0f) {
            searchEnemiesTimer=SEARCH_ENEMIES_COUNTDOWN;
            return GameObject.FindGameObjectsWithTag("Enemy").Length!=0;
        }
        return true;
    }

    void OnGUI() {
        GUI.Label(new Rect(50,64,100,100),"Wave: "+ WaveNumberDisplay());
        GUI.Label(new Rect(50,96,100,100),"Wave State: "+ state.ToString());
    }

}
