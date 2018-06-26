using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using UnityEngine.Networking;

namespace MFramework
{
    public class AISpawner : NetworkBehaviour
    {
        public GameObject spawnPrefab;
        public int numberToSpawn = 15;
        public int timeTillSpawn = 5;
        
        // Use this for initialization
        void Start () {
        
            if (isServer)
		        Invoke("Spawn", timeTillSpawn);
        }

        void Spawn()
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                GameObject spawnedGO = Instantiate(spawnPrefab, transform.position, transform.rotation);
                NetworkServer.Spawn(spawnedGO);
            }
        }
    }
}