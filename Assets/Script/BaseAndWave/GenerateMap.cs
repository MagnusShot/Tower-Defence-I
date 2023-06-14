using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class GenerateMap : MonoBehaviour
{
    public static int GRID_WIDTH = 20;
    public static int GRID_HEIGHT = 20;
    public static float GRID_SIDE = 2.5f;
    public static float GALF_GRID_SIDE = 1.25f;
    public static GenerateMap instance;

    public Vector2[] enemieSpawnPoints;
    public Vector2 enemySpawnPoint;
    public Vector2[] paths;
    public GameObject path;
    public Transform core;
    public NavMeshSurface surface;
    // Start is called before the first frame update
    void Start()
    {
        if (instance!=null) Destroy(this.gameObject);
        instance = this;
        foreach (Vector2 item in paths)
        {
            float xCord = item.x*GRID_SIDE - 23.75f;
            float yCord = item.y*GRID_SIDE - 23.75f;
            GameObject instantiatedObject = Instantiate(path,transform.position + new Vector3(xCord,path.transform.position.y,yCord),new Quaternion(),this.gameObject.transform);
            instantiatedObject.SetActive(true);
        }

        surface.BuildNavMesh();
    }

    public void SpawnEnemy(GameObject enemyToSpawn) {
        float xCord = enemySpawnPoint.x*GRID_SIDE - 23.75f;
        float yCord = enemySpawnPoint.y*GRID_SIDE - 23.75f;
        
        GameObject newEnemy = Instantiate(enemyToSpawn,transform.position + new Vector3(xCord,0,yCord),new Quaternion(),this.gameObject.transform);
        newEnemy.SetActive(true);

        newEnemy.GetComponent<Enemy>().SetTarget(core.position);    
    }
}
