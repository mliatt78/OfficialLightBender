
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
    }
    
    
    public static void UpdateScores()
    {
        Debug.Log(players.Count);
        for (int i = 0; i < players.Count; i++)
        {
            players[i].blueScoreText.text = scores[0].ToString();
            players[i].redScoreText.text = scores[1].ToString();
            Debug.Log(scores[0] + " // "+scores[1]);
        }
    }
    
    
    
    
}
