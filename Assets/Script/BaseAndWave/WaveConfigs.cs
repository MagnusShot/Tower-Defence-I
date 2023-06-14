using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New WaveConfig",menuName ="WaveConfig/New WaveConfig")]
public class WaveConfigs : ScriptableObject
{
    [SerializeField]
    WaveContent[] waveContents;

    public WaveContent GetWave(int wave) {
        return waveContents[wave];
    }

    public WaveContent[] GetWaveContents() {
        return waveContents;
    }

    public bool FinishedSpawningEnemies() {
        foreach (var waveContent in waveContents)
        {
            if (waveContent.CanStillSpawnEnemy()) {
                return false;
            }
        }
        return true;
    }
}
