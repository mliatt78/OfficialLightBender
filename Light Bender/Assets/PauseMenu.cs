using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviourPunCallbacks
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
      //  Time.timeScale = 0;
        GameIsPaused = true;
    }

   /* public void DisconnectPlayer()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(2);
        //StartCoroutine((DisconnectAndLoad()));
    }*/

   /*IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
            yield return null;
        // SceneManager.LoadScene(0);
    }*/
   
   public void LeaveRoom()
   {
       PhotonNetwork.Destroy(RoomManager.Instance.photonView);
       PhotonNetwork.LeaveRoom();
   }
   public override void OnLeftRoom()
   {
       SceneManager.LoadScene(2);
       base.OnLeftRoom();
       

   }

    
}
