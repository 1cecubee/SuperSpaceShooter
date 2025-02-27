using UnityEngine;

[CreateAssetMenu(fileName = "Obstacles", menuName = "Types")]
public class Obstacle_Type : ScriptableObject
{

    public string ObstacleName;
    public float ObstacleHealth;
    public float ObstacleScale;
}
