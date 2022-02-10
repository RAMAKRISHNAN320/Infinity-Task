using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

[CreateAssetMenu(fileName ="Level")]
public class Level : ScriptableObject
{
    [SerializeField]
    public int id;
    [SerializeField]
    public PathCreator creator;
    [SerializeField]
    public PathSpawnerTest spawner;

}
