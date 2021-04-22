using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class PlayerController : MonoBehaviourPunCallbacks,IDamageable
{
    [SerializeField] GameObject cameraHolder;
    
    [SerializeField] float mouseSensivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

    [SerializeField]  Item[] items;

    int itemIndex;
    int previousItemIndex = -1;
     
    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;
    
    Rigidbody rb;

    PhotonView Phv;

    private Animator animator;

    /* const float maxHealth = 100f;
     float currentHealth = maxHealth;*/

    PlayerManager playerManager;
    
    Renderer[] visuals;
    int team = 0;
    public const float maxHealth = 100f;
    public float currentHealth = maxHealth;


   

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Phv = GetComponent<PhotonView>();
        //playerManager = PhotonView.Find((int)Phv.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    void Start()
    {
        if (Phv.IsMine)
        {
            EquipItem(0);
            animator = GetComponent<Animator>();
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }
        visuals = GetComponentsInChildren<Renderer>();
        team = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
    }
    
    void Update()
    {
        if (!Phv.IsMine)
            return;
        
        Look();
        Move();
        Jump();

        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if (itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if (itemIndex <= items.Length - 1)
            {
                EquipItem(items.Length - 1);
            }
            else
            {
                EquipItem(itemIndex - 1);
            }
           
        }

        if (Input.GetMouseButton(0))
        {
            items[itemIndex].Use();
        }

        /*if (transform.position.y < -10f)
        {
            Die();
        }*/
        if (Input.GetKeyDown("1"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Input.GetKeyDown("2"))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed),
            ref smoothMoveVelocity, smoothTime);
        
        bool isWalking = animator.GetBool("IsWalking");
        bool pressedwalk = Input.GetKey("w");
        
        bool isRunning = animator.GetBool("IsRunning");
        bool pressedrun = Input.GetKey(KeyCode.LeftShift);
        
        bool isLeft = animator.GetBool("IsLeftWalk");
        bool isRight = animator.GetBool("IsRightWalk");
        bool pressedleft = Input.GetKey("a");;
        bool pressedright = Input.GetKey("d");;
        
        bool isDance = animator.GetBool("IsDance");
        bool presseddance = Input.GetKey("z");;
        
        // walk
        if (!isWalking && pressedwalk)
        {
            animator.SetBool("IsWalking",true);
        }
        if (isWalking && !pressedwalk)
        {
            animator.SetBool("IsWalking",false);
        }
        // run
        if(!isRunning && pressedrun)
        {
            animator.SetBool("IsRunning",true);
        }
        if (isRunning && !pressedrun)
        {
            animator.SetBool("IsRunning",false);
        }
        // left
        if(!isLeft && pressedleft)
        {
            animator.SetBool("IsLeftWalk",true);
        }
        if (isLeft && !pressedleft)
        {
            animator.SetBool("IsLeftWalk",false);
        }
        // right
        if(!isRight && pressedright)
        {
            animator.SetBool("IsRightWalk",true);
        }
        if (isRight && !pressedright)
        {
            animator.SetBool("IsRightWalk",false);
        }
        
        //dance
        if(!isDance && presseddance)
        {
            animator.SetBool("IsDance",true);
        }
        if (isDance && !presseddance)
        {
            animator.SetBool("IsDance",false);
        }
    }
    void Look()
    {
        transform.Rotate(Vector3.up * (Input.GetAxisRaw("Mouse X") * mouseSensivity));

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90);
        
        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;
        
        itemIndex = _index;

        items[itemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }

        previousItemIndex = itemIndex;
        
        if (Phv.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemindex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!Phv.IsMine && targetPlayer == Phv.Owner)
        {
            EquipItem((int) changedProps["itemindex"]);
        }
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded; 
    }

     void FixedUpdate()
    {
        if (!Phv.IsMine)
            return;
        
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }
     
     public void TakeDamage(float damage) // juste sur le shooter
     {
         Phv.RPC("RPC_TakeDamage", RpcTarget.All,damage);
     }
    
     IEnumerator Respawn()
     {
         SetRenderers(false);
         currentHealth = 100;
         GetComponent<PlayerController>().enabled = false;
         Transform spawn = SpawnManager.instance.GetTeamSpawn(team);
         transform.position = spawn.position;
         transform.rotation = spawn.rotation;
         GetComponent<PlayerController>().enabled = true;
         yield return new WaitForSeconds(1);        
         SetRenderers(true);
     }

     void SetRenderers(bool state)
     {
         foreach (var renderer in visuals)
         {
             renderer.enabled = state;
         }
     }
    
     [PunRPC]
     void RPC_TakeDamage(float damage)
     {
         if (!Phv.IsMine)
             return;

         currentHealth -= damage;

         if (currentHealth <= 0)
         {
             StartCoroutine(Respawn());
         }
     }
}


     
     
     
     
     
     
     
     
     
     
     
     
     
     
     
     
     
     
     
     
     
     
     
     
     
     
     
     
     
     
   /* public void TakeDamage(float damage) // juste sur le shooter
    {
        Phv.RPC("RPC_TakeDamage", RpcTarget.All,damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage) // execute sur tous les ordinateurs
    {
        if (!Phv.IsMine)
            return;
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        playerManager.Die();
    }*/

