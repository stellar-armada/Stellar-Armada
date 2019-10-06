using System.Linq;
using PixelCrushers.DialogueSystem;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.Playables;

public class PlayableManager : MonoBehaviour
{
   [SerializeField] PlayableAssetDictionary dictionary = new PlayableAssetDictionary();
   [SerializeField] private PlayableDirector director;
   [SerializeField] private bool addTenToDebugValue;
   [SerializeField] private bool debug = false;

   [SerializeField] private LookAtController lookAtcontroller;

   private bool isLooking = false;
   
   
   [SerializeField] private string[] badEndings;

   [SerializeField] private string[] goodEndings;
   
   void HandleBadEnding()
   {
      Debug.Log("Bad ending!");
      CameraFadeController.instance.FadeOut();
      PlayerVoiceManager.instance.SaveConversation();
   }
    
   void HandleGoodEnding()
   {
      Debug.Log("Good ending!");
      ContactController.instance.ShowContact();
      PlayerVoiceManager.instance.SaveConversation();
   }
   
   public void OnConversationLine()
   {
      ConversationState c = DialogueManager.instance.currentConversationState;


      if (c.hasNPCResponse)
      {
         // evaluate NPC response and see if it contains a fail or success string
         // this is some real end of hackathon shit right here. if you're reading this, never do stuff like this
         Debug.Log(c.firstNPCResponse.formattedText.text);
         
         foreach (string str in badEndings)
            if (c.firstNPCResponse.formattedText.text.Contains(str) && str != "")
               HandleBadEnding();
         foreach (string str in goodEndings)
            if (c.firstNPCResponse.formattedText.text.Contains(str) && str != "")
               HandleGoodEnding();
            
      }

      if (c.hasNPCResponse && !isLooking)
      {
         isLooking = true;
         lookAtcontroller.EnableLook();
      }
      
      if (!c.hasNPCResponse) return;
         PlayDialoguePlayable(c.firstNPCResponse.formattedText.text);
         PlayerVoiceManager.instance.RecordResponse("AVERY", c.firstNPCResponse.formattedText.text);
   }
   void PlayDialoguePlayable(string dialogue)
   {
      director.playableAsset = dictionary[dialogue];
      director.RebuildGraph();
      director.time = 0.0;
      director.Play();
      
   }


   public void OnConversationEnded() => DisableLookAtPlayer();

    public void DisableLookAtPlayer()
    {
        if (!isLooking) return;

            isLooking = false;
            lookAtcontroller.DisableLook();
    }
   
   // Debug code to test sequences
   void Update()
   {
      if (!debug) return;
      if (!Input.anyKeyDown) return;
      if (!int.TryParse(Input.inputString, out int val)) return;
      if(val <= dictionary.Count)
            PlayDialoguePlayable(dictionary.Keys.ToList()[val + (addTenToDebugValue ? 10 : 0)]);
   }
   
}
