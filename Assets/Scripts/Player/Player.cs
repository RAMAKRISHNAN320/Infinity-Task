using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    int height = 0;
    private PathFollowerTest follower;
    private List<GameObject> cubesHeight = new List<GameObject>();

    private bool[] blocksToRemove;

    bool insideObstacle;
    public Collision insideCollision;
    int heightToRemove = 0;

    public float stackAdjustmentSpeed = 15f;


    private GameObject hitObj;
    public int Height
    {
        get
        {
            return height;
        }
        set
        {
            if (value > height)
            {

                //ADD CUBES UNDER TRANSFORM
                transform.localPosition = Vector3.left * (value) + Vector3.left * .5f;
                for (int i = 0; i < value - height; i++)
                {

                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cubesHeight.Add(cube);
                    cube.layer = LayerMask.NameToLayer("HeightBlocks");
                    cube.GetComponent<MeshRenderer>().materials = GetComponent<MeshRenderer>().materials;
                    Rigidbody rb = cube.AddComponent<Rigidbody>();
                    rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                    rb.mass = 1000;
                    rb.useGravity = false;
                    cube.transform.SetParent(transform);
                    //CUBES ROTATION IS OFFSET BECAUSE OF PATH FOLLOW, RIGHT IS DOWN
                    cube.transform.localPosition = Vector3.right * cubesHeight.Count;
                    cube.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                //UPDATE COLLIDER TO BE SIZE OF WHOLE STACK
                Vector3 newCenter = Vector3.zero;
                Vector3 newHeight = Vector3.one;
                newCenter.x = -((transform.localPosition.x + .5f) / 2);
                newHeight.x = value + 1;
                GetComponent<BoxCollider>().center = newCenter;
                GetComponent<BoxCollider>().size = newHeight;
                Camera.main.GetComponent<CameraFollow>().Distance += value - height;
                height = value;

                StartCoroutine(SetStackHeight());

            }
            if (value < height)
            {
                if (height - value > 0)
                {
                    Camera.main.GetComponent<CameraFollow>().Distance += value - height;
                    height = value;
                    //UPDATE COLLIDER TO BE SIZE OF WHOLE STACK
                    Vector3 newCenter = Vector3.zero;
                    Vector3 newHeight = Vector3.one;
                    newCenter.x = ((height + 1) / 2) - .5f;
                    if (height <= 0)
                    {
                        newCenter.x = 0;
                    }
                    newHeight.x = height + 1; 
                    GetComponent<BoxCollider>().center = newCenter;
                    GetComponent<BoxCollider>().size = newHeight;
                    StartCoroutine(SetStackHeight());
                }
                
            }
            
        }
    }

    private void Awake()
    {
        follower = GetComponentInParent<PathFollowerTest>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        hitObj = collision.gameObject;
        SetTowerCollision(false);
        SetStackGravity(false);

        //HIT HEIGHTBLOCK
        if (collision.gameObject.GetComponent<HeightBlock>())
        {
            hitObj = collision.gameObject;
            Height += collision.gameObject.GetComponent<HeightBlock>().height;
            Destroy(collision.gameObject.transform.parent.parent.gameObject);

            AudioManager.instance.Play("box_grow");

            return;
        }

        //HIT COIN
        if (collision.gameObject.GetComponent<Coin>())
        {
            GameManager.instance.score += 1;
            Destroy(collision.gameObject.transform.parent.parent.gameObject);
            AudioManager.instance.Play("coin");

            return;
        }

        //HIT WALL
        if (collision.gameObject.GetComponentInParent<WallObstacle>())
        {
            insideObstacle = true;
            SetStackGravity(false);
            SetTowerStackPositions();

            int[] offsetLimits = new int[2];
            //SPLIT ROAD INTO COLUMNS FROM 0 TO 4
            //LEFTMOST COLUMN IS 0
            offsetLimits[0] = Mathf.Clamp(Mathf.FloorToInt(follower.Offset + follower.offsetLimit - .5f), 0, 4);
            offsetLimits[1] = Mathf.Clamp(Mathf.CeilToInt(follower.Offset + follower.offsetLimit + .5f), 0, 4);

            //CHECK WHAT COLLUMN SHOULD BE CONSIDERED HIT
            int obstacleHeight = Mathf.Max(collision.gameObject.GetComponentInParent<WallObstacle>().offsetHeight[offsetLimits[0]], collision.gameObject.GetComponentInParent<WallObstacle>().offsetHeight[offsetLimits[1]]);
            int hitColl = -1;
            if (obstacleHeight == collision.gameObject.GetComponentInParent<WallObstacle>().offsetHeight[offsetLimits[0]])
            {
                hitColl = offsetLimits[0];
            }
            if (obstacleHeight == collision.gameObject.GetComponentInParent<WallObstacle>().offsetHeight[offsetLimits[1]])
            {
                hitColl = offsetLimits[1];
            }

            //CHECK HOW MANY BLOCKS TO REMOVE
            if (hitColl >= 0)
            {
                blocksToRemove = collision.gameObject.GetComponentInParent<WallObstacle>().hasBlock[hitColl].atHeight;
            }


            //UPDATE END SCORE MULTIPLIER
            if (collision.gameObject.GetComponentInParent<WallObstacle>().isFinish)
            {
                GameManager.instance.scoreMultiplier = collision.gameObject.GetComponentInParent<WallObstacle>().scoreMultiplier;
            }

            //CHECK IS CURRENT HEIGHT IS ENOUGH TO PASS
            if (height >= obstacleHeight)
            {
                //PASS

                heightToRemove = 0;

                //REMOVE BLOCKS AT NEEDED HEIGHT AND SET HEIGHT TO REMOVE
                for (int i = 0; i < blocksToRemove.Length; i++)
                {
                    if (blocksToRemove[i])
                    {
                        cubesHeight[(cubesHeight.Count + heightToRemove) - (1 + i)].transform.SetParent(null);
                        cubesHeight[(cubesHeight.Count + heightToRemove) - (1 + i)].GetComponent<Rigidbody>().useGravity = true;
                        cubesHeight[(cubesHeight.Count + heightToRemove) - (1 + i)].GetComponent<BoxCollider>().enabled = true;
                        cubesHeight.RemoveAt((cubesHeight.Count + heightToRemove) - (1 + i));
                        heightToRemove += 1;
                    }
                }
                foreach (WallObstacle wall in collision.gameObject.transform.parent.parent.GetComponentsInChildren<WallObstacle>())
                {
                    if (wall.isFinish && heightToRemove > 0)
                    {
                        Height--;
                        Camera.main.GetComponent<CameraFollow>().Height += 1f;
                        break; 
                    }
                }

                if (heightToRemove > 0)
                {
                    AudioManager.instance.Play("wall_hit");
                }

            }
            else
            {
                //CHECK IF HIT WALL IS A FINISH WALL
                if (collision.gameObject.GetComponentInParent<WallObstacle>().isFinish)
                {
                    //WIN
                    follower.enabled = false;

                    PlayerPrefs.SetInt("completedLevel", PlayerPrefs.GetInt("currentLevel", 1));
                    UIManager.instance.NextLevel(GameManager.instance.score * GameManager.instance.scoreMultiplier);
                }
                else
                {
                    AudioManager.instance.Play("death");
                    //GAME OVER
                    UIManager.instance.ShowGameOver();
                    follower.enabled = false;
                    
                }


            }
        }

        //HIT PIT DETECTION MADE INSIDE PitObstacle
    }

    //CLEAR SOME DATA
    public void OnCollisionExit(Collision collision)
    {
        if (hitObj)
        {
            if (hitObj == collision.gameObject)
            {
                insideObstacle = false;
                hitObj = null;
                Height -= heightToRemove;
                blocksToRemove = null;
            }
        }
    }

    public void SetTowerCollision(bool _value)
    {
        foreach (GameObject segment in cubesHeight)
        {
            segment.GetComponent<BoxCollider>().enabled = _value;
        }
    }

    //ONLY UPDATE STACK HEIGHT AFTER PLAYER HAS LEFT THE COLLISION BOUNDS
    private void Update()
    {
        if (insideObstacle)
        {
            Vector3[] pointsToCheck = new Vector3[4];

            pointsToCheck[0] = transform.position + transform.up * 0.5f + transform.forward * .5f;
            pointsToCheck[1] = transform.position - transform.up * 0.5f + transform.forward * .5f;
            pointsToCheck[2] = transform.position - transform.up * 0.5f - transform.forward * .5f;
            pointsToCheck[3] = transform.position + transform.up * 0.5f - transform.forward * .5f;

            pointsToCheck[0].y = 1;
            pointsToCheck[1].y = 1;
            pointsToCheck[2].y = 1;
            pointsToCheck[3].y = 1;
            if (hitObj)
            {
                if (hitObj.GetComponent<BoxCollider>())
                {
                    if (!hitObj.GetComponent<BoxCollider>().bounds.Contains(pointsToCheck[0]) && !hitObj.GetComponent<BoxCollider>().bounds.Contains(pointsToCheck[1]) && !hitObj.GetComponent<BoxCollider>().bounds.Contains(pointsToCheck[2]) && !hitObj.GetComponent<BoxCollider>().bounds.Contains(pointsToCheck[3]))
                    {
                        insideObstacle = false;
                        hitObj = null;
                        Height -= heightToRemove;
                        blocksToRemove = null;
                    }
                }
            }
        }
    }
    
    //CORRECT STACK BLOCKS POSITIONS
    private void SetTowerStackPositions()
    {
        for (int i = 0; i < cubesHeight.Count; i++)
        {
            cubesHeight[i].transform.localPosition = Vector3.right * (i + 1);
        }
    }

    private void SetStackGravity(bool _value)
    {
        foreach (GameObject item in cubesHeight)
        {
            item.GetComponent<Rigidbody>().useGravity = _value;
        }
    }

    //REMOVE
    public void PitRemove(int _value)
    {
        int amount = Mathf.Clamp(_value, 0, cubesHeight.Count);

        for (int i = 0; i < amount; i++)
        {
            Destroy(cubesHeight[cubesHeight.Count - 1]);
            cubesHeight.RemoveAt(cubesHeight.Count - 1);
        }
    }    

    //CHECK IF BLOCKS ARE AT THEIR CORRECT HEIGHT POSITION
    bool IsHeightStacked()
    {
        for (int i = 0; i < cubesHeight.Count; i++)
        {
            if (Vector3.Distance(cubesHeight[i].transform.localPosition, Vector3.right * (i + 1)) > 0.01f)
            {
                return false;
            }
        }
        return true;
    }

    //SMOOTH CORRECTION OF STACK BLOCKS POSITIONS
    IEnumerator SetStackHeight()
    {

        while (!IsHeightStacked())
        {
            for (int i = 0; i < cubesHeight.Count; i++)
            {
                Vector3 desiredPosition = Vector3.right * (i + 1);
                Vector3 smoothedPosition = Vector3.Lerp(cubesHeight[i].transform.localPosition, desiredPosition, stackAdjustmentSpeed * Time.fixedDeltaTime);
                cubesHeight[i].transform.localPosition = smoothedPosition;
            }
            yield return null;
        }
    }
}
