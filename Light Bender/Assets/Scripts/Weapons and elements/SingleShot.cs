using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SingleShot : GUN
{
   public float firerate = 15; 
   [SerializeField] Camera cam;
   public LayerMask ignoreLayerMask;
   public Transform gunTransform;

   private float nexttimetofire = 0;

   public  ParticleSystem particleSystem;

   public float impactforce = 60;
   
    PhotonView Phv;
    void Awake()
    {
       Phv = GetComponent<PhotonView>();
       
    }
   public override void Use()
   {
      Phv.RPC("RPC_Shoot",RpcTarget.All);
   }

   //void Shoot()
  // {
     /* Ray rayon = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
      rayon.origin = cam.transform.position;
      if (Time.time >= nexttimetofire)
      {
         particleSystem.Play();
         nexttimetofire = Time.time + 1f/firerate;
         if (Physics.Raycast(rayon, out RaycastHit hit))                //if (Physics.Raycast(rayon, out RaycastHit hit),100f,~ignoreLayerMask)
         {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)iteminfo).damage);
            if (hit.rigidbody != null)
            {
               hit.rigidbody.AddForce(-hit.normal * impactforce);
            }
            Pv.RPC("RPC_Shoot",RpcTarget.All,hit.point,hit.normal);
         }*/
      
           // Phv.RPC("RPC_Shoot",RpcTarget.All);
  // }

   [PunRPC]
   void RPC_Shoot()
   {
      
     /* Ray rayon = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
      rayon.origin = cam.transform.position;*/
     //if (!Phv.IsMine)
      //  return;
     Ray rayon = new Ray(gunTransform.position, gunTransform.forward);
      if (Time.time >= nexttimetofire)
      {
         particleSystem.Play();
         nexttimetofire = Time.time + 1f / firerate;
         if (Physics.Raycast(rayon, out RaycastHit hit, 100f, ~ignoreLayerMask))
        // if (Physics.Raycast(rayon, out RaycastHit hit),100f,~ignoreLayerMask)
         {
           
            Collider[] bimp = Physics.OverlapSphere(hit.point, 0.3f);
            if (bimp.Length != 0)
            {
               GameObject buletimpact = Instantiate(bulletImpactPrefab, hit.point + hit.normal * 0.001f,
                  Quaternion.LookRotation(hit.normal, Vector3.up) * bulletImpactPrefab.transform.rotation);
               Destroy(buletimpact, 2f);
               buletimpact.transform.SetParent(bimp[0].transform);
            }
            // hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo) iteminfo).damage);
            if (hit.rigidbody != null)
            {
               hit.rigidbody.AddForce(-hit.normal * impactforce);
            }
           var enemyhealth = hit.collider.GetComponent<HEALTH>();
          //  Debug.Log(enemyhealth);
           // Debug.Log(hit.collider.name);
            if (enemyhealth)
            {
               enemyhealth.TakeDamage(((GunInfo) iteminfo).damage);
               Debug.Log("hit");
            }
         }
      }
   }
}
