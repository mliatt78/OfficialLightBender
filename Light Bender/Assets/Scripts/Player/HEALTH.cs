using System.Collections;
using Photon.Pun;
using UnityEngine;

public class HEALTH : MonoBehaviourPunCallbacks,IPunObservable
{
    public float health = 100;
    Renderer[] visuals;
    int team = 0;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // recupere la health
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            health = (float) stream.ReceiveNext();
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    IEnumerator Respawn()
    {
        SetRenderers(false);
        health = 100;
        GetComponent<PlayerController>().enabled = false;
        Transform spawn = SpawnManager.instance.GetTeamSpawn(team);
        transform.position = spawn.position;
        transform.rotation = spawn.rotation;
        GetComponent<PlayerController>().enabled = true;
        yield return new WaitForSeconds(1);        
        SetRenderers(true);
    }

    void SetRenderers(bool state)
    {
        foreach (var renderer in visuals)
        {
            renderer.enabled = state;
        }
    }

    void Start()
    {
        visuals = GetComponentsInChildren<Renderer>();
        team = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
    }
    void Update()
    {
        if (health <= 0)
        {
            if (photonView.IsMine)
            {
                StartCoroutine(Respawn());
            }
        }
    }
}