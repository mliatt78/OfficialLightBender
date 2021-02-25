
using System.IO;
using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
     PhotonView Phv;

     GameObject controller;

    void Awake()
    {
        Phv = GetComponent<PhotonView>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (Phv.IsMine)
        {
            CreateController();
        }
    }

    void CreateController() // gestion des mouvements du joueur
    { 
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"),spawnpoint.position,spawnpoint.rotation,0,new object[] { Phv.ViewID });
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }
    
    
    
}
