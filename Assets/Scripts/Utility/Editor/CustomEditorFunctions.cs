using UnityEditor;
using UnityEngine;

#pragma warning disable 0649
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