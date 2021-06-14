
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using UnityEngine;
using UnityEngine.WSA;

public class KeyBinderScript : MonoBehaviour
{
    public  Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

    public ButtonManagerBasic up, left, down, right, jump, crouch, prone;
    public List<ButtonManagerBasic> keyButtons = new List<ButtonManagerBasic>();

    public static List<string> keyNames = new List<string>{"Up","Down","Left","Right","Jump","Crouch","Prone"};
    
    private GameObject currentkey;

    
    void Awake()
    {
        (keyButtons,keys) = KeysInitialise(up,left,down,right,jump,crouch,prone);
    }

    (List<ButtonManagerBasic> keyButtons,Dictionary<string,KeyCode> keys) KeysInitialise(
        ButtonManagerBasic up,ButtonManagerBasic left,ButtonManagerBasic down,
        ButtonManagerBasic right,ButtonManagerBasic jump, ButtonManagerBasic crouch, ButtonManagerBasic prone)
    {
        List<ButtonManagerBasic> keyButtons = new List<ButtonManagerBasic>();
        Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();
        
        keyButtons.Add(up);
        keyButtons.Add(left);
        keyButtons.Add(down);
        keyButtons.Add(right);
        keyButtons.Add(jump);
        keyButtons.Add(crouch);
        keyButtons.Add(prone);
        
        keys.Add("Up", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Up","W")));
        keys.Add("Down", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Down","S")));
        keys.Add("Left", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Left","A")));
        keys.Add("Right", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Right","D")));
        keys.Add("Jump", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Jump","Space")));
        keys.Add("Crouch", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Crouch","C")));
        keys.Add("Prone", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Prone","V")));

        // there is a copy of keys in GameManager, so if you add something here, 
        // you have to add it manually in GameManager also
        
        for (int i = 0; i < keyButtons.Count && i < keyNames.Count; i++)
        {
            Debug.Log("i count : "+i);
            Debug.Log("keyButtons[i]: "+ keyButtons[i]);
            Debug.Log("keyNames[i]: "+ keyNames[i]);
            keyButtons[i].buttonText = keys[keyNames[i]].ToString();
            keyButtons[i].UpdateUI();
        }

        return (keyButtons,keys);

        /*
        up.buttonText = keys["Up"].ToString();
        down.buttonText = keys["Down"].ToString();
        left.buttonText = keys["Left"].ToString();
        right.buttonText = keys["Right"].ToString();
        jump.buttonText = keys["Jump"].ToString();
        crouch.buttonText = keys["Crouch"].ToString();
        prone.buttonText = keys["Prone"].ToString();
                
        up.UpdateUI();
        down.UpdateUI();
        left.UpdateUI();
        right.UpdateUI();
        jump.UpdateUI();
        crouch.UpdateUI();
        prone.UpdateUI();
        */
    }

    
    void OnGUI()
    {
        if (currentkey != null)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                Debug.Log("currentkey : " + currentkey.name);
                Debug.Log("e keycode : " + e.keyCode);
                keys[currentkey.name] = e.keyCode;

                ButtonManagerBasic buttonManager = currentkey.GetComponent<ButtonManagerBasic>();
                buttonManager.buttonText = e.keyCode.ToString();
                buttonManager.UpdateUI();
                currentkey = null;
            }
        }
    }
    public void Changekey(GameObject clicked) => currentkey = clicked;

    public void Savekeys()
    {
        //Debug.Log(keys.Count);
       foreach (var key in keys)
        {
            PlayerPrefs.SetString(key.Key,key.Value.ToString());
            GameManager.instance.keys[key.Key] = key.Value;
        }
        PlayerPrefs.Save();
    }
}
