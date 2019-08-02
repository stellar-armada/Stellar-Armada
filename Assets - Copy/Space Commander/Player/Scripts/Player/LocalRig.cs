using UnityEngine;
using SpaceCommander.Game;
using InputHandling;
using UnityEngine.Serialization;

namespace SpaceCommander.Player
{
    /* This is the local player rig that gets instantiated by the GameManager and exists unless the player disconnects from a game and resets the scene
     * LocalPlayerRig prefab contains the camera, UI, pointer and any controller and local input management
     * When the player joins a game, the local player rig is parented to the Player object
     */

    public class LocalRig : MonoBehaviour
    {
        public static LocalRig instance;
        [SerializeField] private GameObject vrCameraPrefab;
        void Awake()
        {
            if (instance != null)
            {
                Debug.Log("LocalPlayerRig already exists. Destroying.");
                Destroy(instance);
            }

            instance = this;
            GameObject cameraObj = vrCameraPrefab;
            
            InputManager.CreateController();
                InputManager.CreatePointer();

                Instantiate(cameraObj, transform);
        }

        private void OnDestroy()
        {
            instance = null;
        }

    }

}