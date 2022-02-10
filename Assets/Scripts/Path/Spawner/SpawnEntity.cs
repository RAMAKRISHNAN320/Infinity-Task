using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public enum EEntityType{
    OBSTACLE, CUBE, COIN
}
 [System.Serializable]
public class SpawnEntity
{
    public EEntityType type;
    public float offset = 0;
    public Vector3 position;
    public GameObject prefab;
    public float dst;
    public int height;
   


    public SpawnEntity(EEntityType _type, Vector3 _position,int _id, float _dst)
    {
        type = _type;
        position = _position;
        dst = _dst;
        switch (_type)
        {
            case EEntityType.OBSTACLE:
                if (_id >= 0 && _id < PrefabsHolder.instance.obstaclePrefabs.Count)
                {
                    prefab = PrefabsHolder.instance.obstaclePrefabs[_id];
                }
                break;
            case EEntityType.CUBE:
                prefab = PrefabsHolder.instance.heightBlock;
                break;
            case EEntityType.COIN:
                prefab = PrefabsHolder.instance.coinPrefabs;
                break;
            default:
                break;
        }
    }

    public SpawnEntity(EEntityType _type, Vector3 _position, int _id, float _dst, float _offset)
    {
        type = _type;
        position = _position;
        dst = _dst;
        offset = _offset;
        switch (_type)
        {
            case EEntityType.OBSTACLE:
                if (_id >= 0 && _id < PrefabsHolder.instance.obstaclePrefabs.Count)
                {
                    prefab = PrefabsHolder.instance.obstaclePrefabs[_id];
                }
                break;
            case EEntityType.CUBE:
                prefab = PrefabsHolder.instance.heightBlock;
                break;
            case EEntityType.COIN:
                prefab = PrefabsHolder.instance.coinPrefabs;
                break;
            default:
                break;
        }
    }
}
