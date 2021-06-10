using System.Collections;
using UnityEngine;
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
   public float scopeWaitTime = 0.15f;
   public bool canScope = true;

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
      if (Input.GetMouseButtonDown(1))
      {
         if (!Phv.IsMine || PauseMenu.GameIsPaused || !canScope)
            return;
         isScoped = !isScoped;
         animator.SetBool("Scoped",isScoped);
         scopeOverlay.SetActive(isScoped);
         if (isScoped)
            StartCoroutine(OnScoped(scopeWaitTime));
         else
            OnUnscoped();
      }

   }

   IEnumerator OnScoped(float scopeWait)
   {
      canScope = false;
      // cannot unscope when in the process of scoping
      
      yield return new WaitForSeconds(scopeWait);

      scopeOverlay.SetActive(true);
      weaponCam.SetActive(false);

      normalFOV = mainCamera.fieldOfView;
      mainCamera.fieldOfView = scopedFOV;

      canScope = true;
   }

   private void OnUnscoped()
   {
      scopeOverlay.SetActive(false);
      weaponCam.SetActive(true);

      mainCamera.fieldOfView = normalFOV;
   }
}
