using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = System.Random;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject redPlayerPrefab;
    public GameObject bluePlayerPrefab;

    public GameObject RedBot;
    public GameObject BlueBot;


    public static GameManager instance;

    public static Random rand = new Random();
    [SerializeField] GameObject map;

    int RS; 
    int BS;

    public int redbots;
    public int bluebots;

    public static int BlueLayer;
    public static int RedLayer;

    public static bool isFocused = true;
    
    public int currentweapon;

    public bool IsLobby = false;
    public GameObject settingsbutton;
    public int[] scores = {0,0};


    public static List<ChatMessage> chatMessages = new List<ChatMessage>();
    public  Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>(); 
    

    public void ApplySettings()
    {
        bluebots = SettingsForPlay.NbBots;
        redbots = SettingsForPlay.NbBots;
        PlayerController.CanJump = SettingsForPlay.Jump;
        PlayerController.nbmessages = SettingsForPlay.NbMessages;
      //  RoomOptions options = new RoomOptions();
      //  options.MaxPlayers = SettingsForPlay.NbPlayers;
    }

    private void Start()
    {
        // ApplySettings();
        Debug.Log("start in GameManager");
        BlueLayer = LayerMask.NameToLayer("BlueL");
        RedLayer = LayerMask.NameToLayer("RedL");
        //check that we dont have a local instance before we instantiate the prefab
        if (PlayerManager.localPlayerInstance == null)
        {
            Debug.Log("localPlayerInstance is null");
//            Debug.Log("PhotonNetwork.IsMasterClient  :  " + PhotonNetwork.IsMasterClient);
            if (PhotonNetwork.IsMasterClient)
            {
                int redbotsCount = redbots;
                int bluebotsCount = bluebots;
                foreach (var currentPlayer in PhotonNetwork.PlayerList)
                {
                    if ((int) currentPlayer.CustomProperties["Team"] == 0)
                        bluebotsCount--;
                    else
                        redbotsCount--;
                }
                
                for (int i = bluebotsCount; i > 0; i--)
                {
                    //get a spawn for the correct team
                    Transform spawn = SpawnManager.instance.blueTeamSpawns[BS].transform;
                    GameObject bot = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", BlueBot.name),
                        spawn.position, spawn.rotation);
                    BS++;
                }

                for (int i = redbotsCount; i > 0; i--)
                {
                    //get a spawn for the correct team
                    Transform spawn = SpawnManager.instance.redTeamSpawns[RS].transform;
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", RedBot.name), spawn.position,
                        spawn.rotation);
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
                player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", bluePlayerPrefab.name), spawn.position,
                    spawn.rotation);
                BS++;
               // Debug.Log("Player is Istanciate");
            }
            else
            {
                //now for the red team
                Transform spawn = SpawnManager.instance.redTeamSpawns[RS].transform;
                player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", redPlayerPrefab.name), spawn.position,
                    spawn.rotation);
              //  Debug.Log("Player is Istanciate");
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
        else
            Destroy(gameObject);
    }

    void Awake()
    {
       //Debug.Log("Awake");
        if (instance == null)
        {
            instance = this;
            keys.Add("Up", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up", "W")));
            keys.Add("Down", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down", "S")));
            keys.Add("Left", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left", "A")));
            keys.Add("Right", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right", "D")));
            keys.Add("Jump", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Jump", "Space")));
            keys.Add("Shoot", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Shoot", "Mouse0")));
            keys.Add("Reload", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Reload", "R")));
            keys.Add("Sprint",
                (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Sprint", "LeftShift")));
            keys.Add("Crouch", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Crouch", "C")));
            keys.Add("Prone", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Prone", "V")));
            keys.Add("Dance", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Dance", "L")));
            keys.Add("Lock", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Lock", "1")));
            keys.Add("Unlock", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Unlock", "2")));
            keys.Add("Chat", (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Chat", "T")));
        }
        else if (instance != this)
            Destroy(gameObject);
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