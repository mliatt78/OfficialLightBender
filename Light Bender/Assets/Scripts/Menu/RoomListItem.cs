using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{

    [SerializeField] TMP_Text text;
    public RoomInfo info;
    
    public void Setup(RoomInfo _info ) // le nom de la room
    {
        info = _info;
        text.text = info.Name;
        Debug.Log(info.Name);
        Debug.Log(text.text);
        Debug.Log(_info);
    }

    public void OnClick() // quand tu cliques tu rejoins la room
    {
        Launcher.Instance.JoinRoom(info);
    }
}
