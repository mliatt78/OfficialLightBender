using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Zones
{
    public class ZoneCapture : MonoBehaviourPun
    {
        [SerializeField] private int controlled = -1; // 0 for blue, 1 for red, -1 for nobody
        private List<PlayerController> playersNear = new List<PlayerController>();
        private List<PlayerController> bluePlayersNear = new List<PlayerController>();
        private List<PlayerController> redPlayersNear = new List<PlayerController>();
        private List<PlayerController>[] playersTeam = new List<PlayerController>[2];

        [SerializeField] private double maxValue = 3000;
        private double maxValueOre = 1500;
        private double[] blueTimers = new double[2];
        private double[] redTimers = new double[2]; 
        private double[,] Timers = new double[2,2];

        public float Radius = 5;
        public bool playerNear;

        // Update is called once per frame
        void Update()
        {
            if (this == null)
            {
                Debug.LogError("Cannot update time on a timer with no zones assigned !");
            }

            if (PauseMenu.isleft)
            {
                return;
            }

            CheckIfPlayersInZone();
           CheckIfPlayersLeave();
            
            if (playerNear)
            {
                DecreaseTimers();
            }
            else
            {
                RechargeTimers();
            }

            DisplayTimeForPlayers(bluePlayersNear,redPlayersNear);
            // this will be difficult. We need to show the timer which has been updated, however, 
            // if a red-player enters a blue-controlled zone with a blue player in it, the blue player should still see the ore timer freezed, while
            // the red player should see the capture timer freezed.
            
            // A player of a certain team can only see the timers of its team, except when recapturing.
            
            
            
        }

        private void DecreaseTimers()
        {
            (int teamTryingControl, int playersTryControl) = GetTeamAndPlayersTryControl(bluePlayersNear, redPlayersNear);
                //Debug.Log("number of playersTryControl: "+playersTryControl);
                if (playersTryControl != 0)
                { // if there are the same number of blue and red players, then do nothing
                    double factor = 1 + 0.5 * (playersTryControl - 1);
                    double timeValue = (Time.deltaTime * 100 * factor);
                    // 1x --> 1 player, 1.5x --> 2 players, 2x --> 3 players and so on
                    if (teamTryingControl == controlled)
                    {
                        if (Timers[teamTryingControl,0] == 0)
                        { // timer Capture at 0, then timer Ore
                            if (Timers[teamTryingControl,1] == 0)
                            { // timer Ore at 0
                                int randInt = GameManager.rand.Next(playersTeam[teamTryingControl].Count);
                                (playersTeam[teamTryingControl])[randInt].AddOre(1);
                                int ores = playersTeam[teamTryingControl][randInt].GetOresBeingHeld();
                                PlayerController oreGetter = playersTeam[teamTryingControl][randInt];
                                string multipleOres = ores > 1 ? "s" : "";
                                oreGetter.SendChatMessage(oreGetter.name,
                                    "Got "+ores+" ore"+multipleOres+".");
                                // should add a visual way to see that the player holds the ore
                                Timers[teamTryingControl,1] += maxValueOre;
                                // resets the timer Ore
                            }
                            else
                            {
                                Timers[teamTryingControl,1] = DecreaseTimer(Timers[teamTryingControl,1],
                                    timeValue);
                            }
                        }
                        else
                        { // first fully bring timer Capture to 0
                            Timers[teamTryingControl,0] = DecreaseTimer(Timers[teamTryingControl,0],
                                timeValue);
                            //Timers[TeamTryingControl][0] -= (int)(Time.deltaTime * 100 * factor);
                        }
                    }
                    else
                    { // 2 possibilities :
                      // zone is neutral or
                      // zone is controlled by enemy
                        if ((teamTryingControl+1)%2 == controlled)
                        { // zone controlled by enemy
                            int teamEnemy = (teamTryingControl + 1) % 2;
                            if (Timers[teamEnemy,0] < maxValue)
                            { // zone to recapture
                                Timers[teamEnemy,0] = IncreaseTimer(Timers[teamEnemy,0],
                                        timeValue,maxValue);
                            }
                            else
                            { // Timers[teamEnemy][0] == maxValue , so reset neutral
                                Timers[0,1] = maxValueOre;
                                Timers[1,1] = maxValueOre; 
                                // reset ore timers

                                Timers[0,0] = maxValue;
                                Timers[1,0] = maxValue; 
                                // reset capture timers, just to be sure. 

                                controlled = -1;
                            }
                        }
                        else
                        { // zone is neutral
                            //Debug.Log("Test step 0 : timer capture of own team "+Timers[TeamTryingControl][0]);
                            if (Timers[teamTryingControl,0] == 0)
                            { // zone was neutral and will be controlled by capturing team
                                controlled = teamTryingControl;
                                
                                int randInt = GameManager.rand.Next(playersTeam[teamTryingControl].Count);
                                (playersTeam[teamTryingControl])[randInt].AddOre(1);
                                //Debug.Log((playersTeam[teamTryingControl])[randInt].name + " has got an ore.");
                            }
                            else
                            {
                                //Debug.Log("Test step 1 : timer capture of enemy "+Timers[(TeamTryingControl+1)%2][0]);
                                if (Timers[(teamTryingControl+1)%2,0] < maxValue)
                                {
                                    Timers[(teamTryingControl + 1) % 2,0] = IncreaseTimer(
                                        Timers[(teamTryingControl + 1) % 2,0],
                                        timeValue, maxValue);
                                }
                                else
                                { // timer capture of enemy team is at max, so decrease own timer capture
                                    Timers[teamTryingControl,0] = DecreaseTimer(Timers[teamTryingControl,0],
                                        timeValue);
                                }
                            }
                        }
                        
                    }
                }
        }

        private void RechargeTimers()
        {
            double factor = 1.5;
            // recharges 1.5x as fast
                
            // recharge timers
            if (Timers[0,0] < maxValue)
            {
                Timers[0,0] = IncreaseTimer(Timers[0,0],
                    (Time.deltaTime * 100 * factor), maxValue);
            }
            else if (Timers[1,0] < maxValue)
            {
                Timers[1,0] = IncreaseTimer(Timers[1,0],
                    (Time.deltaTime * 100 * factor), maxValue);
            }
        }

        private void DisplayTimeForPlayers(List<PlayerController> bluePlayers,List<PlayerController> redPlayers)
        {
            /*
             for each team, 3 possiblities :
             - timer capture of said team is 0, meaning we show the timer ore of said team
             - timer capture of said team is greater than 0, meaning we show the timer capture
             - timer capture of said team is max value, in which case, we show the timer capture of the other team, which is either max value or lower.
             */
            double blueShow = GetTeamShow(0);
            double redShow = GetTeamShow(1);
            
            DisplayTimeForTeam(bluePlayers,blueShow);
            DisplayTimeForTeam(redPlayers,redShow);

        }

        double GetTeamShow(int team)
        {
            if (team > 2 || team < 0)
            {
                throw new ArgumentException("GetTeamShow: cannot get show of team lower than 0 or greater than 2!");
            }
            /*
             for each team, 3 possiblities :
             - timer capture of said team is 0, meaning we show the timer ore of said team
             - timer capture of said team is greater than 0, meaning we show the timer capture
             - timer capture of said team is max value, in which case, we show the timer capture of the other team, which is either max value or lower.
             */

            if (Timers[team,0] == 0)
            {
                return Timers[team,1]; 
                // timer ore of team
            }

            if (Timers[team,0] > 0 && Timers[team,0] < maxValue)
            {
                return Timers[team,0];
                // timer capture of team
            }

            return Timers[(team + 1) % 2,0];
            // timer capture of other team
        }

        static void DisplayTimeForTeam(List<PlayerController> playersOfTeam, double time)
        {
            double seconds = Mathf.FloorToInt((float) (time / 100));
            double centiseconds = Mathf.FloorToInt((float) (time % 100));
            
            for (int i = 0; i < playersOfTeam.Count; i++)
            {
                playersOfTeam[i].timerText.text =
                    string.Format("{0:00}:{1:00}", seconds, centiseconds);
            }
        }


        static double DecreaseTimer(double timer, double decreaseValue)
        {
            if (timer-decreaseValue < 0)
            {
                return 0;
            }
            return timer - decreaseValue;
        }

        static double IncreaseTimer(double timer, double increaseValue, double maxTimerValue)
        {
            if (timer + increaseValue > maxTimerValue)
            {
                return maxTimerValue;
            }
            return timer + increaseValue;
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
        
        private void AddPlayerNear(PlayerController player)
        {
            if (!playersNear.Contains(player))
            {
                Debug.Log("Successfully added a new player near");
                player.timerText.gameObject.SetActive(true);
                playersNear.Add(player);
                playersTeam[player.GetTeam()].Add(player);
                // add to bluePlayers or redPlayers
                
                //Debug.Log("Test on getting other players' timer");

                // TODO
                // get timers of other players, if there's one
                
                /*
                 * what we need to do :
                 * send a RPC to all other players which are in the zone when one joins
                 * so that they could add the player controller to their local lists
                 * then, another RPC is sent by one of the players already in the zone to
                 * the one joining the zone informing of the current timers of the zone,
                 * so they could be up to date 
                 */
                
                
                //Debug.Log("players Count as of now : "+PlayerManager.players.Count);
                //Debug.Log("playersNear Count as of now : "+playersNear.Count);
                
                DisplayTimeForPlayers(bluePlayersNear,redPlayersNear);
                // show timer
            }

            SetPlayerNear(true);
        }

        private void RemovePlayerNear(PlayerController player)
        {
            //if (player.gameObject.CompareTag("Player"))
            player.timerText.text = "";
            player.timerText.gameObject.SetActive(false);
            // hide timer
            playersNear.Remove(player);
            playersTeam[player.GetTeam()].Remove(player);
            if (playersNear.Count == 0)
            {
                SetPlayerNear(false);
            }
        }

        private void SetPlayerNear(bool playerNearby)
        {
            playerNear = playerNearby;
        }


        private static (int team, int playersIncrementTimer) GetTeamAndPlayersTryControl(List<PlayerController> bluePlayers,
            List<PlayerController> redPlayers)
        {
            /*Debug.Log("GetTeamAndPlayersTryControl ");
            Debug.Log("redPlayersCount: " +redPlayers.Count);
            Debug.Log("bluePlayersCount: "+bluePlayers.Count);*/
            int team;
            int playersTimer;
            if (redPlayers.Count > bluePlayers.Count)
            {
                team = 1; // red;
                playersTimer = redPlayers.Count - bluePlayers.Count;
            }
            else
            {
                team = 0; // blue
                playersTimer = bluePlayers.Count - redPlayers.Count;
            }
            // if bluePlayers.count == redPlayers.count, then no matter the team, 
            // playersIncrementTimer will be 0, so no change in timer, 
            // and that is what we want.

            return (team, playersTimer);
        }
        

        private void Awake()
        {
            playersTeam[0] = bluePlayersNear;
            playersTeam[1] = redPlayersNear;

            blueTimers[0] = maxValue;
            blueTimers[1] = maxValueOre;
            redTimers[0] = maxValue;
            redTimers[1] = maxValueOre;

            for (int i = 0; i < blueTimers.Length; i++)
            {
                Timers[0, i] = blueTimers[i];
            }

            for (int i = 0; i < redTimers.Length; i++)
            {
                Timers[1, i] = redTimers[i];
            }
        }

        [PunRPC]
        private void RPC_SetTimers(double[,] timers)
        {
            Timers = timers;
        }
    }
}
    

