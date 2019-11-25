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
using UnityEngine.SceneManagement;

namespace LeiaLoft
{
    /// <summary>
    /// Presents single display object, has state and settings (decorators) that determine rendering mode.
    /// </summary>
    public class LeiaDisplay : Singleton<LeiaDisplay>
    {
        private const string CreateLeiaSettings = "Creating LeiaSettings gameObject";
        private const string StartParallax = "Starting parallax animation";
        private const string PeakParallax = "Parallax animation peak";
        private const string StopParallax = "Stopping parallax animation";
        private const string VersionFileName = "VERSION";
        private const string IsDirty = "IsDirty detected";

        #if LEIALOFT_BUILD_DEVICE_4K
        public const string ProfileName4K = "PARASOL_4K_4X4";
        #endif

        private LeiaSettings _settings = null;
        private bool _isDirty = false;

        private ILeiaState _leiaState;
        private LeiaStateFactory _stateFactory = new LeiaStateFactory();
        private LeiaDeviceFactory _deviceFactory = new LeiaDeviceFactory();
        private ILeiaDevice _leiaDevice;
        private static float _screenAspectRatio;
        private float _prevScreenAspectRatio = -1;

        private const int CalibratingOffsetMin = -16;
        private const int CalibratingOffsetMax = 16;

        #if UNITY_ANDROID && !UNITY_EDITOR
        private float _disparityBackup;
        private float _disparityAnimTime = 0;
        private float _disparityAnimDirection = 0;
        private const float BASELINE_ANIM_PEAK_TIME = 0.5f;
        #endif
    
        public const string HPO = "HPO";
        public const string TWO_D = "2D";

        /// <summary>
        /// Occurs when LeiaDisplay has leiaState or decorators changed.
        /// </summary>
        public event System.Action StateChanged = delegate { };

        /// <summary>
        /// Gets current leia device.
        /// </summary>
        public ILeiaDevice LeiaDevice
        {
            get
            {
                return _leiaDevice;
            }
        }

        /// <summary>
        /// Gets current leiaState factory.
        /// </summary>
        public LeiaStateFactory StateFactory
        {
            get
            {
                return _stateFactory;
            }
        }

        /// <summary>
        /// Gets current leiaDevice factory
        /// </summary>
        public LeiaDeviceFactory DeviceFactory
        {
            get
            {
                return _deviceFactory;
            }
        }

        /// <summary>
        /// Gets settings object (where all LeiaDisplay settings aggregated)
        /// </summary>
        public LeiaSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = FindObjectOfType<LeiaSettings>();

                    if (_settings == null)
                    {
                        this.Debug(CreateLeiaSettings);
                        _settings = new GameObject(LeiaSettings.GameObjectName).AddComponent<LeiaSettings>();
                    }

                    _settings.gameObject.hideFlags = HideFlags.HideInHierarchy;
                }

                return _settings;
            }
        }

        /// <summary>
        /// Gives access to AntiaAliasing setting, sets dirty flag when changed
        /// </summary>
        public int AntiAliasing
        {
            get
            {
                return Settings.AntiAliasing;
            }
            set
            {
                if (Settings.AntiAliasing != value)
                {
                    this.Trace(string.Format("Set AntiAliasing: {0}", value));
                    Settings.AntiAliasing = value;
                    _isDirty = true;
                }
            }
        }

        /// <summary>
        /// Gives access to ProfileStubName setting, creates new factroy (based on new profile), sets dirty flag when changed
        /// </summary>
        public string ProfileStubName
        {
            get
            {
                return Settings.ProfileStubName;
            }
            set
            {
                this.Trace(string.Format("Set ProfileStubName: {0}", value));
                Settings.ProfileStubName = value;
                _isDirty = true;

                if (Application.isPlaying)
                {
                    _leiaDevice.SetProfileStubName(value);
                    _stateFactory.SetDisplayConfig(GetDisplayConfig());
                }
            }
        }

        /// <summary>
        /// Gets or sets the leia state id, when changing, sets dirty flag.
        /// </summary>
        public string LeiaStateId
        {
            get
            {
                return Settings.LeiaStateId;
            }
            set
            {
                this.Trace(string.Format("Set LeiaStateId: {0}", value));
                Settings.LeiaStateId = value;
                _isDirty = true;
            }
        }

        public string DesiredLeiaStateID
        {
            get
            {
                return Settings.DesiredLeiaStateID;
            }
            set
            {

                if(value != Settings.DesiredLeiaStateID && value.Equals(HPO)) {
                    if(_leiaDevice != null)
                        _leiaDevice.SetBacklightMode(3);
                    
                    Settings.DesiredLeiaStateID = value;
                }

                if(value != Settings.DesiredLeiaStateID && !value.Equals(HPO)) {
                    if(_leiaDevice != null)
                        _leiaDevice.SetBacklightMode(2);
                    
                    Settings.DesiredLeiaStateID = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the decorators, when changed - recreates LeiaStateFactory and applies leiaState to views
        /// </summary>
        /// <value>The decorators.</value>
        public LeiaStateDecorators Decorators
        {
            get
            {
                return Settings.Decorators;
            }
            set
            {
                if (Settings.Decorators.Equals(value)) return;
                Settings.Decorators = value;
                this.Trace(string.Format("Set Decorators: {0}", Settings.Decorators.ToString()));

                if (Application.isPlaying)
                {
                    _stateFactory.SetDisplayConfig(GetDisplayConfig());
                    UpdateLeiaState();
                }
            }
        }

        public bool SwitchModeOnSceneChange
        {
            get
            {
                return Settings.SwitchModeOnSceneChange;
            }
            set
            {
                Settings.SwitchModeOnSceneChange = value;
            }
        }



        private void LogVersionAndGeneralInfo()
        {
            var version = Resources.Load<TextAsset>(VersionFileName);

            if (version == null)
            {
                return;
            }

            this.Debug("Version: " + version.text);
            this.Debug("Unity Version: " + Application.unityVersion);
            this.Debug("Unity Platform: " + Application.platform.ToString());
            this.Debug("Is Editor: " + Application.isEditor);
            this.Debug("Is Playing: " + Application.isPlaying);
        }

        public int[] CalibrationOffset
        {
            get
            {
                return _leiaDevice.CalibrationOffset;
            }
            set
            {
                this.Trace(string.Format("Set CalibrationOffset: {0}", value));

                if (Application.isPlaying)
                {
                    value[1] = Mathf.Clamp(value[1], CalibratingOffsetMin, CalibratingOffsetMax);
                    value[0] = Mathf.Clamp(value[0], CalibratingOffsetMin, CalibratingOffsetMax);
                    _leiaDevice.CalibrationOffset = value;
                    _isDirty = true;
                }
            }
        }

        private void OnResume()
        {
            if(_leiaDevice == null) return;
            
            if(DesiredLeiaStateID == HPO && _leiaDevice.GetBacklightMode() != 3) {
                _leiaDevice.SetBacklightMode(3);
            }
        }

        private void OnPause()
        {
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
            {
                OnResume();
            }
            else
            {
                OnPause();
            }
        }

        #if UNITY_ANDROID && (UNITY_4_6 || UNITY_4_7) && !UNITY_EDITOR
        private void OnApplicationPause(bool pauseStatus)
        {
            this.Debug("OnApplicationPause");
            UpdateNavigationBarVisibility();
        }

        /// <summary>
        /// Attempt to hide navbar and ensure full screen is used
        /// </summary>
        private void UpdateNavigationBarVisibility()
        {
            this.Debug("UpdateNavigationBarVisibility");
            var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
            .GetStatic<AndroidJavaObject>("currentActivity");

            activity.Call("runOnUiThread",
            new AndroidJavaRunnable(() =>
            activity.Call<AndroidJavaObject>("getWindow")
            .Call<AndroidJavaObject>("getDecorView")
            .Call("setSystemUiVisibility", 4102)));
        }

        private void Start()
        {
            this.Debug("Start");
            UpdateNavigationBarVisibility();
        }
        #endif

        private void OnEnable()
        {
            

            LogVersionAndGeneralInfo();
            this.Debug("OnEnable");

#if UNITY_ANDROID && !UNITY_EDITOR
            if (!GetComponent("AndroidLeiaDeviceBehaviour"))
            {
                gameObject.AddComponent(
                    System.Reflection.Assembly.GetExecutingAssembly()
                        .GetType("LeiaLoft.AndroidLeiaDeviceBehaviour"));
            }
#endif

            this.Trace("Settings: " + Settings);

            var camera = GetComponent<Camera>();
            if (camera == null)
            {
                camera = gameObject.AddComponent<Camera>();
            }
            camera.cullingMask = 0;
            camera.depth = 100;
            camera.clearFlags = CameraClearFlags.Nothing;

            UpdateDevice();
            SceneManager.activeSceneChanged += onSceneChange;
        }

        /// <summary>
        /// Gets new device from deviceFactory (providing profile stub name in case if device not available).
        /// Gets profile from new device, sends it to leiaStateFactory.
        /// Gets default LeiaStateId if LeiaStateId is empty.
        /// Applies lState.
        /// </summary>
        public void UpdateDevice()
        {
            this.Debug("UpdateDevice");
            _leiaDevice = _deviceFactory.GetDevice(ProfileStubName);
            _stateFactory.SetDisplayConfig(GetDisplayConfig());

            if (string.IsNullOrEmpty(LeiaStateId))
            {
                LeiaStateId = "GRATING_4x4";
            }
                
            RequestLeiaStateUpdate();
        }

        private void OnDisable()
        {
            this.Debug("OnDisable");
            if (_leiaState != null)
            {
                _leiaState.Release();
            }
            SceneManager.activeSceneChanged -= onSceneChange;
        }

        #if UNITY_ANDROID && !UNITY_EDITOR
        /// <summary>
        /// If device is rotated, animates disparity scaling to 0, sets proper parallax orientation
        /// and then animates disparity scaling back
        /// </summary>
        private void ProcessParallaxRotation()
        {
            if (Decorators.ParallaxAutoRotation &&
                (Decorators.ShouldParallaxBePortrait !=
                 (Input.deviceOrientation == DeviceOrientation.Portrait ||
                  Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)) &&
                _disparityAnimDirection == 0)
            {
                this.Trace(StartParallax);
                _disparityAnimDirection = 1;
                _disparityBackup = LeiaCamera.Instance.BaselineScaling;
                _disparityAnimTime = Time.realtimeSinceStartup;
            }

            float timeDifference = Time.realtimeSinceStartup - _disparityAnimTime + 0.001f;

            if (_disparityAnimDirection == 1)
            {
                if (timeDifference < BASELINE_ANIM_PEAK_TIME)
                {
                    LeiaCamera.Instance.BaselineScaling = _disparityBackup * (1.0f - timeDifference/BASELINE_ANIM_PEAK_TIME);
                }
                else
                {
                    this.Trace(PeakParallax);
                    _disparityAnimTime = Time.realtimeSinceStartup;
                    _disparityAnimDirection = -1;
                    var tmpDecorator = Decorators;
                    tmpDecorator.ShouldParallaxBePortrait =
                    (Input.deviceOrientation == DeviceOrientation.Portrait ||
                    Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown);
                    Decorators = tmpDecorator;
                    _isDirty = true;
                }
            }
            else if (_disparityAnimDirection == -1)
            {
                if (timeDifference < BASELINE_ANIM_PEAK_TIME)
                {
                    LeiaCamera.Instance.BaselineScaling = _disparityBackup * (timeDifference/BASELINE_ANIM_PEAK_TIME);
                }
                else
                {
                    this.Trace(StopParallax);
                    LeiaCamera.Instance.BaselineScaling = _disparityBackup;
                    _disparityAnimDirection = 0;
                }
            }
        }
        #endif

        /// <summary>
        /// Render in Play mode, check` dirty flag and (re)Set leiaState
        /// </summary>
        private void Update()
        {
            _screenAspectRatio = Screen.width / Screen.height;
            if (_prevScreenAspectRatio != _screenAspectRatio)
            {
                _prevScreenAspectRatio = _screenAspectRatio;
                _isDirty = true;
            }

            #if UNITY_ANDROID && !UNITY_EDITOR
            ProcessParallaxRotation();
            #else
            #endif

            if (_isDirty)
            {
                this.Debug("IsDirty detected");
                DisplayConfig displayConfig = GetDisplayConfig();
                displayConfig.UserOrientationIsLandscape = Screen.width > Screen.height;
                _stateFactory.SetDisplayConfig(displayConfig);
                RequestLeiaStateUpdate();
                _isDirty = false;
            }
        }

        /// <summary>
        /// Use leiaState to render final picture
        /// </summary>
        public void RenderImage(LeiaCamera camera)
        {
            if (enabled)
            {
                _leiaState.DrawImage(camera, Decorators);
            }
        }

        /// <summary>
        /// Requests new state from current LeiaStateFactory, switches backlight,
        /// updates texture, raises StateChanged event
        /// </summary>
        private void RequestLeiaStateUpdate()
        {
            if (_leiaState != null)
            {
                _leiaState.Release();
            }

            _leiaState = _stateFactory.GetState(LeiaStateId);

            if(_leiaDevice.GetBacklightMode() == 2 && !LeiaStateId.Equals(HPO))
            {
                UpdateLeiaState();
            }
            else if(_leiaDevice.GetBacklightMode() == 3 && LeiaStateId.Equals(HPO))
            {
                UpdateLeiaState();
            }
            else
            {
                UpdateLeiaState();
                _leiaDevice.SetBacklightMode(_leiaState.GetBacklightMode());
            }

        }
            

        private void UpdateLeiaState() 
        {

            _leiaState.UpdateState(Decorators, LeiaDevice);

            if (StateChanged != null)
                StateChanged();
        }



        public void BacklightModeChanged(string mode)
        {
            if(mode.Equals(TWO_D))
            {
                if(!LeiaStateId.Equals(TWO_D))
                    LeiaStateId = TWO_D;
            }
            else if(mode.Equals("3D"))
            {
                if(!LeiaStateId.Equals(HPO)) 
                    LeiaStateId = HPO;
            }
        }

        public DisplayConfig GetDisplayConfig()
        {
            DisplayConfig displayConfig;

            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                displayConfig = _deviceFactory.GetDevice(ProfileStubName).GetDisplayConfig();
                displayConfig.UserOrientationIsLandscape = Screen.width > Screen.height;
                return displayConfig;
            }
            #endif

            displayConfig = _leiaDevice.GetDisplayConfig();
            displayConfig.UserOrientationIsLandscape = Screen.width > Screen.height;
            return displayConfig;
        }

        public bool IsConnected()
        {
            return LeiaDevice.IsConnected();
        }

        /// <summary>
        /// Updates the views by applying leiaState UpdateViews method and sets renderTextures from texturePool.
        /// </summary>
        public void UpdateViews(LeiaCamera camera)
        {
            if (enabled)
            {
                this.Debug("UpdateViews");
                _leiaState.UpdateViews(camera);
            }
        }

        private void onSceneChange(Scene scene, Scene scene2)
        {
            if(!SwitchModeOnSceneChange) return;
                bool containsLeia;

            GameObject[] gameObjects = scene2.GetRootGameObjects();
            LeiaDisplay leiaDisp = null;
            for(int i = 0; i < gameObjects.Length; i++) 
            {
                leiaDisp = gameObjects[i].GetComponent<LeiaDisplay>();
                if(leiaDisp != null) break;
            }
            containsLeia = (leiaDisp != null);

            if(!containsLeia) 
            {
                LeiaDevice.SetBacklightMode(2, 1500);
            }
        }

    }
}
