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
   
    PhotonView Pv;

    public int nbballes;

    private int nbinit;

    public AudioSource audioSource;

    public void Start()
    {
       nbinit = nbballes;
    }
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
      Pv.RPC("RPC_Shoot",RpcTarget.All);
   }

   IEnumerator Reload()
   {
      yield return new WaitForSeconds(4.5f);
      nbballes = nbinit;
   }

   [PunRPC]
   void RPC_Shoot()
   {
      Ray rayon = new Ray(gunTransform.position, gunTransform.forward);
      if (nbballes <= 0)
         StartCoroutine(Reload());
      else
      {
         if (Time.time >= nexttimetofire)
         {
            nbballes -= 1;
            particleSystem.Play();
            audioSource.Play();
            nexttimetofire = Time.time + 1f / firerate;
            if (Physics.Raycast(rayon, out RaycastHit hit, 100f, ~ignoreLayerMask))
            {
               Collider[] bimp = Physics.OverlapSphere(hit.point, 0.3f);
               if (bimp.Length != 0)
               {
                  GameObject buletimpact = Instantiate(bulletImpactPrefab, hit.point + hit.normal * 0.001f,
                     Quaternion.LookRotation(hit.normal, Vector3.up) * bulletImpactPrefab.transform.rotation);
                  Destroy(buletimpact, 2f);
                  buletimpact.transform.SetParent(bimp[0].transform);
               }
               hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo) iteminfo).damage);
               
               Debug.Log(((GunInfo) iteminfo).damage + " DOMMAGES");
               if (hit.rigidbody != null)
               {
                  hit.rigidbody.AddForce(-hit.normal * impactforce);
               }
            }
         }
      }
   }
}
