using UnityEngine;
using UnityEngine.UI;
using Mirror;
using SpaceCommander.Game;

namespace SpaceCommander.UI
{
    // Controller for in-game client options submenu
    public class ClientOptionsMenuManager : MonoBehaviour
    {

        [SerializeField] Button joinRedButton;
        [SerializeField] Button joinBlueButton;
        [SerializeField] Button spectateButton;
        [SerializeField] GameObject mainClientOptionsPanel;
        [SerializeField] GameObject settingsMenuSubpanel;
        [SerializeField] GameObject clientOptionsParentPanel;
        [SerializeField] GameObject scoreboardPanel;

        private void OnEnable()
        {
            CheckTeam();
        }

        void CheckTeam()
        {
            Debug.Log("Handle team swap logic here");
            /*
            if (IPlayer.GetLocalNetworkPlayer().GetTeam() == PlayerController.Team.Red)
            {
                joinRedButton.gameObject.SetActive(false);
                joinBlueButton.gameObject.SetActive(true);
                spectateButton.gameObject.SetActive(true);

            }
            if (IPlayer.GetLocalNetworkPlayer().GetTeam() == PlayerController.Team.Blue)
            {
                joinBlueButton.gameObject.SetActive(false);
                joinRedButton.gameObject.SetActive(true);
                spectateButton.gameObject.SetActive(true);
            }
            if (IPlayer.GetLocalNetworkPlayer().GetTeam() == PlayerController.Team.Spectator)
            {
                spectateButton.gameObject.SetActive(false);
                joinBlueButton.gameObject.SetActive(true);
                joinRedButton.gameObject.SetActive(true);
            }
            */
        }


        public void Spectate()
        {
            Debug.Log("Handle spectate logic here");

           // PlayerController.localInstance.JoinTeam(IPlayer.Team.Spectator);
           // ScoreboardController.instance.PopulateScoreboard();
            CheckTeam();
            ResetCanvas();
        }

        public void ShowSettings()
        {
            mainClientOptionsPanel.SetActive(false);
            settingsMenuSubpanel.SetActive(true);
        }

        public void HideSettings()
        {
            mainClientOptionsPanel.SetActive(true);
            settingsMenuSubpanel.SetActive(false);
            scoreboardPanel.SetActive(true); // just in case
        }

        public void Disconnect()
        {
           // ResetCanvas();
            if (PlayerManager.GetLocalNetworkPlayer().IsServer())
            {
                NetworkManager.singleton.StopHost();
                Debug.Log(" *** DISCONNECT CODE GOES HERE");
            }
            else
            if (PlayerManager.GetLocalNetworkPlayer().IsClient())
            {
                NetworkManager.singleton.StopClient();
            }
        }

        public void HideClientOptionsMenu()
        {
            clientOptionsParentPanel.SetActive(false);
        }

        // reset active states and hide the canvas after we've pressed a button
        void ResetCanvas()
        {
            clientOptionsParentPanel.SetActive(false);
            settingsMenuSubpanel.SetActive(false);
            scoreboardPanel.SetActive(true);
            PlayerCanvasController.instance.HideCanvas();
        }
    }
}