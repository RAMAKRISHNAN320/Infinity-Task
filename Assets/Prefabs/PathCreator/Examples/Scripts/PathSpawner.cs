using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

namespace PathCreation.Examples {
    [System.Serializable]
    public class PathSpawner : MonoBehaviour {

        public PathCreator pathPrefab;
        public PathFollower followerPrefab;
        public Transform[] spawnPoints;


        private void Awake()
        {
            pathPrefab = GetComponent<PathCreator>();
        }
        [SerializeField]
        public List<SpawnEntity> spawnEntities = new List<SpawnEntity>();

        void Start () {
            //foreach (Transform t in spawnPoints) {
            //    var path = Instantiate (pathPrefab, t.position, t.rotation);
            //    var follower = Instantiate (followerPrefab);
            //    follower.pathCreator = path;
            //    path.transform.parent = t;
                
            //}
        }
    }

}