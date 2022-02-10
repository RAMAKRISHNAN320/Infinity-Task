using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabsHolder : MonoBehaviour
{

    public static PrefabsHolder instance;
    [SerializeField]
    public List<GameObject> obstaclePrefabs = new List<GameObject>();
    [SerializeField]
    public GameObject coinPrefabs;
    [SerializeField]
    public GameObject heightBlock;

    public void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
