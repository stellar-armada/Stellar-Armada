using UnityEngine;
using System.Text.RegularExpressions;
using SpaceCommander.Audio;

namespace VRKeyboard.Utils
{
    public class KeyboardManager : MonoBehaviour
    {
        #region Public Variables
        [Header("User defined")]
        [Tooltip("If the character is uppercase at the initialization")]
        public bool isUppercase = false;
        public int maxInputLength;

        [Header("UI Elements")]
        public TMPro.TextMeshProUGUI inputText;

        public TMPro.TextMeshProUGUI outputText;

        public bool shouldHaveMinimumValue;

        public string defaultDisplayValue = "0";


        public string postFixText = "";

        [Header("Essentials")]
        public Transform keys;
        #endregion

        #region Private Variables
        private string Input
        {
            get { return inputText.text; }
            set {
                inputText.text = value;
                if(outputText != null)
                {

              
                if(inputText.text == "" && shouldHaveMinimumValue)
                {

                    outputText.text = defaultDisplayValue + postFixText;
                }
             else
                {
                    outputText.text = inputText.text + postFixText;
                    }

                // Hack to remove new lines because this was happening for some reason
                    outputText.text = Regex.Replace(outputText.text, @"\t|\n|\r", "");



                }
            }
        }
        private Key[] keyList;
        private bool capslockFlag;
        #endregion

        #region Monobehaviour Callbacks
        void Awake()
        {
            if(outputText != null)
            {
                outputText.text = inputText.text + postFixText;

            }
            keyList = keys.GetComponentsInChildren<Key>();
        }

        void PlayKeyPressed(string s)
        {
            SFXController.instance.PlayOneShot(SFXType.KEY_PRESSED);
        }

        void Start()
        {
            foreach (var key in keyList)
            {
                key.OnKeyClicked += GenerateInput;
                key.OnKeyClicked += PlayKeyPressed;
            }
            capslockFlag = isUppercase;
            CapsLock();
        }
        #endregion

        #region Public Methods
        public void Backspace()
        {
            if (Input.Length > 0)
            {
                Input = Input.Remove(Input.Length - 1);
                SFXController.instance.PlayOneShot(SFXType.KEY_BACKSPACE);
            }
            else
            {
                return;
            }
        }

        public void Clear()
        {
            Input = "";
        }

        public void CapsLock()
        {
            foreach (var key in keyList)
            {
                if (key is Alphabet)
                {
                    key.CapsLock(capslockFlag);
                }
            }
            capslockFlag = !capslockFlag;
        }

        public void Shift()
        {
            foreach (var key in keyList)
            {
                if (key is Shift)
                {
                    key.ShiftKey();
                }
            }
        }

        public void GenerateInput(string s)
        {
            Debug.Log(s);
            if (Input.Length > maxInputLength) { return; }
            Input += s;
        }
        #endregion
    }
}