
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
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
        //Debug.Log(players.Count);
        for (int i = 0; i < players.Count; i++)
        {
            players[i].blueScoreText.text = scores[0].ToString();
            players[i].redScoreText.text = scores[1].ToString();
            Debug.Log("Score : " + scores[0] + " -- "+scores[1]);
        }
    }

    public static PlayerController GetLocalPlayer()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].IsLocal)
            {
                return players[i];
            }
        }
        
        Debug.LogError("GetLocalPLayer: No Local Player recognized!");
        return null;
    }

}
