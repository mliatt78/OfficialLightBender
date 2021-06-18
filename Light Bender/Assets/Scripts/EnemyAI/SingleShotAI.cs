using System.Collections;
using EnemyAI;
using Photon.Pun;
using UnityEngine;

public class SingleShotAI : GUN
{
   public AIController AiOwner;
   
   public float firerate;
   public LayerMask ignoreLayerMask;
   public Transform gunTransform;

   private float nexttimetofire = 0;

   public ParticleSystem particleSystem;

   public float impactforce = 60;
   
    PhotonView Pv;

    public int nbballes;
    public float secondsToReload;
    public float BotWaitReloadTime;

    private int nbinit;

    
    void Awake()
    {
       Pv = GetComponent<PhotonView>();
       GunInfo gunProperties = (GunInfo) iteminfo;
       nbinit = gunProperties.nbinit;
       nbballes = nbinit;
       secondsToReload = gunProperties.secondsToReload;

       BotWaitReloadTime = 2; 
       // bots have to wait the time to reload the gun + 2 seconds to reload
    }
   public override void Use()
   {
      Shoot();
   }

   void Shoot()
   {
      Pv.RPC("RPC_Shoot",RpcTarget.All);
   }

   IEnumerator Reload(float seconds_To_Reload)
   {
      yield return new WaitForSeconds(seconds_To_Reload);
      nbballes = nbinit;
   }

   [PunRPC]
   void RPC_Shoot()
   {
      Ray rayon = new Ray(gunTransform.position, gunTransform.forward);
      if (nbballes <= 0)
         StartCoroutine(Reload(secondsToReload+BotWaitReloadTime));
      else
      {
         if (Time.time >= nexttimetofire)
         {
            nbballes -= 1;
            particleSystem.Play();
            nexttimetofire = Time.time + 1f / firerate;
            if (Physics.Raycast(rayon, out RaycastHit hit, 100f, ~ignoreLayerMask))
            {
               Collider[] bimp = Physics.OverlapSphere(hit.point, 0.3f);
               if (bimp.Length != 0)
               {
                  GameObject bulletImpact = Instantiate(bulletImpactPrefab, hit.point + hit.normal * 0.001f,
                     Quaternion.LookRotation(hit.normal, Vector3.up) * bulletImpactPrefab.transform.rotation);
                  Destroy(bulletImpact, 2f);
                  bulletImpact.transform.SetParent(bimp[0].transform);
               }
               hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo) iteminfo).damage, AiOwner.gameObject.name);
               /*if (hit.collider.gameObject.GetComponent<PlayerController>() != null)
               {
                  hit.collider.gameObject.GetComponent<PlayerController>().lastShooter = AiOwner.gameObject;
               }
               else if (hit.collider.gameObject.GetComponent<AIController>() != null)
               {
                  hit.collider.gameObject.GetComponent<AIController>().lastShooter = AiOwner.gameObject;
               }*/
               
               //Debug.Log(((GunInfo) iteminfo).damage + " DAMAGE");
               if (hit.rigidbody != null)
               {
                  hit.rigidbody.AddForce(-hit.normal * impactforce);
               }
            }
         }
      }
   }
}
