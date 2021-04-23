using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace EnemyAI
{
    public class AIController : MonoBehaviour
    {
        public NavMeshAgent agent;

        Transform checkpoint;

        private bool ToCheck = false;

        private bool alreadyset = false;
        void Awake()
        {
            checkpoint = GameObject.FindWithTag("CHECKPOINT").transform;
            agent = GetComponent<NavMeshAgent>();
            RaycastHit rch;
            if (Physics.Raycast(checkpoint.position, checkpoint.up, out rch, 100))
            {
                checkpoint = rch.transform;
            }
        }
            
        void Start()
        {
            agent.SetDestination(checkpoint.position);
        }
        void Update()
        {
            Debug.Log(agent.destination+" "+agent.pathStatus+" "+checkpoint.position); 
            if (!ToCheck)
            {
                Vector3 DistanceTillCheckpoint = transform.position - checkpoint.position;
                if (DistanceTillCheckpoint.magnitude < 1f)
                {
                    Debug.Log("Checkpoint atteint");
                    ToCheck = true;
                }
            }
        }
        
        IEnumerator wait()
        {

            yield return new WaitForSeconds(5);

        }
    }
}
