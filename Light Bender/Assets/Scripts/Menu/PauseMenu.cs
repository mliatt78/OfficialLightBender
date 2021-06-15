using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviourPunCallbacks
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public AudioSource audioSource;
    public GameObject KEYS;

    public static bool isleft = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (GameIsPaused)
                Resume();
            else
                Pause();
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        KEYS.SetActive(false);
        //Debug.Log("Resume");
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
      //  Time.timeScale = 0;
        GameIsPaused = true;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void DisconnectPlayer()
    {
       // PhotonNetwork.LeaveRoom();
       // SceneManager.LoadScene(2);
        StartCoroutine((DisconnectAndLoad()));
    }

   IEnumerator DisconnectAndLoad()
   {
        GameIsPaused = false;
        isleft = true;
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;
        SceneManager.LoadScene(2);
   }
   
   /*public void LeaveRoom() // local player leaves
   {
      // PhotonNetwork.Destroy(RoomManager.Instance.photonView);
      PhotonNetwork.Disconnect();
      while(PhotonNetwork.IsConnected)
          Debug.Log("say hi");
      
      PhotonNetwork.LeaveLobby();
   }
   public override void OnLeftRoom()
   {
      // PlayerManager.players.Remove(PlayerManager.GetLocalPlayer());
       SceneManager.LoadScene(2);
       base.OnLeftRoom();
   }*/

   public void PlayButtonSound()
   {
       audioSource.Play();
   }

   public void OpenKeys()
   {
       pauseMenuUI.SetActive(false);
       KEYS.SetActive(true);
   }
}
