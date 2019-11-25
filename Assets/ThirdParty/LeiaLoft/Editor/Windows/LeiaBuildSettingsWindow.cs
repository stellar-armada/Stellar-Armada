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
    public class LeiaBuildSettingsWindow : UnityEditor.EditorWindow
    {
        const string FixButtonText = "Fix";
        const string FixButtonTip = "[Warning] Stripping Level should be set to DISABLE to support Android builds";
        const string LogoAssetPath = "LeiaLoft/Leia-loft-logo.png";
        const string BannerAssetPath = "LeiaLoft/Leia-loft-banner.png";

        private static Texture2D _bannerImage;
#if !(UNITY_4_6 || UNITY_4_7 || UNITY_5_0)
        private static Texture2D _logoImage;
#endif

        static LeiaBuildSettingsWindow()
        {
            EditorApplication.update += Initialize;
        }

        static void Initialize()
        {
            EditorApplication.update -= Initialize;
        }

        [MenuItem ("LeiaLoft/Log Settings")]
        public static void Open()
        {
            EditorWindow.GetWindow<LeiaBuildSettingsWindow>(true);
        }

        private void OnEnable()
        {
            _bannerImage = (Texture2D) EditorGUIUtility.Load(LogoAssetPath);
#if !(UNITY_4_6 || UNITY_4_7 || UNITY_5_0)
            _logoImage = (Texture2D) EditorGUIUtility.Load(LogoAssetPath);
#endif
        }

        private void OnGUI ()
        {
            GUILayout.Label (new GUIContent (_bannerImage), GUIStyle.none);

#if !(UNITY_4_6 || UNITY_4_7 || UNITY_5_0)
            this.titleContent = new GUIContent (_logoImage);
#endif

            GUILayout.BeginVertical ();

            if (PlayerSettings.strippingLevel != StrippingLevel.Disabled) 
            {
                if (GUIButtonLabel (FixButtonText, FixButtonTip)) 
                {
                    PlayerSettings.strippingLevel = StrippingLevel.Disabled;
                }
            }

            // probably not needed anymore:
//            if (PlayerSettings.apiCompatibilityLevel != ApiCompatibilityLevel.NET_2_0) 
//            {
//                if (GUIButtonLabel ("Fix", "[Warning] ApiCompatibilityLevel should be set to NET 2.0 to support external display\n(restart Unity if you still see Errors)")) 
//                {
//                    PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0;
//                }
//            }

            //var targetGroups = (BuildTargetGroup[])System.Enum.GetValues (typeof(BuildTargetGroup));
            var targetGroups = new BuildTargetGroup[] { BuildTargetGroup.Android, BuildTargetGroup.Standalone };

            const string prefix = "LEIALOFT_LOGLEVEL_";
            foreach (var grp in targetGroups) 
            {
                GUIHeader (grp.ToString () + " log level");
                var defines = CompileDefineUtil.GetCompileDefinesWithPrefix (prefix, grp);
                LogLevel finalValue = LogLevel.Warning;

                foreach (var def in defines) 
                {
                    var enumValue = def.Substring (prefix.Length);
                    finalValue = (LogLevel)System.Enum.Parse (typeof(LogLevel), enumValue, true);
                }

                var logLevel = (LogLevel)EditorGUILayout.EnumPopup ("Log Level", finalValue);

                if (logLevel != finalValue) 
                {
                    foreach (var def in defines) 
                    {
                        CompileDefineUtil.RemoveCompileDefine (def, new BuildTargetGroup[]{grp});
                    }

                    CompileDefineUtil.AddCompileDefine(grp, prefix + logLevel.ToString().ToUpper());
                }
            }

            GUIFooter();
            GUILayout.EndVertical();

            if (Event.current.type == EventType.Repaint)
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
            GUILayout.BeginVertical();
            GUILayout.Label(label);
            bool result = GUILayout.Button(new GUIContent(button), GUILayout.Width(200));
            GUILayout.Space(20);
            GUILayout.EndVertical();
            return result;
        }

        private void InitializeGUIContent ( ) {
            var width = 500;
            var height = 250;

            position.Set(position.x, position.y, width, height);

            minSize = new Vector2(width, height);
            maxSize = new Vector2(width, height + 1);
        }
    }
}