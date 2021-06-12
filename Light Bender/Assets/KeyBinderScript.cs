
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using UnityEngine;

public class KeyBinderScript : MonoBehaviour
{
    public Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>(); 

    public ButtonManagerBasic up, left, down, right, jump;

    private GameObject currentkey;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        keys.Add("Up", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Up","W")));
        keys.Add("Down", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Down","S")));
        keys.Add("Left", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Left","A")));
        keys.Add("Right", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Right","D")));
        keys.Add("Jump", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Jump","Space")));

        up.buttonText = keys["Up"].ToString();
        down.buttonText = keys["Down"].ToString();
        left.buttonText = keys["Left"].ToString();
        right.buttonText = keys["Right"].ToString();
        jump.buttonText = keys["Jump"].ToString();
        
        up.UpdateUI();
        down.UpdateUI();
        left.UpdateUI();
        right.UpdateUI();
        jump.UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
      
    }
    
    void OnGUI()
    {
        if (currentkey != null)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                keys[currentkey.name] = e.keyCode;

                ButtonManagerBasic buttonManager = currentkey.GetComponent<ButtonManagerBasic>();
                buttonManager.buttonText = e.keyCode.ToString();
                buttonManager.UpdateUI();
                currentkey = null;
               
            }
        }
    }

    public void Changekey(GameObject clicked)
    {
        currentkey = clicked;
    }

    public void Savekeys()
    {
        Debug.Log(keys.Count);
        foreach (var key in keys)
        {
            //Debug.Log(key.Key);
            PlayerPrefs.SetString(key.Key,key.Value.ToString());
        }
        PlayerPrefs.Save();
    }
}
