using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
   public static SpawnManager instance;
   
   public GameObject[] redTeamSpawns;
   public GameObject[] blueTeamSpawns;

   void Awake()
   {
      instance = this;
      redTeamSpawns = GameObject.FindGameObjectsWithTag("RedS");
      blueTeamSpawns = GameObject.FindGameObjectsWithTag("BlueS");
   }
 /*  public Transform GetSpawnpoint(int team)
   {
      return team == 0 ? GetBlueSpawnpoint() : GetRedSpawnpoint();
   }*/
   public Transform GetRedSpawnpoint()
   {
      return redTeamSpawns[Random.Range(0, redTeamSpawns.Length)].transform;
   }
   public Transform GetBlueSpawnpoint()
   {
      return blueTeamSpawns[Random.Range(0, blueTeamSpawns.Length)].transform;
   }
   public Transform GetTeamSpawn(int teamNumber)
   {
      return teamNumber == 0 ? GetBlueSpawnpoint() : GetRedSpawnpoint();
   }

}
