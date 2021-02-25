using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SingleShot : GUN
{
   public float firerate = 15; 
   [SerializeField] Camera cam;

   private float nexttimetofire = 0;

   public new ParticleSystem particleSystem;

   public float impactforce = 60;
   
    PhotonView Pv;
    void Awake()
    {
       Pv = GetComponent<PhotonView>();
       
    }
   public override void Use()
   {
      Shoot();
   }

   void Shoot()
   {
      Ray rayon = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
      rayon.origin = cam.transform.position;
      if (Physics.Raycast(rayon, out RaycastHit hit) && Time.time >= nexttimetofire  )
      {
         nexttimetofire = Time.time + 1f/firerate;
         hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)iteminfo).damage);
        if (hit.rigidbody != null)
        {
           hit.rigidbody.AddForce(-hit.normal * impactforce);
        }
        Pv.RPC("RPC_Shoot",RpcTarget.All,hit.point,hit.normal);
        particleSystem.Play();
      }
   }

   [PunRPC]
   void RPC_Shoot(Vector3 hitPosition,Vector3 hitNormal)
   {
      Collider[] bimp = Physics.OverlapSphere(hitPosition, 0.3f);
      if (bimp.Length != 0)
      {
         GameObject buletimpact  =  Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal,Vector3.up) * bulletImpactPrefab.transform.rotation);
         Destroy(buletimpact, 2f);
         buletimpact.transform.SetParent(bimp[0].transform);
      }
   }
}
