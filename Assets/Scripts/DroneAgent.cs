using System.Collections.Generic;
using UnityEngine;

public class DroneAgent : MonoBehaviour
{
    private Vector3 _direction;
    public float speed = 3.0f;
    
    public SwarmSettings swarmSettings;
    
    void Start()
    {
        GenerateRandomDirection();
        FaceDirection();
    }
    
    void Update()
    {
        MoveForward();
    }
    
    private void GenerateRandomDirection()
    {
        Vector2 random2D = Random.insideUnitCircle.normalized;
        _direction = new Vector3(random2D.x, 0f, random2D.y);
    }

    private void FaceDirection()
    {
        transform.rotation = Quaternion.LookRotation(_direction);
    }

    private void MoveForward()
    {
        transform.position += transform.forward * (speed * Time.deltaTime);
    }
}
