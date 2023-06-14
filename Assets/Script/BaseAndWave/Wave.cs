using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveContent {
        public int enemiesToSpawnCount;
        public GameObject enemy;
        public float delayBetweenEnemies;
        public float startSpawningDelay;

        int _enemiesToSpawnCount;
        float _lastSpawnedEnemyTime;

        public void PrepareWaveContent() {
            _enemiesToSpawnCount=enemiesToSpawnCount;
            _lastSpawnedEnemyTime=0;
        }

        public bool CanStillSpawnEnemy() {
            return _enemiesToSpawnCount>0;
        }

        public bool CanSpawnEnemyDelay(float spawnTime) {
            return spawnTime>_lastSpawnedEnemyTime+delayBetweenEnemies;
        }

        public void SpawnEnemy(float spawnTime) {
            _enemiesToSpawnCount--;
            _lastSpawnedEnemyTime=spawnTime;
        }
    }