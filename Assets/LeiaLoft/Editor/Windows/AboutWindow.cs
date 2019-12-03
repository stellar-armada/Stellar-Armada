/****************************************************************
*
* Copyright 2019 © Leia Inc.  All rights reserved.
*
* NOTICE:  All information contained herein is, and remains
* the property of Leia Inc. and its suppliers, if any.  The
* intellectual and technical concepts contained herein are
* proprietary to Leia Inc. and its suppliers and may be covered
* by U.S. and Foreign Patents, patents in process, and are
* protected by trade secret or copyright law.  Dissemination of
* this information or reproduction of this materials strictly
* forbidden unless prior written permission is obtained from
* Leia Inc.
*
****************************************************************
*/

using UnityEngine;
using UnityEditor;


namespace LeiaLoft.Editor {

    [InitializeOnLoad]
    public class AboutWindow : UnityEditor.EditorWindow {

		const string HeaderWhatsNew = "What's new?";
		const string HeaderLearning = "Learning resources";
		const string HeaderLinks = "Links";
		const string ReleaseButtonText = "Release Notes";
		const string ReleaseButtonTip = "View release notes and upgrade instructions";
		const string DocButtonText = "Documentation";
		const string DocButtonTip = "View online documentation";
		const string WebsiteButtonText = "Website";
		const string WebsiteButtonTip = "Visit LeiaLoft's main website";
		const string HelpButtonText = "Help";
		const string HelpButtonTip = "Submit a help request";
		const string BackButtonText = "Back";
		const string BackButtonTip = "Go back to the main About page";
		const string WebsiteLink = "https://www.leiainc.com";
		const string BannerAssetPath = "LeiaLoft/Leia-loft-logo.png";

        private enum Page { Main, ReleaseNotes };
        private static Page _page = Page.Main;
        private static GUIStyle _monospace;

        private static bool _isInitialized = false;

        private static Texture2D _bannerImage;
        private static Texture2D _white;

		//[MenuItem ("LeiaLoft/Help/Documentation")]
        public static void OpenDocumentation() 
        {
			Application.OpenURL(WebsiteLink);
        }

		[MenuItem ("LeiaLoft/Help/Release Notes")]
        public static void OpenReleaseNotes()
        {
            EditorWindow.GetWindow<AboutWindow>(true);
            _page = Page.ReleaseNotes;
        }

        public static void OpenLeiaMainURL()
        {
			Application.OpenURL(WebsiteLink);
        }

        public static void OpenLeiaHelpURL()
        {
			Application.OpenURL(WebsiteLink);
        }

		//[MenuItem ("LeiaLoft/About")]
        public static void Open()
        {
            EditorWindow.GetWindow<AboutWindow>(true);
        }

        private void OnEnable()
        {
			_bannerImage = (Texture2D) EditorGUIUtility.Load(BannerAssetPath);
        }


        private void MainPage()
        {
            GUILayout.Label(new GUIContent(_bannerImage), GUIStyle.none);
            var versionAsset = Resources.Load<TextAsset>("VERSION");

            if (versionAsset == null)
            {
                // initial import not done yet
                return;
            }

            GUIHeader(new string(' ', 90) + "Version: " + versionAsset.text);
        
			GUIHeader(HeaderWhatsNew);
            if (GUIButtonLabel(ReleaseButtonText, ReleaseButtonTip))
                OpenReleaseNotes();
            GUIFooter();

            GUIHeader(HeaderLearning);
			if (GUIButtonLabel(DocButtonText, DocButtonTip))
                OpenDocumentation();
            GUIFooter();

			GUIHeader(HeaderLinks);
			if (GUIButtonLabel(WebsiteButtonText, WebsiteButtonTip))
                OpenLeiaMainURL();
			if (GUIButtonLabel(HelpButtonText, HelpButtonTip))
                OpenLeiaHelpURL();
            GUIFooter();
        }

        private static Vector2 _scroll;

        private void ReleaseNotesPage()
        {
            if (_white == null)
            {
                _white = new Texture2D(1, 1);
                _white.SetPixel(0, 0, Color.white);
                _white.Apply();
            }

            if (_monospace == null)
            {
                _monospace = new GUIStyle(GUIStyle.none);
                _monospace.font = Resources.Load<Font>("arial-monospaced");
                _monospace.fontSize = 10;
                _monospace.normal.background = _white;
            }

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            var text = Resources.Load<TextAsset>("RELEASE");
            _scroll = GUILayout.BeginScrollView(_scroll, false, true);
            GUILayout.Label(text.text, _monospace);
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUIFooter();
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();

            titleContent = new GUIContent();
            switch (_page)
            {
                case Page.Main: MainPage(); break;
                case Page.ReleaseNotes: ReleaseNotesPage(); break;
            }

            GUILayout.EndVertical();

            if (!_isInitialized && Event.current.type == EventType.Repaint)
                InitializeGUIContent();
        }

        private void GUIHeader ( string text ) {
            GUILayout.BeginHorizontal();
            GUILayout.Space(2);
            GUILayout.Label(text, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void GUIFooter ( ) {
            GUILayout.Space(5);
        }

        private bool GUIButtonLabel ( string button, string label ) {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            bool result = GUILayout.Button(new GUIContent(button), GUILayout.Width(150));
            GUILayout.Space(30);
            GUILayout.Label(label);
            GUILayout.EndHorizontal();
            return result;
        }

        private void InitializeGUIContent ( ) {
            var width = 500;
            var height = GUILayoutUtility.GetLastRect().height + 10f;

            position.Set(position.x, position.y, width, height);

            minSize = new Vector2(width, height);
            maxSize = new Vector2(width, height + 1);

            _isInitialized = true;
        }
    }
}