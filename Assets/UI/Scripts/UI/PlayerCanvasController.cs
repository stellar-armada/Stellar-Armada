using UnityEngine;
using UnityEngine.EventSystems;
using SpaceCommander.Audio;
using SpaceCommander.Game;
using SpaceCommander.Player;

namespace SpaceCommander.UI
{
    /* This is the main canvas controller
     * We have three canvases -- Main, In-game and Shared
     * Shared canvas is always on, but the Settings and Host Panel inside can be enabled/disabled
     * Main and in-game canvas are switched on and on by the [Menu] controller button
     */

    public class PlayerCanvasController : MonoBehaviour
    {
        public static PlayerCanvasController instance; // singleton accessor

        #region Private Variables Serialized In Inspector
        [SerializeField] GameObject inGameMenuCanvas;
        [SerializeField] MatchMessageManager matchCanvasController;
        [SerializeField] MainMenuManager mainMenuManager;
        [SerializeField] GameObject microphoneIndicator;
        [SerializeField] LocalRig localRig;
        [SerializeField] GameObject canvas;
        [SerializeField] GameObject canvasPrefabOculus;
        [SerializeField] GameObject canvasRoot;
        [SerializeField] float worldCanvasSize = .00025f;
        #endregion
        
        #region Private Variables
        private GameObject mobileInputCanvas;
        private Transform t;
        #endregion

        #region Public Events
        public event GameManager.EventHandler EventOnCanvasOpened; // GameManager's void EventHandlers work for this purpose
        public event GameManager.EventHandler EventOnCanvasClosed;
        public event GameManager.EventHandler EventOnMainMenuCanvasOpened;
        public event GameManager.EventHandler EventOnMainMenuCanvasClosed;
        public event GameManager.EventHandler EventOnInGameMenuCanvasOpened;
        public event GameManager.EventHandler EventOnInGameMenuCanvasClosed;
        #endregion

        bool canvasIsActive;

        #region Initialization / Deinitialization

        private void Awake()
        {
            Debug.Log("Canvas controller awake");
            t = transform;
            if (instance != null)
            {
                Debug.Log("PlayerLocalCanvasController already exists. Destroying.");
                Destroy(instance);
            }

            instance = this;

            InitScoreboard();

            mainMenuManager.gameObject.SetActive(false);

            inGameMenuCanvas.gameObject.SetActive(false);

            
            GameObject canvasObj = Instantiate(canvasPrefabOculus, transform);

                canvasRoot.transform.parent = canvasObj.transform;
                Destroy(canvas.gameObject);
                canvas = canvasObj;
                canvasObj.transform.localScale = Vector3.one * worldCanvasSize;
                canvasRoot.transform.position = Vector3.zero;


                Invoke(nameof(SetCanvasEventCamera), .25f);

            Invoke(nameof(SetCanvasPosition), .25f);

                ToggleControllerCanvas(); // Open canvas

            }

        
        void InitLocalPlayerCanvas()
        {
            Debug.Log("Initing local player canvas");
            HideMainMenu();
        }
        
        private void OnDestroy()
        {
            instance = null;
        }

        #endregion

        #region Public Methods

        
        public void HideMainMenu()
        {
            mainMenuManager.gameObject.SetActive(false);
        }

        public bool MenuIsActive()
        {
            return canvasIsActive;
        }
        
        public MatchMessageManager GetHudCanvasController()
        {
            return matchCanvasController;
        }

        public void ToggleMicrophoneUIImage(bool visible)
        {
            microphoneIndicator.SetActive(visible);
        }
        
        public void ShowCanvas()
        {
            EventOnCanvasOpened?.Invoke();
            
            if (PlayerManager.GetLocalNetworkPlayer() == null) // Local player is null, so player is most likely not yet in a game
            {
                mainMenuManager.gameObject.SetActive(true);

                EventSystem.current = mainMenuManager.gameObject.GetComponent<EventSystem>();

                inGameMenuCanvas.gameObject.SetActive(false); // Deactivate other canvas just in case

                EventOnMainMenuCanvasOpened?.Invoke();
            }
            else  // Local player is set, so player is most likely in a game already
            {
                inGameMenuCanvas.gameObject.SetActive(true);

                EventSystem.current = inGameMenuCanvas.gameObject.GetComponent<EventSystem>();

                mainMenuManager.gameObject.SetActive(false);  // Deactivate other canvas just in case
                
                EventOnInGameMenuCanvasOpened?.Invoke();
            }

            canvasIsActive = true;

            SetCanvasPosition();
            
            SFXController.instance.PlayOneShot(SFXType.CANVAS_OPEN);
        }

        private Vector3 u;
        private Transform c;
        public void SetCanvasPosition()
        {

            u = LocalCameraController.instance.uiRoot.position;
            c = LocalCameraController.instance.transform;
            
            t.position = new Vector3(u.x, Mathf.Lerp(c.position.y - .1f, u.y, .5f), u.z); // Set to camera height

            t.LookAt(c, Vector3.up);

            t.rotation = Quaternion.Euler(0, t.rotation.eulerAngles.y + 180f, 0);
        }

        public void HideCanvas()
        {
            canvasIsActive = false;

            EventOnCanvasClosed?.Invoke();

            // Turn both off, just in case it's either one

            if (inGameMenuCanvas.gameObject.activeSelf)
            {
                inGameMenuCanvas.gameObject.SetActive(false);

                EventOnInGameMenuCanvasClosed?.Invoke();

            }
            if (mainMenuManager.gameObject.activeSelf)
            {
                mainMenuManager.gameObject.SetActive(false);
                EventOnMainMenuCanvasClosed?.Invoke();
            }
            
            SFXController.instance.PlayOneShot(SFXType.CANVAS_CLOSED);
        }

    

        public void InitScoreboard()
        {
            inGameMenuCanvas.GetComponent<ScoreboardController>().Awake();
        }

        public void ToggleControllerCanvas()
        {
            if (inGameMenuCanvas.gameObject.activeSelf)
            {
                HideCanvas();
            }
            else
            {
                ShowCanvas();
            }
        }

        #endregion

        #region Private Methods

        void SetCanvasEventCamera()
        {
            canvas.GetComponent<Canvas>().worldCamera = LocalCameraController.instance.GetCamera();
        }

        #endregion
        
    }
}