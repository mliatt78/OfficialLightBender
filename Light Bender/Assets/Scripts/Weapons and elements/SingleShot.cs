using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShot : GUN
{
   [SerializeField] Camera cam;
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
      }
   }
}
