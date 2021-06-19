using System.Collections;
using Photon.Pun;
using UnityEngine;

public class SingleShot : GUN
{
   public PlayerController PlayerOwner;
   
   public float firerate = 15;
   public LayerMask ignoreLayerMask;
   public Transform gunTransform;

   private float nexttimetofire = 0;

   public ParticleSystem particleSystem;

   public float impactforce = 60;
   
    PhotonView Phv;
    
    [SerializeField]  ProgressBarPro munitionsSlider;

    public int nbinit { get; set; }
    public float secondsToReload { get; set; }


    public int nbballes { get; set; }
   
    public AudioSource audioSource;

    
    void Awake()
    {
       Phv = GetComponent<PhotonView>();
       GunInfo weaponProperties = (GunInfo) iteminfo;
       nbinit = weaponProperties.nbinit;
       nbballes = nbinit;
       secondsToReload = weaponProperties.secondsToReload;

       //Debug.Log(nbinit + "--" + nbballes);
    }
   public override void Use()
   {
      Shoot();
   }

   void Shoot()
   {
      Debug.Log("Owner Name in SingleShot: "+Phv.Controller.NickName);
      //TODO
      Phv.RPC("RPC_Shoot",RpcTarget.All);
   }
   

  public IEnumerator Reload(float secondsToWait)
   {
      yield return new WaitForSeconds(secondsToWait);
      nbballes = nbinit;
      munitionsSlider.SetValue(nbballes,nbinit);
   }

   [PunRPC]
   void RPC_Shoot()
   {
      Ray rayon = new Ray(gunTransform.position, gunTransform.forward);
      if (nbballes > 0)
      {
         if (Time.time >= nexttimetofire)
         {
            nbballes -= 1;
            particleSystem.Play();
            audioSource.Play();
            nexttimetofire = Time.time + 1f / firerate;
            //Debug.Log(nbballes + "///" + nbinit);
            if (Physics.Raycast(rayon, out RaycastHit hit, 100f, ~ignoreLayerMask))
            {
               munitionsSlider.SetValue(nbballes, nbinit);
               // Debug.Log(nbballes + " Update " + nbinit);
               Collider[] bimp = Physics.OverlapSphere(hit.point, 0.3f);
               if (bimp.Length != 0)
               {
                  GameObject buletimpact = Instantiate(bulletImpactPrefab, hit.point + hit.normal * 0.001f,
                     Quaternion.LookRotation(hit.normal, Vector3.up) * bulletImpactPrefab.transform.rotation);
                  Destroy(buletimpact, 2f);
                  buletimpact.transform.SetParent(bimp[0].transform);
               }
               
               IDamageable damageableOfCollider = hit.collider.gameObject.GetComponent<IDamageable>();
               damageableOfCollider?.TakeDamage(((GunInfo) iteminfo).damage, PlayerOwner.gameObject.name);
               if (hit.rigidbody != null)
               {
                  hit.rigidbody.AddForce(-hit.normal * impactforce);
               }
            }
         }
      }
   }
}
