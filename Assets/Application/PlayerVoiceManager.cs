using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class PlayerVoiceManager : MonoBehaviour
{
    public static PlayerVoiceManager instance;
    
    void Awake() => instance =this;
    
    private string[] responseStrings;
    private Response[] responses;
    private KeywordRecognizer keywordRecognizer;
    [SerializeField] float audioTriggerThreshold = .3f;
    [SerializeField] private float micWaitTime = .4f;

    
    private string path;
    private string text;
    private StringBuilder sb;

    public void InitializeNewSpeechSession()
    {
        // Create a new text file (use user name) and set up path var for next functions
        path = Path.Combine(Application.streamingAssetsPath, (DateTime.Now.ToLongDateString() + NameManager.name + ".txt"));
          
        sb = new StringBuilder();
    }

    public void InitWatson()
    {
        WatsonManager.instance.Init();
    }

    // Record NPC response, called if NPC has responses
    public void RecordResponse(string actorName, string response)
    {
        // Capture response and write to the text file with actor name and timestamp
        sb.AppendLine(DateTime.Now.ToLongDateString() + " | " + actorName.ToUpper() + " : " + response);
        Debug.Log("<color=blue>" + DateTime.Now.ToLongDateString() + " | " + actorName.ToUpper() + " : " + response + "</color>");
    }

    // Called by conversation ended
    public void SaveConversation()
    {
        Debug.Log("Wrote conversation to: " + path);
        File.WriteAllText(path, sb.ToString());
    }                                                                
    
    void Start()
    {
        DialogueLua.SetVariable("NAME", NameManager.name);
        InitializeNewSpeechSession();
    }

    public void OnConversationResponseMenu()
    {
        ConversationState c = DialogueManager.instance.currentConversationState;

        if (c.hasPCResponses)
        {
            responses = c.pcResponses;
            SetupKeywordListener();
        }
    }

    void SetupKeywordListener()
    {
        if (keywordRecognizer != null)
            keywordRecognizer.Dispose();
        
        responseStrings = new string[responses.Length];
        for (int i = 0; i < responses.Length; i++)
        {
            responseStrings[i] = responses[i].formattedText.text;
            // search for <IMPROVISE>
            if (responseStrings[i].Contains("IMPROVISE"))
            {
                Debug.Log("Improvise section located");
                StartCoroutine(ResponseVoiceHandler());
                return;
            }   
        }
        
        keywordRecognizer = new KeywordRecognizer(responseStrings, ConfidenceLevel.Low);
        keywordRecognizer.OnPhraseRecognized += HandlePhraseRecognized;
        keywordRecognizer.Start();
    }

    private void HandlePhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log(args.text);
        for (int i = 0; i < responseStrings.Length; i++)
        {
            // If found, start the response voice handler
            if (responseStrings[i].Contains(args.text))
            {
                Debug.Log("responseStrings[" + i + "]");
                RecordResponse(NameManager.name.ToUpper(), responseStrings[i]);
                DialogueManager.instance.conversationController.SetCurrentResponse(responses[i]);
                DialogueManager.instance.conversationController.GotoCurrentResponse();
            }
        }
    }


    
    
    // debug to test the voice response handler
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(ResponseVoiceHandler());
    }
    
    
    IEnumerator ResponseVoiceHandler()
    {
        Debug.Log("Voice response handler started");
        InitWatson();
        
        yield return new WaitForSeconds(.5f);
        // Mic check coroutine -- listens for start, kicks off a timer that gets reset every time audio goes above thresh
        bool micInited = false;
        while (!micInited)
        {
            Debug.Log("Waiting for mic to init");
            if (MicInput.CurrentLoudness > audioTriggerThreshold) micInited = true;
            yield return null;
        }

        Debug.Log("Going into wait loop");
        float timer = 0;

        while (timer < micWaitTime)
        {
            timer += Time.deltaTime;
            if (MicInput.CurrentLoudness > audioTriggerThreshold)
            {
                timer = 0;
                Debug.Log("Timer reset");
            }
            yield return null;
        }
        
        
        string text = WatsonManager.instance.ResultsField.text;
        
        // Remove things between parenths from Watson
        text = Regex.Replace(text, @"\(.*\)", "");

// Remove extra spaces.
        text = Regex.Replace(text, @"\s+", " ");
        
        //  TODO: write to file here
        sb.AppendLine(DateTime.Now.ToLongDateString() + " | " + NameManager.name.ToUpper() + ": (Interpreted by Watson) " + text);
        
        yield return null;
        
        Debug.Log("Voice response logged");
        
        DialogueManager.instance.conversationController.SetCurrentResponse(responses[0]);
        DialogueManager.instance.conversationController.GotoCurrentResponse();

        // Log watson here
        
        yield return null;
    }
}