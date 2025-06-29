using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SwarmSettings", menuName = "Swarm Settings")]
public class SwarmSettings : ScriptableObject
{   
    [Header("Movement")]
    public float maxSpeed = 5f;
    
    [Header("Boids")]
    public float cohesionWeight = 1.0f;
    public float alignmentWeight = 1.0f;
    public float separationWeight = 1.0f;
    public float searchWeight = 1.0f;
    public float obstacleWeight  = 1.0f;
    
    [Header("Perception")]
    public float neighborRadius = 1.0f;
    public float separationRadius = 1.0f;
    
    public List<Transform> targets;

}
