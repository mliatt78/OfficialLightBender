using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = System.Random;

namespace Zones
{
    public class ZoneCapture : MonoBehaviour
    {
        [SerializeField] private int controlled = 0; // 0 for nobody, 1 for red, 2 for blue
        private List<PlayerController> playersNear = new List<PlayerController>();
        private List<PlayerController> bluePlayers = new List<PlayerController>();
        private List<PlayerController> redPlayers = new List<PlayerController>();
        private List<PlayerController>[] playersTeam = new List<PlayerController>[2];

        [SerializeField] private float maxValue = 3000;
        private double timeValue = 3000;

        public float Radius = 5;
        public bool playerNear;

        public Random rand;




        // Update is called once per frame
        void Update()
        {
            if (this == null)
            {
                Debug.LogError("Cannot update time on a timer with no zones assigned !");
            }

            CheckIfPlayersInZone();
            CheckIfPlayersLeave();

            if (playerNear)
            {
                double factor = 1 + 0.5 * (playersNear.Count - 1);
                // 1x --> 1 player, 1.5x --> 2 players, 2x --> 3 players and so on
                if (timeValue > 0)
                {
                    timeValue -= Time.deltaTime * 100 * factor;
                }
                else
                {
                    int TeamWhoCaptured = GetTeamMaxPlayers();
                    controlled = TeamWhoCaptured;
                    int randInt = rand.Next(playersTeam[TeamWhoCaptured].Count);
                    (playersTeam[TeamWhoCaptured])[randInt].SetHasOre(true);
                    // should add a visual way to see that the player holds the ore
                    timeValue += maxValue;
                    // should not reset but rather tend to neutral
                }
                // timeValue is in centiseconds

                DisplayTimeForPlayers(timeValue);
            }
            else
            {
                double factor = 1.5;
                // recharges 1.5x as fast
                if (timeValue < maxValue)
                {
                    if (timeValue + Time.deltaTime * factor * 100 > maxValue)
                    {
                        timeValue = maxValue;
                    }
                    else
                    {
                        timeValue += Time.deltaTime * factor * 100;
                    }
                }
            }

            DisplayTimeForPlayers(timeValue);
        }

        public void DisplayTimeForPlayers(double timeDisplay)
        {
            if (timeDisplay < 0)
            {
                timeDisplay = 0;
            }

            double seconds = Mathf.FloorToInt((float) (timeDisplay / 100));
            double centiseconds = Mathf.FloorToInt((float) (timeDisplay % 100));

            for (int i = 0; i < playersNear.Count; i++)
            {
                playersNear[i].transform.Find("Canvas").GetComponentsInChildren<TextMeshProUGUI>()[0].text =
                    string.Format("{0:00}:{1:00}", seconds, centiseconds);
            }

        }

        void CheckIfPlayersInZone()
        {
            for (int i = 0; i < PlayerManager.players.Count; i++)
            {
                float dist = Vector3.Distance(PlayerManager.players[i].transform.position, this.transform.position);
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
                float dist = Vector3.Distance(playersNear[i].transform.position, this.transform.position);
                if (dist > Radius)
                {
                    RemovePlayerNear(playersNear[i]);
                }
            }
        }

        
        public void AddPlayerNear(PlayerController player)
        {
            if (player.gameObject.CompareTag("Player"))
            {
                if (!playersNear.Contains(player))
                {
                    Debug.Log("Successfully added a new player near");
                    playersNear.Add(player);
                    playersTeam[player.GetTeam()].Add(player);
                    DisplayTimeForPlayers(timeValue);
                    // show timer
                }

                SetPlayerNear(true);
            }
        }

        public void RemovePlayerNear(PlayerController player)
        {
            if (player.gameObject.CompareTag("Player"))
            {
                player.transform.Find("Canvas").GetComponentsInChildren<TextMeshProUGUI>()[0].text = "";
                // hide timer
                playersNear.Remove(player);
                playersTeam[player.GetTeam()].Remove(player);
                if (playersNear.Count == 0)
                {
                    SetPlayerNear(false);
                }
            }
        }

        public void SetPlayerNear(bool playerNear)
        {
            this.playerNear = playerNear;
        }
        
        
        private int GetTeamMaxPlayers()
        {
            int[] playersTeam = {0, 0};
            for (int i = 0; i < playersNear.Count; i++)
            {
                playersTeam[playersNear[i].GetTeam()] += 1;
            }

            if (playersTeam[0] > playersTeam[1])
            { // more blue players than red
                return 0;
            }

            return 1;
        }

        private void Awake()
        {
            playersTeam[0] = bluePlayers;
            playersTeam[1] = redPlayers;
        }
    }
}
    

