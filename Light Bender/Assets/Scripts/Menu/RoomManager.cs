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
            Destroy(gameObject); // il ne peut y en avoir qu'une ici
            return;
        }
        DontDestroyOnLoad(gameObject); // il y en a plus qu'une ici
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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene,LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1) // on check si la scene est la deuxieme cad l'entree en jeu
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero,
                Quaternion.identity);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
