using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    void Awake()
    {
        if (Instance) // regarde si un autre roommanager existe
        {
            Destroy(Instance); // il ne peut y en avoir qu'une ici
            Debug.Log("hello");
           // return;
        }
       
        DontDestroyOnLoad(gameObject); // une seule room manager, donc on destroy pas
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
       
        if (scene.buildIndex == 1 || scene.buildIndex == 2) // on check si la scene est la deuxieme cad l'entree en jeu
        {
            Debug.Log("onSceneLoaded");
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero,
                Quaternion.identity);
        }
    }
}
