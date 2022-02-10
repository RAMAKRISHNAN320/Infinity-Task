using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

public class PathSpawnerTest : PathSpawner
{

    public EEntityType selectedType;

    [SerializeField, HideInInspector]
    List<GameObject> entities = new List<GameObject>();
    GameObject holder;
    //CLEARS ALL OBJECTS ON THE PATH AND SPAWN ALL SAVED ENTITIES
    public virtual void UpdateEntities()
    {
        foreach (GameObject item in entities)
        {
            DestroyImmediate(item);
        }
        entities.Clear();
        if (!holder)
        {
            holder = new GameObject("Spawner Entities");
        }

        foreach (SpawnEntity entity in spawnEntities)
        {

            GameObject obj = Instantiate(entity.prefab, holder.transform);
            obj.transform.position = entity.position;
            obj.transform.rotation = pathPrefab.path.GetRotationAtDistance(entity.dst);
            obj.transform.position = entity.position + obj.transform.up * entity.offset;

            if (obj.GetComponentInChildren<Obstacle>())
            {
                obj.GetComponentInChildren<Obstacle>().entity = entity;
            }

            if (obj.GetComponentInChildren<HeightBlock>())
            {
                obj.GetComponentInChildren<HeightBlock>().height = entity.height;
                for (int i = 1; i < entity.height; i++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    DestroyImmediate(cube.GetComponent<BoxCollider>());
                    cube.transform.SetParent(obj.GetComponentInChildren<HeightBlock>().transform);
                    cube.transform.localPosition = Vector3.down * i;
                    cube.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    cube.GetComponent<MeshRenderer>().sharedMaterial = obj.GetComponentInChildren<MeshRenderer>().sharedMaterial; 
                }
                Vector3 newCenter = Vector3.zero;
                Vector3 newHeight = Vector3.one;
                newCenter.y = -(entity.height / 2 - .5f);
                newHeight.y = entity.height;
                obj.GetComponentInChildren<HeightBlock>().gameObject.GetComponent<BoxCollider>().center = newCenter;
                obj.GetComponentInChildren<HeightBlock>().gameObject.GetComponent<BoxCollider>().size = newHeight;
            }
         

            entities.Add(obj);
        }
    }

    //SAVE ENTITIES
    public void AddSpawnEntity(EEntityType _type, Vector3 _position, int _id, float _dst)
    {
        SpawnEntity entity = new SpawnEntity(_type, _position, _id, _dst);

        spawnEntities.Add(entity);
    }
    public void AddSpawnEntity(EEntityType _type, Vector3 _position, int _id, float _dst, int _height)
    {
        SpawnEntity entity = new SpawnEntity(_type, _position, _id, _dst);
        entity.height = _height;
        spawnEntities.Add(entity);
    }
    public void AddSpawnEntity(EEntityType _type, Vector3 _position, int _id, float _dst, int _height, float _offset)
    {
        SpawnEntity entity = new SpawnEntity(_type, _position, _id, _dst, _offset);
        entity.height = _height;
        spawnEntities.Add(entity);
    }

    //GETS CLOSEST ENTITY TO WORLD POSITION
    public int GetClosestEntityToPoint(Vector3 _pos)
    {
        float minDist = 10f;
        int id = -1;
        for (int i = 0; i < spawnEntities.Count; i++)
        {
            if (Vector3.Distance(spawnEntities[i].position, _pos) < minDist)
            {
                minDist = Vector3.Distance(spawnEntities[i].position, _pos);
                id = i;
            }
        }
        return id;
    }

    //REMOVES ENTITY FROM SAVED ENTITIES
    public void RemoveEntity(int _index)
    {
        if (_index >= 0)
        {
            spawnEntities.RemoveAt(_index);
            UpdateEntities();
        }
    }

    //CLEARS ALL OBJECTS AND SAVED ENTITIES
    public void ClearData()
    {
        foreach (GameObject item in entities)
        {
            DestroyImmediate(item);
        }
        entities.Clear();
        spawnEntities.Clear();
    }
}
