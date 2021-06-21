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
    private int[] plbt = {0,2,3,4,5,6,7,8,9,10};
 
   
    public void nbplayers(int Players)
    {
       PlayerPrefs.SetInt("NbPlayers",(byte) plbt[Players]);
       PlayerPrefs.Save();
      // NbPlayers = (byte) plbt[Players];
    }

    public void nbbots(int bots)
    {
        PlayerPrefs.SetInt("Nbbots",plbt[bots]);
        PlayerPrefs.Save();
        Debug.Log("Nbbotes in SettingsForPlay : " + PlayerPrefs.GetInt("Nbbots"));
    }
    public void nbmessages(int number)
    {
        PlayerPrefs.SetInt("messages",messages[number]);
        PlayerPrefs.Save();
        Debug.Log("");
    }

    public void activecapturezones(bool ch)
    {
        PlayerPrefs.SetString("Zones",ch.ToString());
        PlayerPrefs.Save();
        Debug.Log("");
     
    }

    public void jump(bool ch)
    {
        PlayerPrefs.SetString("JumpActive",ch.ToString());
        PlayerPrefs.Save();
        Debug.Log("");
       
    }

    public void zones(bool ch)
    {
        PlayerPrefs.SetString("IsZones", ch.ToString());
        PlayerPrefs.Save();
    }
    
    
}

    
    
