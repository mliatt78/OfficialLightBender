using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = System.Random;

namespace Zones
{
    public class TeamBaseZone : MonoBehaviour
    {
        [SerializeField] private int team; // 0 for blue, 1 for red
        private List<PlayerController> playersNear = new List<PlayerController>();
        
        public float Radius = 5;
        

        // Update is called once per frame
        void Update()
        {
            if (!PauseMenu.isleft)
            {
                CheckIfPlayersInZone();
                CheckIfPlayersLeave();
                CheckIfPlayersHaveOre();
            }
        }

       void CheckIfPlayersHaveOre()
        {
            for (int i = 0; i < playersNear.Count; i++)
            {
                if (playersNear[i].hasOre && playersNear[i].GetTeam() == team)
                {
                    GameManager.scores[playersNear[i].GetTeam()] += (5 * playersNear[i].GetOresBeingHeld());
                    //PlayerManager.UpdateScores();
                    //TODO
                    playersNear[i].RemoveOres();
                    Debug.Log(playersNear[i].name + " brought its ores back to his base!");
                }
            }
        }

        void CheckIfPlayersInZone()
        {
            
            for (int i = 0; i < PlayerManager.players.Count; i++)
            {
                float dist = Vector3.Distance(PlayerManager.players[i].transform.position, transform.position);
                if (dist < Radius)
                {
                    AddPlayerNear(PlayerManager.players[i]);
                }
            }
        }

        void CheckIfPlayersLeave()
        {
            for (int i = 0; i < playersNear.Count; i++)
            {
                float dist = Vector3.Distance(playersNear[i].transform.position, transform.position);
                if (dist > Radius)
                {
                    RemovePlayerNear(playersNear[i]);
                }
            }
        }
        

        public void AddPlayerNear(PlayerController player)
        {
            //if (player.gameObject.CompareTag("Player"))
            if (!playersNear.Contains(player))
            {
                //Debug.Log("Successfully added a new player near "+name);
                playersNear.Add(player);
            }
        }

        public void RemovePlayerNear(PlayerController player)
        {
            //if (player.gameObject.CompareTag("Player"))
            //Debug.Log("Removed a new player near "+name);
            playersNear.Remove(player);
        }
        
    }
}


