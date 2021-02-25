using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SingleShot : GUN
{
   
   [SerializeField] Camera cam;

   public  ParticleSystem particleSystem;
   
   

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
      if (Physics.Raycast(rayon, out RaycastHit hit))
      {
        hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)iteminfo).damage);
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
         GameObject buletimpact  =  Instantiate(bulletImpactprefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal,Vector3.up) * bulletImpactprefab.transform.rotation);
         Destroy(buletimpact, 10f);
         buletimpact.transform.SetParent(bimp[0].transform);
      }
   }
}
