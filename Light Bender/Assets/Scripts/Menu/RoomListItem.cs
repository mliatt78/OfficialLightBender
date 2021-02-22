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
    }

    public void OnClick() // quand tu cliques tu rejoins la room
    {
        Launcher.Instance.JoinRoom(info);
    }
}
