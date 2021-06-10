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
   public KeyCode scopeKey = KeyCode.Mouse1;
   
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
      if (Input.GetKeyDown(scopeKey))
      {
         if (!Phv.IsMine || PauseMenu.GameIsPaused)
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
      yield return new WaitForSeconds(scopeWait);

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
