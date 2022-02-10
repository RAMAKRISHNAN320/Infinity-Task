using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EObstacleType
{
    WALL, PIT
}

public class Obstacle : MonoBehaviour
{
    public EObstacleType type;
    public SpawnEntity entity;
}
