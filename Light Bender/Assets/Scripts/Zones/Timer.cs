using Photon.Pun;
using UnityEngine;

public class Timer : MonoBehaviour
{
    bool startTimer = false;
    double increasingTimer;
    double decreasingTimer;
    double startTime;
    [SerializeField] double maxValue = 40;
    ExitGames.Client.Photon.Hashtable CustomeValue;

    void Start()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            CustomeValue = new ExitGames.Client.Photon.Hashtable();
            startTime = PhotonNetwork.Time;
            startTimer = true;
            CustomeValue.Add("StartTime", startTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
        }
        else
        {
            startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
            startTimer = true;
        }
    }

    void Update()
    {
        if (!startTimer) return;
        increasingTimer = PhotonNetwork.Time - startTime;
        // as photonNetwork.Time increases, timerIncrementValue also increases
        decreasingTimer = maxValue - increasingTimer;
        // so this is an increasing timer
        if (decreasingTimer < 0)
        {
            //Timer Completed
            //Do What Ever You What to Do Here
        }
    }
    
    
    // CODE BY AQIB SADIQ
}
