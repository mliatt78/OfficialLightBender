using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;

namespace EnemyAI
{
    public class AIController : MonoBehaviour
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
        void Awake()
        {
            walkpoints = GameObject.FindGameObjectsWithTag("CHECKPOINT");
            enemies = GameObject.FindGameObjectsWithTag(enemyTeam);
            agent = GetComponent<NavMeshAgent>();
        }
            
        void Start()
        {
            NextWalkPoint();
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

        void Attack(GameObject enemy)
        {
            if (alreadyAttacked)
            {
                framesUntilAttack--;
            }
            else
            {
                transform.LookAt(enemy.transform);
                // insert shooting code here
            }
        }
    }
}
