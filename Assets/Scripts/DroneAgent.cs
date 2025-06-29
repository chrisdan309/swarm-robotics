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
        List<Transform> neighbors = GetNeighbors();
        Vector3 cohesion = ComputeCohesion(neighbors);
        Vector3 alignment = ComputeAlignment(neighbors);
        Vector3 separation = ComputeSeparation(neighbors);
        
        Vector3 force = cohesion * swarmSettings.cohesionWeight + 
                        alignment * swarmSettings.alignmentWeight + 
                        separation * swarmSettings.separationWeight;
        
        Vector3 finalDir = force.normalized * swarmSettings.maxSpeed;

        if (finalDir != Vector3.zero)
        {
            transform.forward = finalDir.normalized;
        }
        
        transform.position += finalDir * Time.deltaTime;
        
        MoveForward();
        AdjustHeightToGround();

    }

    private List<Transform> GetNeighbors()
    {
        int droneLayerMask = LayerMask.GetMask("Drones");
        Collider[] colliders = Physics.OverlapSphere(transform.position, swarmSettings.neighborRadius, droneLayerMask);
        List<Transform> neighbors = new List<Transform>();

        foreach (Collider col in colliders)
        {
            if (col.gameObject != gameObject)
            {
                neighbors.Add(col.transform);
            }
        }
        
        return neighbors;
    }

    private Vector3 ComputeCohesion(List<Transform> neighbors)
    {
        if (neighbors.Count == 0) return Vector3.zero;
        
        Vector3 center = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            center += neighbor.position;
        }
        
        center /= neighbors.Count;
        return (center - transform.position).normalized;
    }
    
    private Vector3 ComputeAlignment(List<Transform> neighbors)
    {
        if (neighbors.Count == 0) return Vector3.zero;
        
        Vector3 averageDir = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            averageDir += neighbor.forward;
        }
        
        averageDir /= neighbors.Count;
        return averageDir.normalized;
    }
    
    private Vector3 ComputeSeparation(List<Transform> neighbors)
    {
        if (neighbors.Count == 0) return Vector3.zero;
        
        Vector3 repulsion = Vector3.zero;
        int count = 0;
        
        foreach (var neighbor in neighbors)
        {
            Vector3 directionToMe = transform.position - neighbor.position;
            float distance = directionToMe.magnitude;
            if (distance > 0f && distance < swarmSettings.separationRadius)
            {
                repulsion += directionToMe.normalized / (distance * distance);
                count++;
            }
        }
        if (count == 0) return Vector3.zero;
        return repulsion.normalized;
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
        Vector3 flatForward = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
        transform.position += flatForward * (speed * Time.deltaTime);

    }
    
    private void AdjustHeightToGround()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 5f;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 10f, LayerMask.GetMask("Ground")))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y + 0.5f;
            transform.position = pos;
        }
    }

}
