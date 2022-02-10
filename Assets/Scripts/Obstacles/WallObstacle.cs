using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HasBlock
{
    public bool[] atHeight;
}

public class WallObstacle : Obstacle
{
    public int[] offsetHeight = new int[5];
    public HasBlock[] hasBlock = new HasBlock[5];
    public int scoreMultiplier = 1;
    public bool isFinish;

    

    WallObstacle()
    {
        type = EObstacleType.WALL;
    }

    //MINIMUN BOX HEIGHT NEEDED TO PASS OBSTACLE
    public int GetLowestPoint()
    {
        int lowest = 10;
        for (int i = 0; i < offsetHeight.Length; i++)
        {
            if (offsetHeight[i] < lowest)
            {
                lowest = offsetHeight[i];
            }
        }

        return lowest;
    }
}
