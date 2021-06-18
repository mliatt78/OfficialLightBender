using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SettingsForPlay : MonoBehaviour
{
    public static byte NbPlayers = 10;
    public  static int NbBots = 3;
    public  static int NbMessages = 10;
    public static bool Activezones = true;
    public static bool Jump = true;
    public  static SettingsForPlay instance;

    private int[] messages = {1,2,3,4,5,6,7,8};
    private int[] plbt = {2,3,4,5,6,7,8,9,10};
 
    void Awake ()
    {        
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }        
    }
    
    public void nbplayers(int Players)
    {
        NbPlayers = (byte) plbt[Players];
    }

    public void nbbots(int bots)
    {
        NbBots = plbt[bots];
        Debug.Log("Number of bots : " + NbBots );
    }
    public void nbmessages(int number)
    {
        NbMessages = messages[number];
       // Debug.Log("Number of messages : " + NbMessages );
    }

    public void activecapturezones(bool ch)
    {
        Activezones = ch;
    }

    public void jump(bool ch)
    {
        Jump = ch;
    }
    
    
}

    
    
