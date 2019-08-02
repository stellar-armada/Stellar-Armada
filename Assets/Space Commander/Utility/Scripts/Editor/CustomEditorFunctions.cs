using UnityEditor;
using UnityEngine;

namespace SpaceCommander.Editor
{
    public class CustomEditorMethods : MonoBehaviour
    {
        // Put helper methods here
        [MenuItem("Utils/Delete All PlayerPrefs")]
        static public void DeleteAllPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
    }
    
}