using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class Scope : MonoBehaviour
{
   private  Animator animator;

    private PhotonView Phv;

   private bool isScoped = false;

   public GameObject scopeOverlay;

   public GameObject weaponCam;

   public Camera mainCamera;

   public float scopedFOV = 15f;

   private float normalFOV;

   private void Awake()
   {
      Phv = GetComponent<PhotonView>();
   }

   private void Start()
   {
      if (Phv.IsMine)
         animator = GetComponent<Animator>();
   }

   private void Update()
   {
      if (Input.GetButtonDown("Fire2"))
      {
         
         if (!Phv.IsMine)
            return;
         {
            isScoped = !isScoped;
            animator.SetBool("Scoped",isScoped);
            scopeOverlay.SetActive(isScoped);
            if (isScoped)
               StartCoroutine(OnScoped());
            else
               OnUnscoped();
         }
         
      }

   }

   IEnumerator OnScoped()
   {
      yield return new WaitForSeconds(.15f);

      scopeOverlay.SetActive(true);
      weaponCam.SetActive(false);

      normalFOV = mainCamera.fieldOfView;
      mainCamera.fieldOfView = scopedFOV;
   }

   private void OnUnscoped()
   {
      scopeOverlay.SetActive(false);
      weaponCam.SetActive(true);

      mainCamera.fieldOfView = normalFOV;
   }
}
