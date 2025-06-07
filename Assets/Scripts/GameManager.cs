using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("Fruit")]
    public int FruitCount = 0;
    public int totalfruits=0;
    public Fruit[] allFruits;
    
    [Header("Player")]
    public Player player;
    public GameObject playerPrefab;
    public Transform respawnPoint;
    public float respawnTime = 3f;
    
    // [Header("Traps")]
    //public GameObject arrowPrefab;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

     void Start()
     { 
         totalfruits=allFruits.Length;
    }

    //public void UpdateRespawnPosition(Transform newRespawnPoint) => respawnPoint = newRespawnPoint;
    
    // public void RespawnPlayer()
    // {
    //     StartCoroutine(RespawnCoroutine());
    // }
    // public void AddFruit()
    // {
    //     FruitCount++;
    // }
    // private IEnumerator RespawnCoroutine()
    // {
    //     yield return new WaitForSeconds(respawnTime);
    //     player = Instantiate(playerPrefab, respawnPoint.position,Quaternion.identity).GetComponent<Player>();
    // }
    //
    // public void CreatObject(GameObject newObject, Transform target, float delay = 0)
    // {
    //     StartCoroutine(CreateObjectCoroutine(newObject, target, delay));
    // }
    
    // private IEnumerator CreateObjectCoroutine(GameObject prefab,Transform target,float delay)
    // {
    //     Vector3 pos = target.transform.position;
    //     yield return new WaitForSeconds(delay);
    //     GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
    //  }
}
