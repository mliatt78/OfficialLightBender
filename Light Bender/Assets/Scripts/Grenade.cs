using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Grenade : Item
{

    public float delay = 8f;
    
    private bool hasExploded;

     PhotonView Pv;

     public GameObject explosion;

     public float blastradius;

     public float explosionforce = 100f;
     
     public float throwforce = 40f;

     public GameObject grenadeprefab;


     private GameObject grenadethown;

     public int numberofgrenades;
    
    // Start is called before the first frame update
    

    void Awake() => Pv = GetComponent<PhotonView>();

    // Update is called once per frame
    
    public override void Use()
    {
        
       // Debug.Log("I use the grenade ");
        StartCoroutine(waitexplosion(delay));
    }

    IEnumerator waitexplosion(float delays)
    {
        if (numberofgrenades > 0)
        {
            numberofgrenades -= 1;
            Debug.Log("I use the grenade");
            ThrowGrenade();
            yield return new WaitForSeconds (delays);
            if (!hasExploded)
            {
                Debug.Log("Begin Explosion");
                Explode();
                hasExploded = true;
            }
        }
    }
    
    void Explode()
    {
        Instantiate(explosion, grenadethown.transform.position, grenadethown.transform.rotation);
        Pv.RPC("RPC_Explode",RpcTarget.All);
        Debug.Log("BOOM !!!");
    }
    [PunRPC]
    void RPC_Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(grenadethown.transform.position, blastradius);

        foreach (Collider nearbyobject in colliders)
        {
            Rigidbody rb = nearbyobject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionforce,grenadethown.transform.position,blastradius);
                IDamageable damageableOfCollider = nearbyobject.gameObject.GetComponent<IDamageable>();
                damageableOfCollider?.TakeDamage(((GunInfo) iteminfo).damage);
            }
        }
        Destroy(gameObject);
    }

    void ThrowGrenade()
    {
        GameObject grenade = Instantiate(grenadeprefab, transform.position, transform.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        grenadethown = grenade;
        rb.AddForce(transform.forward * throwforce,ForceMode.VelocityChange);
    }
    

    
}
