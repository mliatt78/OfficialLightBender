using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;
using Random = System.Random;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject redPlayerPrefab;
    public GameObject bluePlayerPrefab;

    public GameObject RedBot;
    public GameObject BlueBot;

    int RS;
    int BS;

    public static GameManager instance;

    public static Random rand = new Random();
    [SerializeField] GameObject map;


    [SerializeField] int redbots;
    [SerializeField] int bluebots;

    public static int BlueLayer;
    public static int RedLayer;

    public bool isFocused;

    public static List<ChatMessage> chatMessages = new List<ChatMessage>();

    private void Start()
    {
        BlueLayer = LayerMask.NameToLayer("BlueL");
        RedLayer = LayerMask.NameToLayer("RedL");
        //check that we dont have a local instance before we instantiate the prefab
        if (PlayerManager.localPlayerInstance == null)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                int redbotsCount = redbots;
                int bluebotsCount = bluebots;
                foreach (var currentPlayer in PhotonNetwork.PlayerList)
                {
                    if ((int) currentPlayer.CustomProperties["Team"] == 0)
                    {
                        bluebotsCount--;
                    }
                    else
                    {
                        redbotsCount--;
                    }
                }

                for (int i = bluebotsCount; i > 0; i--)
                {
                    //get a spawn for the correct team
                    Transform spawn = SpawnManager.instance.blueTeamSpawns[BS].transform;
                    GameObject bot = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", BlueBot.name), spawn.position, spawn.rotation);
                    BS++;
                }
         
                for (int i = redbotsCount; i > 0; i--)
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
            GameObject player;
            if (team == 0)
            {
                //get a spawn for the correct team
                Transform spawn = SpawnManager.instance.blueTeamSpawns[BS].transform;
                player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", bluePlayerPrefab.name), spawn.position, spawn.rotation);
                BS++;
            }
            else
            {
                //now for the red team
                Transform spawn = SpawnManager.instance.redTeamSpawns[RS].transform;
                player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", redPlayerPrefab.name), spawn.position, spawn.rotation);
                RS++;
            }
        
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.SetTeam(team);
            playerController.isLocal = true;
        
            // SET UP CHAT
            Chat chat = player.AddComponent<Chat>();
            playerController.chat = chat;
            playerController.chat.playerController = playerController;
            
            PlayerManager.players.Add(playerController);
        }

    }
    private void Awake()
    {
        instance = this;
    }

    public void ChangeMaterialOfMap(Material material)
    {
        Renderer[] renderers = map.GetComponentsInChildren<Renderer>();
        // gets components in children is recursive, so it will give us
        // the renderers of the walls, the floors, ...

        foreach (var render in renderers)
        {
            //renderers[i].material = material;
            render.material = material;
        }

        // change all renderers' material
    }
    public void ChangeColorOfMap(Color color)
    {
        var renderers = map.GetComponentsInChildren<Renderer>();
        foreach (var render in renderers)
        {
            render.material.color = color;
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        isFocused = hasFocus;
        // not sure if having a bool indicating if we're focused on the window
        // serves any purpose, but it's there anyway.

        AudioListener.pause = !isFocused;
        // if focused, then no pause, and vice-versa
    }
}

