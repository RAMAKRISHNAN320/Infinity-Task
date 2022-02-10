using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;
using UnityEditor;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

    }

    [SerializeField, HideInInspector]
    PathCreator creator;
    [SerializeField, HideInInspector]
    RoadMeshCreator road;
    [SerializeField, HideInInspector]
    PathSpawnerTest spawner;
    [SerializeField]
    Level level;
    [SerializeField]
    PlayerController controller;

    [SerializeField]
    List<Level> createdLevels = new List<Level>();

    [SerializeField]
    PathFollowerTest surfer;

    public int score;
    public int scoreMultiplier;

    private void Start()
    {
        //LOAD OR GENERATE NEW LEVEL
        if (PlayerPrefs.GetInt("completedLevel", 0) >= PlayerPrefs.GetInt("currentLevel", 0))
        {
            Debug.Log("Generating new Level...");

            GenerateNewLevel(PlayerPrefs.GetInt("completedLevel", 0) + 1);
        }
        LoadLevel(PlayerPrefs.GetInt("currentLevel", 0));

        UIManager.instance.UpdateUI();
    }

    public void LoadLevel(int _id)
    {
        //ERRORS WITH SERIALIZATION
        //LevelData data = SaveLoad.LoadData(_id);
        //if (data == null)
        //{
        //    return;
        //}

        Level newLevel = ScriptableObject.CreateInstance<Level>();
        //newLevel.id = data.id;
        newLevel.creator = level.creator;
        newLevel.spawner = level.spawner;
        GameObject creatorPrefab = Instantiate(newLevel.creator.gameObject);
        GameObject spawnerPrefab = Instantiate(newLevel.spawner.gameObject);

        creator = creatorPrefab.GetComponent<PathCreator>();
        spawner = spawnerPrefab.GetComponent<PathSpawnerTest>();
        road = creatorPrefab.GetComponent<RoadMeshCreator>();
        
        //creator.EditorData = data.creatorData;
        //spawner.spawnEntities = data.spawnerData;

        creator.TriggerPathUpdate();
        road.TriggerUpdate();
        spawner.pathPrefab = creator;
        spawner.UpdateEntities();

        //SPAWN PLAYER
        GameObject player = Instantiate(surfer.gameObject);
        surfer = player.GetComponent<PathFollowerTest>();
        surfer.pathCreator = creator;
        player.transform.position = level.creator.path.GetPointAtDistance(surfer.startDistanceTravelled, EndOfPathInstruction.Stop);
        player.transform.rotation = level.creator.path.GetRotationAtDistance(surfer.startDistanceTravelled, EndOfPathInstruction.Stop);
        controller.follower = surfer;
        surfer.enabled = false;
    }

    public void GenerateNewLevel(int _id)
    {
        //CREATE PATH

        Level newLevel = ScriptableObject.CreateInstance<Level>();
        newLevel.id = _id;

        //CREATE DEFAULT PATH
        level.creator.EditorData.ResetBezierPath(level.creator.transform.position, false);
        level.creator.bezierPath.Space = PathSpace.xz;
        level.creator.bezierPath.ControlPointMode = BezierPath.ControlMode.Automatic;

        //PATH SIZE IN SIGMENTS
        int numSegments = 8;
        Vector3 prevPos = Vector3.zero;
        //CREATE PATH POINTS WITH SOME RANDOMNESS
        for (int i = 0; i < numSegments; i++)
        {
            Vector3 pos = new Vector3(Random.Range(10f, 100f), 0, Random.Range(-50f, 50f)) + prevPos;
            prevPos = pos;
            level.creator.bezierPath.AddSegmentToEnd(pos);
        }

        //CREATE FINAL PATH POINTS TO ENSURE STRAIGHT LINE AT END
        Vector3 lastPos =  Vector3.right * level.creator.path.length;
        level.creator.bezierPath.AddSegmentToEnd(lastPos);
        level.creator.bezierPath.AddSegmentToEnd(lastPos + Vector3.right * 50f);
        lastPos = Vector3.right * (level.creator.path.length + 100);
        level.creator.bezierPath.AddSegmentToEnd(lastPos);


        //CREATE SPAWN ENTITIES

        level.spawner.ClearData();

        // VALUES THAT CONTROL HOW SPAWN HAPPENS

        float sectionDst = 30f;
        float minDstBetweenObstacles = 10f;
        float minDstBetweenCubes = 5f;
        float minDstBetweenSpawnables = 5.0f;


        int maxObstacles = (int)(level.creator.path.length / sectionDst);
        
        //Start at 2 to give room to increase height
        //PLAYER STARTS AT 20 TO MAKE PATH APPEAR BEHIND
        for (int i = 2; i < maxObstacles - 1; i++)
        {
            //THERE ARE 60 - 20 = 40 UNITS OF SPACE TO ADD THINGS HERE
            if (i == 2)
            {
                //SPAWN HEIGHT BLOCKS AND COINS
                //MAX HEIGHT IS 2 AND HAVE LOWER CHANCE
                int randHeight = Random.Range(2, 6);
                int randCoins = Random.Range(2, 10);
                int randHeight2 = Random.Range(0, randHeight / 2);

                randHeight -= randHeight2;

                int maxSpawnables = (int)(40f / minDstBetweenSpawnables);
                //-1 To Avoid Spawn Inside Player
                for (int j = maxSpawnables - 1; j >= 0; j--)
                {
                    //Spawn Height Blocks First
                    float spawnDst = 60f - (j * minDstBetweenSpawnables);
                    int spawnColumn = Random.Range(-2, 2);
                    float spawnOffset = spawnColumn;

                    if (Random.value < 0.5)
                    {
                        if (randHeight > 0)
                        {
                            level.spawner.AddSpawnEntity(EEntityType.CUBE, level.creator.path.GetPointAtDistance(spawnDst), 0, spawnDst, 1, spawnOffset);
                            randHeight--;
                            continue;
                        }
                        else
                        {
                            if (randHeight2 > 0)
                            {
                                level.spawner.AddSpawnEntity(EEntityType.CUBE, level.creator.path.GetPointAtDistance(spawnDst), 0, spawnDst, 2, spawnOffset);
                                randHeight2--;
                                continue;
                            }
                            else
                            {
                                if (randCoins > 0)
                                {
                                    level.spawner.AddSpawnEntity(EEntityType.COIN, level.creator.path.GetPointAtDistance(spawnDst), 0, spawnDst, 2, spawnOffset);
                                    randCoins--;
                                    continue;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (randHeight2 > 0)
                        {
                            level.spawner.AddSpawnEntity(EEntityType.CUBE, level.creator.path.GetPointAtDistance(spawnDst), 0, spawnDst, 2, spawnOffset);
                            randHeight2--;
                            continue;
                        }
                        else
                        {
                            if (randHeight > 0)
                            {
                                level.spawner.AddSpawnEntity(EEntityType.CUBE, level.creator.path.GetPointAtDistance(spawnDst), 0, spawnDst, 1, spawnOffset);
                                randHeight--;
                                continue;
                            }
                            else
                            {
                                if (randCoins > 0)
                                {
                                    level.spawner.AddSpawnEntity(EEntityType.COIN, level.creator.path.GetPointAtDistance(spawnDst), 0, spawnDst, 2, spawnOffset);
                                    randCoins--;
                                    continue;
                                }
                            }
                        }
                    }


                }
                continue;
            }

            //CHANCE TO SPAWN OBSTACLE = 50%
            //CHANCE OF SPAWN NO OBSTACLES = .5 epx (maxObstacles - 2 ) = VERY LOW
            if (Random.value >= 0.5)
            {
                //SELECT HOW MANY IN SECTION AND WHAT OBSTACLES TO SPAWN
                int sectionObstacleAmount = Random.Range(2, 4);
                int[] obstacleIDs = new int[sectionObstacleAmount];
                int[] minRequiredHeight = new int[sectionObstacleAmount];
                for (int k = 0; k < sectionObstacleAmount; k++)
                {
                    obstacleIDs[k] = Random.Range(0, PrefabsHolder.instance.obstaclePrefabs.Count - 1);

                    if (PrefabsHolder.instance.obstaclePrefabs[obstacleIDs[k]].GetComponentInChildren<WallObstacle>())
                    {

                        minRequiredHeight[k] = PrefabsHolder.instance.obstaclePrefabs[obstacleIDs[k]].GetComponentInChildren<WallObstacle>().GetLowestPoint();
                    }
                    if (PrefabsHolder.instance.obstaclePrefabs[obstacleIDs[k]].GetComponentInChildren<PitObstacle>())
                    {

                        minRequiredHeight[k] = PrefabsHolder.instance.obstaclePrefabs[obstacleIDs[k]].GetComponentInChildren<PitObstacle>().GetLowestCollumnOcupy();
                    }

                }

                int maxSpawnables = (int)(sectionDst / minDstBetweenObstacles);
                int currentObstacle = 0;
                //SPAWN OBSTACLES
                for (int l = 0; l < maxSpawnables; l++)
                {
                    if (currentObstacle < sectionObstacleAmount)
                    {
                        float spawnDst = i * sectionDst - (l * minDstBetweenObstacles);
                        int spawnColumn = Random.Range(-2, 2);
                        float spawnOffset = spawnColumn;

                        //IF OBSTACLE HAS A MIN HEIGHT, SPAWN BLOCKS TO HELP PLAYER
                        //PLAYER MUST ALWAYS HAVE A CHANCE TO WIN, LOSE ONLY IF MISSPLAY
                        if (minRequiredHeight[currentObstacle] > 0)
                        {
                            level.spawner.AddSpawnEntity(EEntityType.CUBE, level.creator.path.GetPointAtDistance(spawnDst - minDstBetweenCubes), 0, spawnDst - minDstBetweenCubes, minRequiredHeight[currentObstacle], spawnOffset);
                            level.spawner.AddSpawnEntity(EEntityType.OBSTACLE, level.creator.path.GetPointAtDistance(spawnDst), obstacleIDs[currentObstacle], spawnDst);
                            currentObstacle++;
                        }
                        else
                        {
                            level.spawner.AddSpawnEntity(EEntityType.OBSTACLE, level.creator.path.GetPointAtDistance(spawnDst), obstacleIDs[currentObstacle], spawnDst);
                            currentObstacle++;
                        }
                    }
                }
            }
            else
            {
                //SPAWN COINS
                int sectionCoinsAmount = Random.Range(10, 30);
                int maxSpawnables = (int)(sectionDst / minDstBetweenSpawnables);
                for (int l = 0; l < maxSpawnables; l++)
                {
                    float spawnDst = i * sectionDst - (l * minDstBetweenSpawnables);
                    int spawnColumn = Random.Range(-2, 2);
                    float spawnOffset = spawnColumn;

                    if (Random.value <= 0.1)
                    {
                        //SPAWN HEIGHT CUBE
                        level.spawner.AddSpawnEntity(EEntityType.CUBE, level.creator.path.GetPointAtDistance(spawnDst), 0, spawnDst, 1, spawnOffset);

                    }
                    else
                    {
                        level.spawner.AddSpawnEntity(EEntityType.COIN, level.creator.path.GetPointAtDistance(spawnDst), 0, spawnDst, 2, spawnOffset);

                    }
                }

                

            }

        }
        //SPAWN FINAL SECTION 
        level.spawner.AddSpawnEntity(EEntityType.OBSTACLE, level.creator.path.GetPointAtDistance(level.creator.path.length - 40f, EndOfPathInstruction.Stop), 6, level.creator.path.length - 40f);


        //ERRORS WITH SERIALIZATION
        //newLevel.creator = level.creator;
        //newLevel.spawner = level.spawner;
        ////SaveLoad.SaveData(newLevel);
        ///

        PlayerPrefs.SetInt("currentLevel", PlayerPrefs.GetInt("currentLevel", 0) + 1);
        PlayerPrefs.SetInt("generatedLevels", _id);

    }



}
