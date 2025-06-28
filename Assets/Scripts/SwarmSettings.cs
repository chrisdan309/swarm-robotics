using UnityEngine;

[CreateAssetMenu(fileName = "SwarmSettings", menuName = "Swarm Settings")]
public class SwarmSettings : ScriptableObject
{   
    [Header("Boids")]
    public float cohesionWeight = 1.0f;
    public float alignmentWeight = 1.0f;
    public float separationWeight = 1.0f;
    
    [Header("Percepción")]
    public float neighborRadius = 1.0f;
    public float separationRadius = 1.0f;
}
