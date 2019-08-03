using UnityEngine;
using Mirror;
using SpaceCommander.Player;

namespace SpaceCommander.Game
{
    // Manages current game state, match state, list of players
    // Also initializes the other game managers, to ensure proper execution order even if our project settings are lost
    // Now that things are a bit abstracted, we might be able to have our managers self-execute again, but sometime's it's nice to know exactly whre and when everything Inits :)

    public class GameManager : MonoBehaviour
    {
        public static GameManager instance; // singleton accessor
        [SerializeField] private GameObject matchPrefab;


        public delegate void EventHandler(); // generic event handler prototype that doesn't take any values

        [SerializeField] GameObject localPlayerRigPrefab;
        [SerializeField] private GameObject serverObjectPrefab;
        
        public EventHandler EventOnMatchStarted;
        public EventHandler EventOnMatchEnded;
        public EventHandler EventOnNewMatchCreated;

        void Awake()
        {
            if (instance != null)
            {
                Destroy(instance);
            }

            instance = this;
            Instantiate(localPlayerRigPrefab);

        }

        private void OnDestroy()
        {
            instance = null;
            PlayerController.ClearDelegates();
        }
        
        static PlayerController localPlayer;

        public static void CreateServerObject()
        {
            Debug.Log("Creating server network data");
            GameObject serverObject = Instantiate(instance.serverObjectPrefab, instance.transform);
            NetworkServer.Spawn(serverObject);
        }

    }
}