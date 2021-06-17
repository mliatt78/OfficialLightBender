using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] GameObject cameraHolder;
    
    [SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField] SkinnedMeshRenderer playerRenderer;
    
    [SerializeField] float mouseSensivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

    [SerializeField] float respawnTime;

    [SerializeField] List<Item> items;
    
    [SerializeField] ProgressBarPro _progressBarPro;

    [SerializeField] ProgressBarPro munitionsSlider;

    [SerializeField] GameObject munitionObject;

    [SerializeField] GameObject health;

    int itemIndex;
    int previousItemIndex = -1;

    int oresHolded;
    public bool hasOre => oresHolded != 0;
    
    public bool isLocal;

    private bool canRespawn = true;
    private bool DetectCollisions = true;
    private bool isFreezed = false;

    private bool isCrouching,isProning;

    private Vector3 capsuleColliderCenter;
    private int capsuleColliderDirection;
    private float heightCollider;

    private float baseWalkSpeed, baseSprintSpeed;
    
    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    Rigidbody rb;

    public PhotonView Phv;

    private Animator animator;

    PlayerManager playerManager;

    public GameObject lastShooter;

    public TextMeshProUGUI blueScoreText;
    public TextMeshProUGUI redScoreText;

    Renderer[] visuals;
    
    int team;
    int ID;

    public bool PlayerOnlyLook;

    private const float maxHealth = 100f;
    public float currentHealth = maxHealth;

    private SingleShot singleshot;
    private Grenade grenade;
    
    public Chat chat;

    public GameObject launchbutton;

    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Phv = GetComponent<PhotonView>();

        baseWalkSpeed = walkSpeed;
        baseSprintSpeed = sprintSpeed;
        
        heightCollider = capsuleCollider.height;
        capsuleColliderCenter = capsuleCollider.center;
        capsuleColliderDirection = capsuleCollider.direction;

        //playerManager = PhotonView.Find((int)Phv.InstantiationData[0]).GetComponent<PlayerManager>();

        if (!PlayerManager.players.Contains(this)) 
            PlayerManager.players.Add(this);
        

        // playerController is instantiated on each machine in the room. 
        // by doing this, it will locally, on each machine in the room 
        // (PhotonNetwork.Instantiate() creates the object for all machines)
        // add the object to the players. 

        // DEBUG
        /*Debug.Log("Displaying PlayerManager.players on machine of player "+name+":");
        for (int i = 0; i < PlayerManager.players.Count; i++)
        {
            Debug.Log("PLayer Name:" + PlayerManager.players[i].name);
        }*/
    }


    void Start()
    {
        //Debug.Log("Starting");
        if (Phv.IsMine)
        {
            //Debug.Log("Phv is mine");
            EquipItem(0);
            animator = GetComponent<Animator>();
            health.SetActive(true);
            munitionObject.SetActive(true);
            PauseMenu.isleft = true;

            if (items[itemIndex] is SingleShot)
            {
                singleshot = (SingleShot) items[itemIndex];
                //Debug.Log("Name " + items[itemIndex].name);
                //Debug.Log(singleshot.nbballes + " :::: " + singleshot.nbinit);
            }
            

            visuals = GetComponentsInChildren<Renderer>();
            team = (int) PhotonNetwork.LocalPlayer.CustomProperties["Team"];
            Debug.Log("Instantiation is finished");
            GameManager.instance.currentweapon = 0;
        }
        else
        {
            Debug.Log("Destroy component");
            Debug.Log("Owner name of phv: " + Phv.Owner.NickName);
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }

        Debug.Log("IsMasterClient " + PhotonNetwork.IsMasterClient +" IsLobby : " + GameManager.instance.IsLobby );

        if (PhotonNetwork.IsMasterClient && GameManager.instance.IsLobby)
        {
            Debug.Log("Set Active is True");
            GameManager.instance.settingsbutton.SetActive(true);
            launchbutton.SetActive(true);
        }
    }


    void Update()
    {
        
        //Debug.Log(PauseMenu.GameIsPaused + "  <>  " + Phv.IsMine );
        if (!Phv.IsMine || PauseMenu.GameIsPaused)
            return;
        
        Look();

        if (PlayerOnlyLook)
        {
            return;
        }
        
        Move();
        Jump();
        CheckCrouchProne();

        for (int i = 0; i < items.Count; i++)
        {
            //Debug.Log("Items count : " + items.Count);
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if (itemIndex >= items.Count - 1)
            {
                EquipItem(0);
                Debug.Log("Equip item");
            }
            else
            {
                EquipItem(itemIndex + 1);
            }
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if (itemIndex <= items.Count - 1)
            {
                EquipItem(items.Count - 1);
            }
            else
            {
                EquipItem(itemIndex - 1);
            }
        }
        
        if (Input.GetKey(GameManager.instance.keys["Shoot"]))

        {
            items[itemIndex].Use();
            if (items[itemIndex] is Grenade)
            {
                Debug.Log("Item index : " + itemIndex);
                //Debug.Log("I use the grenade 2 ");
                items.RemoveAt(itemIndex);
                previousItemIndex = (itemIndex - 2 < 0 ? 0 : itemIndex - 2); 
                EquipItem(itemIndex - 1);
                Debug.Log("Item bomb ");
            }
        }
        

        /*if (transform.position.y < -10f)
        {
            Die();
        }*/

        if (Input.GetKeyDown(GameManager.instance.keys["Lock"]))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }


        if (Input.GetKeyDown(GameManager.instance.keys["Unlock"]))

        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }


        if (Input.GetKeyDown(GameManager.instance.keys["Reload"]))

        {
            StartCoroutine(singleshot.Reload(singleshot.secondsToReload));
        }
    }
    
    void FixedUpdate()
    {
        if (PlayerOnlyLook)
        {
            rb.velocity = Vector3.zero;
            // we do not want the player to move if the player stopped
        }

        if (!Phv.IsMine || PauseMenu.GameIsPaused || PlayerOnlyLook)
            return;
        
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    /*public bool GetInputRaw(Dictionary<string, KeyCode> dict,string keycode)
    {
        foreach (var input in dict)
            if (Input.GetKey(input.Value) && keycode == input.Key)
                return true;
        return false;
    }

    public int CustomGetAxisRaw(bool inputX, bool inputY)
    {
        int ret = 0;
        ret += (inputX ? 1 : 0);
        ret += (inputY ? -1 : 0);
        return ret;
    }*/

    public Vector3 CustomGetAxisRaw(Dictionary<string, KeyCode> dict)
    {
        Vector3 vect = Vector3.zero;
        foreach (var dc in dict)
        {
            if (Input.GetKey(dc.Value))
            {
                //Debug.Log(dc.Value);
                switch (dc.Key)
                {
                    case "Up":
                        vect.z += 1;
                        break;
                    case "Down":
                        vect.z -= 1;
                        break;
                    case "Left":
                        vect.x -= 1;
                        break;
                    case "Right":
                        vect.x += 1;
                        break;
                }
            }
        }
        return vect.normalized;
    }

    void Move()
    {
        //Debug.Log("Movement activated");
        //Debug.Log(moveDir.x + " : " + moveDir.y +" : " + moveDir.z + " ");

        Vector3 moveDir = CustomGetAxisRaw(GameManager.instance.keys);

        moveAmount = Vector3.SmoothDamp(moveAmount,
            moveDir * (Input.GetKey(GameManager.instance.keys["Sprint"]) ? sprintSpeed : walkSpeed),
            ref smoothMoveVelocity, smoothTime);


        bool isWalking = animator.GetBool("IsWalking");
        bool pressedwalk = Input.GetKey(GameManager.instance.keys["Up"]);

        bool isRunning = animator.GetBool("IsRunning");
        bool pressedrun = Input.GetKey(GameManager.instance.keys["Sprint"]);

        bool isLeft = animator.GetBool("IsLeftWalk");
        bool isRight = animator.GetBool("IsRightWalk");
        bool pressedleft = Input.GetKey(GameManager.instance.keys["Left"]);
        bool pressedright = Input.GetKey(GameManager.instance.keys["Right"]);
        
        bool pressedcrouch = Input.GetKey(GameManager.instance.keys["Crouch"]);
        bool iscrouch = animator.GetBool("IsCrouch");
        
        bool pressedprone = Input.GetKey(GameManager.instance.keys["Prone"]);
        bool isprone = animator.GetBool("IsProne");
        
        

        bool isDance = animator.GetBool("IsDance");
        bool presseddance = Input.GetKey(GameManager.instance.keys["Dance"]);
        ;
        //Debug.Log("Movement");

        // walk
        if (!isWalking && pressedwalk)
        {
            animator.SetBool("IsWalking", true);
            //Debug.Log("Walking");
        }

        if (isWalking && !pressedwalk)
        {
            animator.SetBool("IsWalking", false);
        }

        // run
        if (!isRunning && pressedrun)
        {
            animator.SetBool("IsRunning", true);
        }

        if (isRunning && !pressedrun)
        {
            animator.SetBool("IsRunning", false);
        }

        // left
        if (!isLeft && pressedleft)
        {
            animator.SetBool("IsLeftWalk", true);
        }

        if (isLeft && !pressedleft)
        {
            animator.SetBool("IsLeftWalk", false);
        }

        // right
        if (!isRight && pressedright)
        {
            animator.SetBool("IsRightWalk", true);
        }

        if (isRight && !pressedright)
        {
            animator.SetBool("IsRightWalk", false);
        }

        //dance
        if (!isDance && presseddance)
        {
            animator.SetBool("IsDance", true);
        }

        if (isDance && !presseddance)
        {
            animator.SetBool("IsDance", false);
        }
        
        
        if (!iscrouch && pressedcrouch)
        {
            animator.SetBool("IsCrouch", true);
        }
        if (iscrouch && !pressedcrouch)
        {
            animator.SetBool("IsCrouch", false);
        }
        
        
        if (!isprone && pressedprone)
        {
            animator.SetBool("IsProne", true);
            
        }
        if (isprone && !pressedprone)
        {
            animator.SetBool("IsProne", false);
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
        if (Input.GetKeyDown(GameManager.instance.keys["Jump"]) && grounded)
        {
            UnCrouchOrProne();
            rb.AddForce(transform.up * jumpForce);
        }
    }
    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;

        itemIndex = _index;
        GameManager.instance.currentweapon = itemIndex;

        items[itemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }


        if (items[itemIndex] is SingleShot)
        {
            singleshot = (SingleShot) items[itemIndex];
            munitionsSlider.SetValue(singleshot.nbballes, singleshot.nbinit);
        }
        else if (items[itemIndex] is Grenade)
        {
            grenade = (Grenade) items[itemIndex];
        }
        else
            Debug.LogError("ERROR");


        //Debug.Log(singleshot.nbballes + "*/*/*/*/" + singleshot.nbinit);

        previousItemIndex = itemIndex;

        if (Phv.IsMine)
        {
            Hashtable hash = new Hashtable {{"itemindex", itemIndex}};
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!Phv.IsMine && targetPlayer == Phv.Owner) EquipItem((int) changedProps["itemindex"]);
    }

    private void ResetAnimator()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsLeftWalk", false);
        animator.SetBool("IsRightWalk", false);
        animator.SetBool("IsDance", false);
    }

    private void SetCollisionState(bool state)
    {
        rb.detectCollisions = state;
        capsuleCollider.enabled = state;
        DetectCollisions = state;
    }

    private void SetFreezeState(bool state)
    {
        rb.constraints = (state ? RigidbodyConstraints.FreezeAll 
            : RigidbodyConstraints.FreezeRotation);
        isFreezed = state;
    }

    void CheckCrouchProne()
    {
        if (Input.GetKeyDown(GameManager.instance.keys["Prone"]))
        {
            if (!isProning)
            {
                Prone();
            }
            else
            {
                UnCrouchOrProne();
            }
        }
        else if (Input.GetKeyDown(GameManager.instance.keys["Crouch"]))
        {
            if (!isCrouching)
            {
                Crouch();
            }
            else
            {
                UnCrouchOrProne();
            }
        }
        
    }


    private void Crouch()
    {
        capsuleCollider.height = 1f;
        Vector3 center = capsuleCollider.center;
        center.y = 0.5f;
        capsuleCollider.center = center;

        walkSpeed = baseWalkSpeed/2;
        sprintSpeed = baseSprintSpeed/2;

        isCrouching = true;
    }
    
    private void Prone()
    {
        capsuleCollider.direction = 2; // z-axis
        Vector3 center = capsuleCollider.center;
        center.y = 0.2f; // just above floor
        capsuleCollider.center = center;

        walkSpeed = baseWalkSpeed/3;
        sprintSpeed = baseSprintSpeed/6;

        isProning = true;
    }

    private void UnCrouchOrProne()
    {
        capsuleCollider.height = heightCollider;
        capsuleCollider.center = capsuleColliderCenter;
        capsuleCollider.direction = capsuleColliderDirection;

        walkSpeed = baseWalkSpeed;
        sprintSpeed = baseSprintSpeed;

        isCrouching = false;
        isProning = false;
    }

    public void SetGroundedState(bool _grounded) => grounded = _grounded;
    public void AddOre(int oresToAdd) => oresHolded += oresToAdd;
    public int GetOresBeingHeld() => oresHolded;
    public void RemoveOres() => oresHolded = 0;
    public bool GetOnlyLook() => PlayerOnlyLook;

    public void SetOnlyLook(bool onlyLook)
    {
        PlayerOnlyLook = onlyLook;
        ResetAnimator();
        rb.velocity = Vector3.zero;
    }

    public void SetTeam(int Team) => team = Team;

    public int GetTeam() => team;

    public void TakeDamage(float damage) // juste sur le shooter
        =>
            Phv.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    
     IEnumerator Respawn(float respawnWait)
     {
         canRespawn = false;
         // overflow protection for respawn
         
         SetRenderers(false);

         currentHealth = 100;
         GameManager.scores[(team+1)%2] += 1;
         //PlayerManager.UpdateScores();
         // TODO
         
         if (hasOre)
         {
             RemoveOres();
         }
         
         //GetComponent<PlayerController>().enabled = false;
         Transform spawn = SpawnManager.instance.GetTeamSpawn(team);

         SendChatMessage("System",
             lastShooter.name +" killed " + name);
         
         SetCollisionState(false);
         SetFreezeState(true);
         
         yield return new WaitForSeconds(respawnWait);

         currentHealth = 100; 
         // just in case someone manages to shoot the player when waiting
         _progressBarPro.SetValue(100f,100f);
         
         transform.position = spawn.position;
         transform.rotation = spawn.rotation;
         //GetComponent<PlayerController>().enabled = true;
         
         SetCollisionState(true);
         SetFreezeState(false);

         SetRenderers(true);
         canRespawn = true;
     }

     void SetRenderers(bool state)
     {
         foreach (var renderer in visuals)
         {
             renderer.enabled = state;
         }
     }

     public void SendChatMessage(string sender, string message)
     {
         Phv.RPC("SendChat",RpcTarget.All,sender,message);
     }

     [PunRPC]
     void RPC_TakeDamage(float damage)
     {
         if (!Phv.IsMine)
             return;

         currentHealth -= damage;
         _progressBarPro.SetValue(currentHealth,100f);
         if (currentHealth <= 0 && canRespawn)
         {
             StartCoroutine(Respawn(respawnTime));
         }
     }
     
     [PunRPC]
     void SendChat(string sender, string message)
     {
         ChatMessage m = new ChatMessage(sender,message);

         GameManager.chatMessages.Insert(0, m);
         if(GameManager.chatMessages.Count > 8)
         {
             GameManager.chatMessages.RemoveAt(GameManager.chatMessages.Count - 1);
         }
         Chat.chatMessages = GameManager.chatMessages;
         // responsible for the synchronisation of all messages
     }
     
     public void StartGame()
     {
         Debug.Log("Start Game");
         PlayerManager.localPlayerInstance = null;
         PhotonNetwork.LoadLevel(2) ; // index de la scene
         
     }
}