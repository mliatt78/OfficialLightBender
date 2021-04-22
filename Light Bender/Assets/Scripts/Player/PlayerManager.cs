
using System;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
     public static PhotonView Phv;
     public static GameObject localPlayerInstance;

     public static List<PlayerController> players = new List<PlayerController>();
     public static int[] scores = {0,0};
     
     

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



}
