using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// source : https://sharpcoderblog.com/blog/pun-2-adding-room-chat

public class Chat : MonoBehaviour
{
    public PlayerController playerController;

    //private PhotonView PhotonView;

    bool isChatting;
    string chatInput = "";

    [SerializeField] int minHeightOnScreen = 75;
    [SerializeField] int maxLengthMessage = 60;
    [SerializeField] GUIStyle chatBoxStyle;

    public static List<ChatMessage> chatMessages = GameManager.chatMessages;
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T) && !isChatting)
        {
            isChatting = true;
            chatInput = "";
        }

        //Hide messages after timer is expired
        for (int i = 0; i < chatMessages.Count; i++)
        {
            if (chatMessages[i].timer > 0)
            {
                chatMessages[i].timer -= Time.deltaTime;
            }
        }
    }

    void OnGUI()
    {
        if (!isChatting)
        {
            playerController.PlayerOnlyLook = false;
        
            GUI.Label(new Rect(5, Screen.height - minHeightOnScreen, 200, 25), "Press 'T' to chat");
        }
        else
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                isChatting = false;
                if(chatInput.Replace(" ", "") != "")
                {
                    //Send message
                    playerController.SendChatMessage(PhotonNetwork.LocalPlayer.NickName, chatInput);
                }
                chatInput = "";
            
            }

            GUI.SetNextControlName("ChatField");
            GUI.Label(new Rect(5, Screen.height - minHeightOnScreen, 200, 25), "Say:");
            chatInput = GUI.TextField(new Rect(10 + 25, Screen.height - minHeightOnScreen, 400, 22), chatInput, maxLengthMessage, chatBoxStyle);
            chatBoxStyle = GUI.skin.GetStyle("box");
            chatBoxStyle.alignment = TextAnchor.MiddleLeft;
            
            playerController.PlayerOnlyLook = true;

            GUI.FocusControl("ChatField");
        }
    
        //Show messages
        for(int i = 0; i < chatMessages.Count; i++)
        {
            if(chatMessages[i].timer > 0 || isChatting)
            {
                GUI.Label(new Rect(5, Screen.height - (minHeightOnScreen+25) - 25 * i, 500, 25), chatMessages[i].sender + ": " + chatMessages[i].message);
            }
        } 
    }
}


public class ChatMessage
{
    public readonly string sender;
    public readonly string message;
    public float timer;

    public ChatMessage(string sender,string message, float timer = 15.0f)
    {
        this.sender = sender;
        this.message = message;
        this.timer = timer;
    }
}