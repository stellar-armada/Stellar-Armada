using UnityEngine;
using SpaceCommander.Game;

namespace SpaceCommander.UI
{
    public class HostOptionsMenuManager : MonoBehaviour
    {

        [SerializeField]
        GameObject resumeButton;
        [SerializeField]
        GameObject pauseButton;
        [SerializeField]
        GameObject restartButton;

        [SerializeField]
        GameObject mainHostPanel;
        [SerializeField]
        GameObject newMatchPanel;
        [SerializeField]
        GameObject scoreboardPanel;


        public void ShowNewMatchPanel()
        {
            newMatchPanel.SetActive(true);
            mainHostPanel.SetActive(false);
        }

        public void HideNewMatchPanel()
        {
            newMatchPanel.SetActive(false);
            mainHostPanel.SetActive(true);
        }

        public void HideMainHostPanel()
        {
            mainHostPanel.SetActive(false);

        }

        void ResetCanvas()
        {
            PopulateButtons();

            newMatchPanel.SetActive(false);
            mainHostPanel.SetActive(false);

            scoreboardPanel.SetActive(true);
            PlayerCanvasController.instance.HideCanvas();
        }

        private void OnEnable()
        {
            PopulateButtons();
        }

        void PopulateButtons()
        {
            if (Match.IsPaused())
            {
                resumeButton.gameObject.SetActive(true);
                pauseButton.gameObject.SetActive(false);
            }
            else
            {
                resumeButton.gameObject.SetActive(false);
                pauseButton.gameObject.SetActive(true);
            }
            if (Match.InLobby())
            {
                restartButton.SetActive(true);
            }
            else
            {
                restartButton.SetActive(false);

            }
        }

        public void Pause()
        {
            Match.GetCurrentMatch().Pause();
            PopulateButtons();
            ResetCanvas();

        }

        public void Resume()
        {
            Match.GetCurrentMatch().Resume();
            PopulateButtons();
            ResetCanvas();

        }

        public void RestartMatch()
        {
            Match.GetCurrentMatch().ResetMatch();
            // hide this panel and in game menu canvas
            ResetCanvas();

        }


    }
}