using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DroneAgent : MonoBehaviour
{
    private Vector3 _direction;
    public float speed = 3.0f;
    public SwarmSettings swarmSettings;

    private Rigidbody rb;

    public float rotationSpeed = 2.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        GenerateRandomDirection();
        FaceDirection();
    }

    void Update()
    {
        List<Transform> neighbors = GetNeighbors();
        
        Vector3 forceSeparation = ComputeSeparation(neighbors);
        Vector3 forceAlignment = ComputeAlignment(neighbors);
        Vector3 forceCohesion = ComputeCohesion(neighbors);
        Vector3 forceSearch = ComputeTargetSeeking(transform.position, swarmSettings.targets);
        Vector3 forceAvoid = ComputeObstacleAvoidance() * swarmSettings.obstacleWeight;
        
        
        bool mustAvoid = forceAvoid.magnitude > 0.1f;
        if (mustAvoid)
        {
            Debug.Log(forceAvoid);    
        }

        float wCohesion = mustAvoid ? 0f : swarmSettings.cohesionWeight;
        float wAlignment = mustAvoid ? 0f : swarmSettings.alignmentWeight;
        float wSeparation = swarmSettings.separationWeight;
        float wSearch = mustAvoid ? 0.2f : swarmSettings.searchWeight;
        
        Vector3 totalForce =
            wSeparation * forceSeparation +
            wAlignment * forceAlignment +
            wCohesion * forceCohesion +
            wSearch * forceSearch +
            forceAvoid;
        
        Vector3 flatForce = new Vector3(totalForce.x, 0f, totalForce.z);

        if (flatForce == Vector3.zero) return;
        
        Vector3 finalDir = flatForce.normalized * swarmSettings.maxSpeed;
        
        Quaternion targetRotation = Quaternion.LookRotation(finalDir);
        Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        rb.MoveRotation(smoothedRotation);
        
        Vector3 currentPosition = rb.position;
        Vector3 newPosition = currentPosition + finalDir * Time.deltaTime;
        newPosition.y = currentPosition.y;

        rb.MovePosition(newPosition);
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
            Vector3 pos = neighbor.position;
            pos.y = 0f;
            center += pos;
        }

        center /= neighbors.Count;
        Vector3 myPos = transform.position;
        myPos.y = 0f;

        Vector3 dir = center - myPos;
        return dir.normalized;
    }
    
    private Vector3 ComputeAlignment(List<Transform> neighbors)
    {
        if (neighbors.Count == 0) return Vector3.zero;

        Vector3 averageDir = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            Vector3 forwardXZ = neighbor.forward;
            forwardXZ.y = 0f;
            averageDir += forwardXZ.normalized;
        }

        averageDir /= neighbors.Count;
        return averageDir.normalized;
    }
    
    
    private Vector3 ComputeSeparation(List<Transform> neighbors)
    {
        if (neighbors.Count == 0) return Vector3.zero;

        Vector3 repulsion = Vector3.zero;
        Vector3 myPos = transform.position;
        myPos.y = 0f;

        foreach (var neighbor in neighbors)
        {
            Vector3 neighborPos = neighbor.position;
            neighborPos.y = 0f;

            Vector3 toMe = myPos - neighborPos;
            float distance = toMe.magnitude;

            if (distance > 0f && distance < swarmSettings.separationRadius)
            {
                repulsion += toMe.normalized / (distance * distance);
            }
        }

        return repulsion.normalized;
    }
    
    private Vector3 ComputeObstacleAvoidance()
    {
        RaycastHit hit;
        Vector3 forward = transform.forward;
        float rayDistance = 2.0f;
        float rayRadius = 3f;

        if (Physics.SphereCast(transform.position, rayRadius, forward, out hit, rayDistance, LayerMask.GetMask("Wall")))
        {
            Vector3 hitNormal = hit.normal;
            Vector3 avoidDir = Vector3.Reflect(forward, hitNormal);
            avoidDir.y = 0f;
            return avoidDir.normalized * (1.0f / hit.distance);
        }

        return Vector3.zero;
    }

    
    private Vector3 ComputeTargetSeeking(Vector3 myPos, List<Transform> targets)
    {
        if (targets == null || targets.Count == 0) return Vector3.zero;

        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (var target in targets)
        {
            float dist = Vector3.Distance(myPos, target.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = target;
            }
        }

        if (closest == null) return Vector3.zero;

        Vector3 dir = closest.position - myPos;
        dir.y = 0f;
        return dir.normalized;
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
}
