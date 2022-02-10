using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

[System.Serializable]
public class LevelData 
{
    public int id;
    public PathCreatorData creatorData;
    public List<SpawnEntity> spawnerData;

    public LevelData(Level _level)
    {
        id = _level.id;
        creatorData = _level.creator.EditorData;
        spawnerData = _level.spawner.spawnEntities;
    }
}
