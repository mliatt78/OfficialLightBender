using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PhotonView Phv;

    void Awake()
    {
        Phv = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (Phv.IsMine)
        {
            CreateConstroller();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateConstroller() // gestion des mouvements du joueur
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotobPrefabs", "PlayerController"),Vector3.zero,Quaternion.identity);
    }
}
