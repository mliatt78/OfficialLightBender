using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using NUnit.Framework.Constraints;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    
    [SerializeField]  TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text RoomNameText;
    [SerializeField]  Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject PlayerListItemPrefab;
    [SerializeField] GameObject StartGamebutton;
    public AudioSource AudioSource;

   
    
    
    public bool isFocused;

    private bool isTeam = false;

    private bool isAllTeam = false;
    private void Awake() => Instance = this;

    private void IsValid()
    {
        Player[] players = PhotonNetwork.PlayerList;
        foreach (var player in players)
        {
            if (player.CustomProperties["ChoseTeam"] is "False")
                return;
        }
        isAllTeam = true;

    }
    // Start is called before the first frame update
    void Start() // tu te connectes au jeu
    {
        
        PlayerPrefs.SetString("ChoseTeam","False");
        PlayerPrefs.Save();
        // if (!PhotonNetwork.IsConnected)
       // {
            Debug.Log("Connecting to Master");
            PhotonNetwork.ConnectUsingSettings();
       // }
    }

    
    public override void OnConnectedToMaster() // se connecter au host
    {
        Debug.Log("Joined to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void ChooseNickName(string input)
    {
        if (String.IsNullOrEmpty(input))
        {
            input = "null";
        }
        PhotonNetwork.NickName = input;
        GameManager.LocalPlayerName = input;
    }

    public void OpenTitleMenu()
    {
        if (!String.IsNullOrEmpty(GameManager.LocalPlayerName) &&
            GameManager.LocalPlayerName != "null")
        {
            MenuManager.Instance.OpenMenu("title");
        }
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("Mainmenu");
        Debug.Log("Joined Lobby");
        // PhotonNetwork.NickName = "Player" + Random.Range(0, 1000).ToString("0000"); // donne un nom random au joueur de 0 a 1000 
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text)) // creation de room
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.Instance.OpenMenu("Loading");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        StartGamebutton.SetActive(PhotonNetwork.IsMasterClient); // switch de master quand le precedent est parti
    }
    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("room");
        RoomNameText.text = PhotonNetwork.CurrentRoom.Name; // donne le nom de la room actuelle avec celle tape precedemment
        
        Player[] players = PhotonNetwork.PlayerList; // liste des joueurs

        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }
        for(int i = 0; i < players.Length; i++)
        {
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(players[i]);
        }
    }
    

    public override void OnCreateRoomFailed(short returnCode,string message)
    {
        errorText.text = "Room Creation Failed" + message;
        MenuManager.Instance.OpenMenu("error");
    }
    
    public void StartLobby()
    {
        Debug.Log("Start Game");
        PhotonNetwork.LoadLevel(1); // index de la scene
    }
    public void LeaveRoom() // leave room
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
        Debug.Log("username" + PhotonNetwork.NickName);
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("Mainmenu");
    }

    public void Update()
    {
        if (isTeam)
        {
            PlayerPrefs.SetString("ChooseTeam","True");
            PlayerPrefs.Save();
        }
        IsValid();
        StartGamebutton.SetActive(isTeam && isAllTeam && PhotonNetwork.IsMasterClient);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject); // clear la liste a chaque fois qu'on update
        }

        for (int i = 0; i < roomList.Count; i++) // quand tu quittes la room on enleve le joueur
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab,roomListContent).GetComponent<RoomListItem>().Setup(roomList[i]);
        }
        StartGamebutton.SetActive(isTeam);
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(PlayerListItemPrefab,playerListContent).GetComponent<PlayerListItem>().Setup(newPlayer); // instancie un  nouveau player
    }
    
    
    public void JoinTeam(int team)
    {

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
        {

            PhotonNetwork.LocalPlayer.CustomProperties["Team"] = team;
        }
        else
        {
            ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable
            {
                {"Team", team}
            };
            PhotonNetwork.SetPlayerCustomProperties(playerProps);
        }
        isTeam = true;
    }
    

    public void PlaySoundButton()
    {
        AudioSource.Play();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        isFocused = hasFocus;
        AudioListener.pause = !isFocused;
    }
}
