using System;
using System.Collections.Generic;
using System.Linq;
using EnemyAI;
using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
     public static PhotonView Phv;
     public static GameObject localPlayerInstance;
    // GameObject controller;
    
    public static List<PlayerController> players = new List<PlayerController>();
    public static List<PlayerController> bluePlayers = new List<PlayerController>();
    public static List<PlayerController> redPlayers = new List<PlayerController>();
    
    public static List<AIController> bots = new List<AIController>();
    public static List<AIController> blueBots = new List<AIController>();
    public static List<AIController> redBots = new List<AIController>();

    void Awake()
    {
        Phv = GetComponent<PhotonView>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // why start ? because we only want the list when all players are here,
        // not when the object is just created.
        // players
        List<GameObject> blueAll, redAll;
        (blueAll, redAll) = SeparateTeams(gameObject.scene.GetRootGameObjects(),GameManager.BlueLayer, GameManager.RedLayer);
        (bluePlayers, blueBots) = SeparateBotsPlayers(blueAll);
        (redPlayers, redBots) = SeparateBotsPlayers(redAll);
        players = bluePlayers.Concat(redPlayers).ToList();
        bots = blueBots.Concat(redBots).ToList();
        
        /*
        Debug.Log("Temp lists :");
        Debug.Log(TempPrintList(blueAll));
        Debug.Log(TempPrintList(redAll));
        Debug.Log("Players List: ");
        Debug.Log(TempPrintListController(bluePlayers));
        Debug.Log(TempPrintListController(redPlayers));
        Debug.Log("Bots List: ");
        Debug.Log(TempPrintListController(blueBots));
        Debug.Log(TempPrintListController(redBots));
        */        
        
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
            players[i].blueScoreText.text = GameManager.scores[0].ToString();
            players[i].redScoreText.text = GameManager.scores[1].ToString();
            //Debug.Log("Score : " + GameManager.scores[0] + " -- " + GameManager.scores[1]);
        }
    }

    public static PlayerController GetLocalPlayer()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].isLocal)
            {
                return players[i];
            }
        }
        
        Debug.LogError("GetLocalPLayer: No Local Player recognized!");
        return null;
    }
    
    private (List<GameObject>,List<GameObject>) SeparateTeams(GameObject[] rootObjects, int layer1, int layer2)
    {
        List<GameObject> layer1Objects = new List<GameObject>();
        List<GameObject> layer2Objects = new List<GameObject>();

        if (rootObjects != null)
        {
            for (int i = 0; i < rootObjects.Length; i++)
            {
                var currGameObject = rootObjects[i];
                if (currGameObject.gameObject.layer == layer1)
                {
                    layer1Objects.Add(currGameObject.gameObject);
                }
                if (currGameObject.gameObject.layer == layer2)
                {
                    layer2Objects.Add(currGameObject.gameObject);
                }
            }
        }

        return (layer1Objects,layer2Objects);
    }

    public static (List<PlayerController>, List<AIController>) SeparateBotsPlayers(List<GameObject> gameObjects)
    {
        List<PlayerController> listPlayers = new List<PlayerController>();
        List<AIController> listBots = new List<AIController>();
        for (int i = 0; i < gameObjects.Count; i++)
        {
            PlayerController playerController = gameObjects[i].GetComponent<PlayerController>();
            if (playerController != null)
            {
                listPlayers.Add(playerController);
            }
            else
            {
                AIController aiController = gameObjects[i].GetComponent<AIController>();
                if (aiController != null)
                {
                    listBots.Add(aiController);
                }
                else
                {
                    throw new ArgumentException("gameObject not recognized when separating players and bots");
                }
            }
        }

        return (listPlayers, listBots);
    }
    
    private static string TempPrintList(List<GameObject> list)
    {
        string ret = "";
        for (int i = 0; i < list.Count; i++)
        {
            ret += list[i].name + " ";
        }
        return ret;
    }
    private static string TempPrintListController<T>(List<T> list)
    {
        string ret = "";
        for (int i = 0; i < list.Count; i++)
        {
            ret += (list[i] as GameObject)?.name + " ";
        }
        return ret;
    }
}
