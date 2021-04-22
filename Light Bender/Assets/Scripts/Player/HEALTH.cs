using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Photon.Pun;
using UnityEngine;

public class HEALTH : MonoBehaviourPunCallbacks, IPunObservable,IDamageable
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
        PlayerController player = GetComponent<PlayerController>();
        player.enabled = false;
        // scores
        int otherTeam = (player.GetTeam() + 1) % 2;
        PlayerManager.scores[otherTeam] += 1;
        // end scores
        Transform spawn = SpawnManager.instance.GetTeamSpawn(team);
        transform.position = spawn.position;
        transform.rotation = spawn.rotation;
        yield return new WaitForSeconds(1);     
        GetComponent<PlayerController>().enabled = true;
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
            Debug.Log("mort");
            if (photonView.IsMine)
            {
                
                Debug.Log("mort2");
               StartCoroutine(Respawn());
            }
        }
    }
}
