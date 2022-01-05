using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    public static SpawnManager instance;


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //deactivate all the gameobjects initially
        foreach(Transform spawn in spawnPoints)
        {
            spawn.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public  Transform getSpawnPoint()
    {
        return spawnPoints[Random.Range(0,spawnPoints.Length)];
    }

}
