using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace EnemyAI
{
    public class AIController : MonoBehaviourPunCallbacks,IDamageable
    {
        public NavMeshAgent agent;
        public GameObject[] walkpoints;
        private int nbWalkpoints = 4;

        public GameObject[] enemies;
        public string enemyTeam;
        
        public const float maxHealth = 100f;
        public float currentHealth = maxHealth;
        
        [NonSerialized] private bool alreadyAttacked;
        [NonSerialized] private int framesUntilAttack = 60;
        
        //Armes
        [SerializeField]  Item[] items;
        PhotonView Phv;
        Renderer[] visuals;
        public int team;
        int itemIndex;
        int previousItemIndex = -1;
        
        void Awake()
        {
            walkpoints = GameObject.FindGameObjectsWithTag("CHECKPOINT");
            enemies = GameObject.FindGameObjectsWithTag(enemyTeam);
            agent = GetComponent<NavMeshAgent>();
            Phv = GetComponent<PhotonView>();
        }
            
        void Start()
        {
            if (Phv.IsMine)
            {
                EquipItem(0);
            }
            NextWalkPoint();
            visuals = GetComponentsInChildren<Renderer>();
        }
        
        void Update()
        {
            Vector3 DistanceTillCheckpoint = transform.position - agent.destination;
            if (DistanceTillCheckpoint.magnitude < 3f)
            {
                NextWalkPoint();
            }

            (bool InSight, GameObject target) = GetTarget();
            if (InSight)
            {
                Attack(target);
            }
        }

        (bool, GameObject) GetTarget()
        {
            float distanceToNearest = 5f;
            GameObject Nearest = null;
            foreach (var player in enemies)
            {
                Vector3 distance = player.transform.position - agent.destination;
                if (distance.magnitude < distanceToNearest)
                {
                    Nearest = player;
                    distanceToNearest = distance.magnitude;
                }
            }

            return (Nearest != null, Nearest);
        }

        void NextWalkPoint()
        {
            int i = Random.Range(0, nbWalkpoints);
            agent.SetDestination(walkpoints[i].transform.position);
            
            // RaycastHit rch;
            // if (Physics.Raycast(walkpoint.position, walkpoint.up, out rch, 100))
            // {
            //     agent.SetDestination(rch.transform.position);
            // }
        }
        
        /// <summary>
        /// Attack
        ///
        /// </summary>
        /// <param name="enemy"></param>
        /// 
        void Attack(GameObject enemy)
        {
            if (alreadyAttacked)
            {
                framesUntilAttack--;
            }
            else
            {
                transform.LookAt(enemy.transform);
                items[0].Use();
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

        
        void SetRenderers(bool state)
        {
            foreach (var renderer in visuals)
            {
                renderer.enabled = state;
            }
        }
        
        IEnumerator Respawn()
        {
            SetRenderers(false);
            currentHealth = 100;
            PlayerManager.scores[(team+1)%2] += 1;
            Debug.Log((team+1)%2 + " modulo");
            PlayerManager.UpdateScores();
            GetComponent<AIController>().enabled = false;
            Transform spawn = SpawnManager.instance.GetTeamSpawn(team);
            transform.position = spawn.position;
            transform.rotation = spawn.rotation;
            GetComponent<AIController>().enabled = true;
            yield return new WaitForSeconds(1);        
            SetRenderers(true);
        }

        public void TakeDamage(float damage)
        {
            Phv.RPC("RPC_TakeDamage", RpcTarget.All,damage);
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
}
