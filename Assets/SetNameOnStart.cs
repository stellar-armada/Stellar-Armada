using PixelCrushers.DialogueSystem;
using UnityEngine;

public class SetNameOnStart : MonoBehaviour
{
    public void SetName()
    {
        if (NameManager.name == "")
            DialogueLua.SetVariable("Name", " dead inside.");
        else
            DialogueLua.SetVariable("Name", NameManager.name);
    }
}
