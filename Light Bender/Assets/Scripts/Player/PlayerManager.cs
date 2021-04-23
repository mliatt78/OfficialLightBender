
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
     public static PhotonView Phv;
     public static GameObject localPlayerInstance;
    // GameObject controller;
    
    public static List<PlayerController> players = new List<PlayerController>();
    public static int[] scores = {0,0};
     
    /* int team;*/

    void Awake()
    {
        Phv = GetComponent<PhotonView>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (Phv.IsMine)
        {
            localPlayerInstance = gameObject;
        }
        //now dont destroy!!
        DontDestroyOnLoad(gameObject);
        /* if (Phv.IsMine)
        {
            CreateController();
        }*/
    }
    
    
    public static void UpdateScores()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].blueScoreText.text = scores[0].ToString();
            players[i].redScoreText.text = scores[1].ToString();
        }
    }

    void CreateController() // gestion des mouvements du joueur
    { 
      /*  Transform spawnpoint = SpawnManager.instance.GetSpawnpoint(team);
        Debug.Log($"Team number {team} is being instantiated");
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"),spawnpoint.position,spawnpoint.rotation,0,new object[] { Phv.ViewID });*/
    
    }

   /* public void Die()
    {
        PhotonNetwork.Destroy(controller);
        PlayerController.health = 1;
       
    }*/
   /* public void SetTeam(int Team)
    {
        if (Team == 0 || Team == 1)
        {
            team = Team;
        }
        else
        {
            Debug.Log("PLayerManager: Team number is not valid");
        }
    
    }*/
    
    
    
}
