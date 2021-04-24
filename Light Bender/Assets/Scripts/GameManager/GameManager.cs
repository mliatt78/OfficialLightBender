using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject redPlayerPrefab;
    public GameObject bluePlayerPrefab;
    
    public GameObject RedBot;
    public GameObject BlueBot;

    int RS = 0;
    int BS = 0;
    
    public static GameManager instance;

    
  private void Start()

    {
        //check that we dont have a local instance before we instantiate the prefab
        if (PlayerManager.localPlayerInstance == null)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                int redbots = 5;
                int bluebots = 5;
                foreach (var players in PhotonNetwork.PlayerList)
                {
                    if ((int) players.CustomProperties["Team"] == 0)
                    {
                        bluebots--;
                    }
                    else
                    {
                        redbots--;
                    }
                }

                for (int i = bluebots; i > 0; i--)
                {
                    //get a spawn for the correct team
                    Transform spawn = SpawnManager.instance.blueTeamSpawns[BS].transform;
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", BlueBot.name), spawn.position, spawn.rotation);
                    BS++;
                }
             
                for (int i = redbots; i > 0; i--)
                {
                    //get a spawn for the correct team
                    Transform spawn = SpawnManager.instance.redTeamSpawns[RS].transform;
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", RedBot.name), spawn.position, spawn.rotation);
                    RS++;
                }
            
            }
            
            //instantiate the correct player based on the team
            int team = (int) PhotonNetwork.LocalPlayer.CustomProperties["Team"];
            Debug.Log($"Team number {team} is being instantiated");
            //instantiate the blue player if team is 0 and red if it is not
            if (team == 0)
            {
                //get a spawn for the correct team
                Transform spawn = SpawnManager.instance.blueTeamSpawns[BS].transform;
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", bluePlayerPrefab.name), spawn.position, spawn.rotation);
                BS++;
            }
            else
            {
                //now for the red team
                Transform spawn = SpawnManager.instance.redTeamSpawns[RS].transform;
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", redPlayerPrefab.name), spawn.position, spawn.rotation);
                Debug.Log("RED");
                RS++;
            }
        }

    }
    private void Awake()
    {
        instance = this;
    }
}
