using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using UnityEngine;

public class KeyBinderScript : MonoBehaviour
{
    public Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();
    
    public List<ButtonManagerBasic> keyButtons = new List<ButtonManagerBasic>();

    public static List<string> keyNames = new List<string>
    {
        "Up","Down","Left","Right","Jump","Shoot","Reload",
        "Sprint","Crouch","Prone","Dance","Lock","Unlock","Chat"
    };
    
    private GameObject currentkey;
    
    void Awake()
    {
        KeysInitialise();
    }

    void KeysInitialise()
    {
        keys.Add("Up", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Up","W")));
        keys.Add("Down", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Down","S")));
        keys.Add("Left", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Left","A")));
        keys.Add("Right", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Right","D")));
        keys.Add("Jump", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Jump","Space")));
        keys.Add("Shoot", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Shoot","Mouse0")));
        keys.Add("Reload", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Reload","R")));
        keys.Add("Sprint", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Sprint","LeftShift")));
        keys.Add("Crouch", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Crouch","C")));
        keys.Add("Prone", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Prone","V")));
        keys.Add("Dance", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Dance","L")));
        keys.Add("Lock", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Lock","1")));
        keys.Add("Unlock", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Unlock","2")));
        keys.Add("Chat", (KeyCode) System.Enum.Parse(typeof(KeyCode),PlayerPrefs.GetString("Chat","T")));

        // there is a copy of keys in GameManager, so if you add something here, 
        // you have to add it manually in GameManager also
        
        PrintKeyCodes();
    }

    void PrintKeyCodes()
    {
        for (int i = 0; i < keyButtons.Count && i < keyNames.Count; i++)
        {
            //Debug.Log("keyButtons[i]: "+keyButtons[i]);
            //Debug.Log("keyNames[i]: "+keyNames[i]);
            //Debug.Log("keys[keyNames[i]]: "+keys[keyNames[i]]);
            keyButtons[i].buttonText = keys[keyNames[i]].ToString();
            keyButtons[i].UpdateUI();
        }
        // print values of keycodes on buttons
    }
    
    void OnGUI()
    {
        if (currentkey != null)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                //Debug.Log("currentkey : " + currentkey.name);
                //Debug.Log("e keycode : " + e.keyCode);
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
