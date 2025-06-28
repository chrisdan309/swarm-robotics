using UnityEngine;

public class DroneSpawner : MonoBehaviour
{
    public GameObject dronePrefab;
    public int droneCount = 10;
    
    void Start()
    {
        for (int i = 0; i < droneCount; i++)
        {
            Vector3 position = new Vector3(Random.Range(-10,10), 1, Random.Range(-10,10));
            Instantiate(dronePrefab, position, Quaternion.identity);
        }
    }
    
}
