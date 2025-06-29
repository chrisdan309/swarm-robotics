using System.Collections.Generic;
using UnityEngine;

public class DroneSpawner : MonoBehaviour
{
    public GameObject dronePrefab;
    public int droneCount = 10;
    public float minDistance = 1.5f;
    private List<Vector3> usedPositions = new List<Vector3>();
    
    void Start()
    {
        int droneLayer = LayerMask.NameToLayer("Drones");
        for (int i = 0; i < droneCount; i++)
        {
            Vector3 spawnPos;
            int attempts = 0;
            do
            {
                spawnPos = new Vector3(Random.Range(-40,40), 1f, Random.Range(-40,40));
                attempts++;
                if (attempts > 200) break;
            }
            while (IsTooClose(spawnPos));
            
            usedPositions.Add(spawnPos);
            GameObject drone = Instantiate(dronePrefab, spawnPos, Quaternion.identity);
            drone.layer = droneLayer;
        }
    }

    private bool IsTooClose(Vector3 pos)
    {
        foreach (var existing in usedPositions)
        {
            if (Vector3.Distance(pos, existing) < minDistance) return true;
        }
        return false;
    }

}
