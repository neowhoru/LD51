using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetting : MonoBehaviour
{

    public string levelName = "4-1";
    public string text = "Escape the building";
    public List<Transform> spawnPointsEnemy = new List<Transform>();

    public bool isLastLevel = false;

    public Vector2 GetRandomSpawnPoint()
    {
        Vector2 spawnPoint = Vector2.zero;

        if (spawnPointsEnemy.Count > 0)
        {
            int randomIndex = Random.Range(0, spawnPointsEnemy.Count);
            return spawnPointsEnemy[randomIndex].position;
        }
        
        return spawnPoint;
    }

}
