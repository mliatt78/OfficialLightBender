using System.Collections;
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
     public GameObject grenadeOwner;
     
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
            GameObject grenade = PhotonNetwork.Instantiate("Grenade_03", transform.position, transform.rotation);
            Rigidbody rb = grenade.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * throwforce,ForceMode.VelocityChange);
            yield return new WaitForSeconds (delays);
            if (!hasExploded)
            {
                Debug.Log("Begin Explosion");
                Instantiate(explosion, grenade.transform.position, grenade.transform.rotation);
                Pv.RPC("RPC_Explode",RpcTarget.All,grenade.name);
                Debug.Log("BOOM !!!");
                hasExploded = true;
            }
        }
    }
    
   
    [PunRPC]
    void RPC_Explode(string name)
    {
        GameObject grenade = GameObject.Find(name);
//        Debug.Log("grenade exists" + grenade is null);
        Collider[] colliders = Physics.OverlapSphere(grenade.transform.position, blastradius);
        foreach (Collider nearbyobject in colliders)
        {
            Rigidbody rb = nearbyobject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionforce,grenade.transform.position,blastradius);
                IDamageable damageableOfCollider = nearbyobject.gameObject.GetComponent<IDamageable>();
                damageableOfCollider?.TakeDamage(((GunInfo) iteminfo).damage, grenadeOwner.gameObject.name);
            }
        }
        // Destroy(gameObject);
    }
    
    

    
}
