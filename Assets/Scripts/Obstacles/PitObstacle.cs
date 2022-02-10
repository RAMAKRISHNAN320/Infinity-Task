using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitObstacle : Obstacle
{
    PathFollowerTest follower;
    Player player;
    bool isSurferInside;
    BoxCollider collision;
    Vector3[] pointsToCheck = new Vector3[4];
    Vector3 newCenter = Vector3.zero;
    float newHeight = 0;
    public float sinkRate = 5.0f;
    public int[] collumnsOcupy = new int[5];

    PitObstacle()
    {
        type = EObstacleType.PIT;
    }

    private void Start()
    {
        collision = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            player = collision.gameObject.GetComponent<Player>();
            follower = collision.gameObject.GetComponentInParent<PathFollowerTest>();
            AudioManager.instance.Play("pit");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Player>())
        {
            AudioManager.instance.Stop("pit");
        }
    }
    
    //CHECK IF OUTSIDE COLLISION BOUNDS
    private void FixedUpdate()
    {
        if (player)
        {
            pointsToCheck[0] = player.transform.position + player.transform.up * 0.5f + player.transform.forward * .5f;
            pointsToCheck[1] = player.transform.position - player.transform.up * 0.5f + player.transform.forward * .5f;
            pointsToCheck[2] = player.transform.position - player.transform.up * 0.5f - player.transform.forward * .5f;
            pointsToCheck[3] = player.transform.position + player.transform.up * 0.5f - player.transform.forward * .5f;

            pointsToCheck[0].y = 1;
            pointsToCheck[1].y = 1;
            pointsToCheck[2].y = 1;
            pointsToCheck[3].y = 1;


            if (!collision.bounds.Contains(pointsToCheck[0]) || !collision.bounds.Contains(pointsToCheck[1]) || !collision.bounds.Contains(pointsToCheck[2]) || !collision.bounds.Contains(pointsToCheck[3]))
            {
                if (isSurferInside)
                {
                    if (Mathf.Abs(Mathf.FloorToInt(newHeight)) >= player.Height)
                    {
                        //GAME OVER
                        UIManager.instance.ShowGameOver();
                        follower.enabled = false;
                        player = null;
                        AudioManager.instance.Stop("pit");
                        AudioManager.instance.Play("death");

                        return;
                    }
                    newHeight = Mathf.FloorToInt(newHeight);
                    player.transform.localPosition = Vector3.left * (player.Height + newHeight);
                    int heightToRemove = Mathf.Abs(Mathf.FloorToInt(newHeight));
                    player.PitRemove(heightToRemove);
                    player.Height -= heightToRemove;
                    isSurferInside = false;
                    player = null;
                    AudioManager.instance.Stop("pit");

                    return;
                }
            }
            else
            {
                isSurferInside = true;
            }
            if (isSurferInside)
            {
                //SINK CUBE STACK
                player.SetTowerCollision(false);
                newHeight -= sinkRate * Time.fixedDeltaTime;
                player.transform.localPosition = Vector3.left * (player.Height + newHeight) + Vector3.left * .5f;

            }
        }
    }

    //MINIMUN BOX HEIGHT NEEDED TO PASS OBSTACLE
    public int GetLowestCollumnOcupy()
    {
        int lowest = 10;
        for (int i = 0; i < collumnsOcupy.Length; i++)
        {
            if (collumnsOcupy[i] < lowest)
            {
                lowest = collumnsOcupy[i];
            }
        }

        return lowest;
    }
}
