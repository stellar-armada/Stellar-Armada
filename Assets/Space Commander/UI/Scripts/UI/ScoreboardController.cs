using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SpaceCommander.Game;
using SpaceCommander.Player;
using UnityEngine.Serialization;

namespace SpaceCommander.UI
{

    /* Managers the scoreboard object potion of the in-game menu
     * TO-DO: Move bot selection menu logic to 
     */

    public class ScoreboardController : MonoBehaviour
    {
        public static ScoreboardController instance; // Singleton accessor


        public Color[] colors;
        
        #region Private Fields Serialized In Inspector
        [SerializeField] GameObject playerScoreObjectPrefab;
        [SerializeField] GameObject scoreboardPanel;
        [SerializeField] TMPro.TextMeshProUGUI matchTime;
        [FormerlySerializedAs("redTeamPlayerLayoutRoot")] [SerializeField] Transform playerLayoutRoot;
        #endregion

        #region Private Fields
        List<ScoreboardPlayerObject> ScoreboardObjects = new List<ScoreboardPlayerObject>();
        List<IPlayer> players;
        GameObject playerScoreObject;
        ScoreboardPlayerObject playerScoreboardObject;
        bool isInited = false;
        int minutes;
        int seconds;
        private Match m;
        #endregion

        #region Initialization / Deinitialization

        public void Awake()
        {
            instance = this;
            isInited = true;
        }

        private void OnDestroy()
        {
            instance = null;
        }

        #endregion
    
        #region Public Methods

        public GameObject GetScoreboardPanel()
        {
            return scoreboardPanel;
        }

        private void OnEnable()
        {
            //PopulateScoreboard();
        }

        public void UpdateMatchTime(string newMatchTime)
        {
            matchTime.text = newMatchTime;
        }

        public void UpdateScore()
        {
           // redTeamScore.text = Match.GetCurrentMatch().GetScoreFromTeam(IPlayer.Team.Red).ToString();
           // blueTeamScore.text = Match.GetCurrentMatch().GetScoreFromTeam(IPlayer.Team.Blue).ToString();
        }

        public void UpdatePlayers()
        {
            players = PlayerManager.GetPlayers();

            foreach (IPlayer player in players)
            {
                string userName = player.GetName();

                playerScoreObject = null;
                playerScoreObject = Instantiate(playerScoreObjectPrefab, playerLayoutRoot);
                playerScoreObject.GetComponent<ScoreboardPlayerObject>().SetBackgroundColor(player.GetTeam().color);
               
                playerScoreObject.GetComponent<ScoreboardPlayerObject>().Init(player.GetId().ToString(), userName);

                ScoreboardObjects.Add(playerScoreObject.GetComponent<ScoreboardPlayerObject>());
            }
        }

        public void UpdateAssists(IPlayer player)
        {
            playerScoreboardObject = ScoreboardObjects.FirstOrDefault(i => i.netId == player.GetId().ToString());
        }

        public void UpdateKills(IPlayer player)
        {
            return;
            playerScoreboardObject = ScoreboardObjects.FirstOrDefault(i => i.netId == player.GetId().ToString());
        }

        public void UpdateDeaths(IPlayer player)
        {
            playerScoreboardObject = ScoreboardObjects.FirstOrDefault(i => i.netId == player.GetId().ToString());
        }

        public void ChangeName(IPlayer player)
        {
            playerScoreboardObject = ScoreboardObjects.FirstOrDefault(i => i.netId == player.GetId().ToString());

            playerScoreboardObject.SetName(player.GetName());
        }

        public void AddPlayer(IPlayer pc)
        {
            GameObject playerScoreObject = null;

            playerScoreObject = Instantiate(playerScoreObjectPrefab, playerLayoutRoot);

            playerScoreObject.GetComponent<ScoreboardPlayerObject>().SetBackgroundColor(pc.GetTeam().color);

            playerScoreObject.GetComponent<ScoreboardPlayerObject>().Init(pc.GetId().ToString(), pc.GetName());

            ScoreboardObjects.Add(playerScoreObject.GetComponent<ScoreboardPlayerObject>());
        }

        public void RemovePlayer(IPlayer pc)
        {
            ScoreboardPlayerObject objToFind = null;

            foreach (ScoreboardPlayerObject obj in ScoreboardObjects)
            {
                if (obj.netId == pc.GetId().ToString())
                {
                    objToFind = obj;
                }
            }
            if (objToFind != null)
            {
                ScoreboardObjects.Remove(objToFind);

                Destroy(objToFind);

                return;
            }
        }

        public void PopulateScoreboard()
        {
            ClearObjects();

            UpdateScore();

            UpdatePlayers();
        }

        #endregion

        #region Private Methods

        void ClearObjects()
        {
            foreach (ScoreboardPlayerObject obj in ScoreboardObjects)
            {
                Destroy(obj.gameObject);
            }

            ScoreboardObjects.Clear();
        }
        
        void Update()
        {
            return;
            if (Match.IsStarted())
            {
                minutes = Mathf.FloorToInt(Match.GetCurrentMatch().clock.GetMatchTime() / 60.0f);
                seconds = Mathf.FloorToInt(Match.GetCurrentMatch().clock.GetMatchTime() % 60f);
                matchTime.text = minutes + ":" + seconds;
            }
            else
            {
                matchTime.text = "-:--";
            }
        }

        #endregion
    }
}