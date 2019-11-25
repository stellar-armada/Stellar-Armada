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
using System.Collections.Generic;

namespace LeiaLoft
{
    public class AndroidLeiaDevice : AbstractLeiaDevice
    {
        private static AndroidJavaClass _leiaBacklightClass;
        private AndroidJavaObject _leiaBacklightInstance;
        private DisplayConfig _displayConfig;

        public override int[] CalibrationOffset
        {
            get
            {
                if (_leiaBacklightInstance == null)
                {
                    return base.CalibrationOffset;
                }

                int[] temp = _leiaBacklightInstance.Call<int[]> ("getXYCalibration");
                return new int[] { temp[1], temp[2] };
            }
            set
            {
                this.Warning("Setting calibration from Unity Plugin is not supported anymore - use relevant app instead.");
            }
        }

        public AndroidLeiaDevice(string stubName)
        {
            this.Debug("ctor");

            try
            {
                if (_leiaBacklightClass == null)
                {
                    _leiaBacklightClass = new AndroidJavaClass("android.leia.LeiaBacklight");
                    _leiaBacklightClass.CallStatic("start", "leia", false);
                }

                _leiaBacklightInstance = _leiaBacklightClass.GetStatic<AndroidJavaObject>("instance");

                string displayType = stubName;

                if (!string.IsNullOrEmpty(displayType))
                {
                    _profileStubName = displayType;
                    this.Trace("displayType " + displayType);
                }
                else
                {
                    this.Debug("No displayType received, using stub: " + stubName);
                }

            }
            catch(System.Exception e)
            {
                this.Error("Unable to get response from backlight service. Using default profile stub:" + stubName);
                this.Error(e.ToString());
                _profileStubName = stubName;

            }
        }

        public override void SetBacklightMode(int modeId)
        {
            if (_leiaBacklightInstance != null)
            {
                this.Trace("SetBacklightMode" + (modeId == 4 ? 2 : modeId));
                _leiaBacklightInstance.Call ("setBacklightMode", modeId == 4 ? 2 : modeId);
            }
        }

        public override void SetBacklightMode(int modeId, int delay)
        {
            if (_leiaBacklightInstance != null)
            {
                this.Trace("SetBacklightMode" + (modeId == 4 ? 2 : modeId));
                _leiaBacklightInstance.Call ("setBacklightMode", modeId == 4 ? 2 : modeId, delay);
            }
        }

        public override void RequestBacklightMode(int modeId)
        {
            
        }

        public override void RequestBacklightMode(int modeId, int delay)
        {
            
        }
        public override int GetBacklightMode()
        {
            if (_leiaBacklightInstance != null)
            {
                
                int mode =_leiaBacklightInstance.Call<int>("getBacklightMode");
                return mode;
            }

            return 2;
        }


        public override DisplayConfig GetDisplayConfig() 
        {
            try
            {
                AndroidJavaObject displayConfig = _leiaBacklightInstance.Call<AndroidJavaObject> ("getDisplayConfig");
                _displayConfig = new DisplayConfig();

                AndroidJavaObject dotPitchInMM = displayConfig.Call<AndroidJavaObject>("getDotPitchInMm");
                _displayConfig.DotPitchInMm = new XyPair<float>(dotPitchInMM.Get<AndroidJavaObject>("x").Call<float>("floatValue"),
                    dotPitchInMM.Get<AndroidJavaObject>("y").Call<float>("floatValue"));

                AndroidJavaObject panelResolution = displayConfig.Call<AndroidJavaObject>("getPanelResolution");
                _displayConfig.PanelResolution = new XyPair<int>( panelResolution.Get<AndroidJavaObject>("x").Call<int>("intValue"),
                    panelResolution.Get<AndroidJavaObject>("y").Call<int>("intValue"));

                AndroidJavaObject numViews = displayConfig.Call<AndroidJavaObject>("getNumViews");
                _displayConfig.NumViews = new XyPair<int>(numViews.Get<AndroidJavaObject>("x").Call<int>("intValue"),
                    numViews.Get<AndroidJavaObject>("y").Call<int>("intValue"));

                AndroidJavaObject alignmentOffset = displayConfig.Call<AndroidJavaObject>("getAlignmentOffset");
                _displayConfig.AlignmentOffset = new XyPair<float>(alignmentOffset.Get<AndroidJavaObject>("x").Call<float>("floatValue"),
                    alignmentOffset.Get<AndroidJavaObject>("y").Call<float>("floatValue"));

                AndroidJavaObject displaySizeInMm = displayConfig.Call<AndroidJavaObject>("getDisplaySizeInMm");
                _displayConfig.DisplaySizeInMm = new XyPair<int>( displaySizeInMm.Get<AndroidJavaObject>("x").Call<int>("intValue"),
                    displaySizeInMm.Get<AndroidJavaObject>("y").Call<int>("intValue"));

                AndroidJavaObject viewResolution = displayConfig.Call<AndroidJavaObject>("getViewResolution");
                _displayConfig.ViewResolution = new XyPair<int>(viewResolution.Get<AndroidJavaObject>("x").Call<int>("intValue"),
                    viewResolution.Get<AndroidJavaObject>("y").Call<int>("intValue"));

                AndroidJavaObject actCoefficients = displayConfig.Call<AndroidJavaObject>("getViewSharpeningCoefficients");

                float xA = actCoefficients.Get<AndroidJavaObject>("x").Call<AndroidJavaObject>("get",0).Call<float>("floatValue");
                float xB = actCoefficients.Get<AndroidJavaObject>("x").Call<AndroidJavaObject>("get",1).Call<float>("floatValue");
                float yA = actCoefficients.Get<AndroidJavaObject>("y").Call<AndroidJavaObject>("get",0).Call<float>("floatValue");
                float yB = actCoefficients.Get<AndroidJavaObject>("y").Call<AndroidJavaObject>("get",1).Call<float>("floatValue");
                Debug.Log("coefs: " + xA + " " + xB + " " + yA + " " + yB);
                _displayConfig.ActCoefficients = new XyPair<List<float>>(new List<float>(), new List<float>());

                _displayConfig.ActCoefficients.x = new List<float>();
                _displayConfig.ActCoefficients.x.Add(xA);
                _displayConfig.ActCoefficients.x.Add(xB);

                _displayConfig.ActCoefficients.y = new List<float>();
                _displayConfig.ActCoefficients.y.Add(yA);
                _displayConfig.ActCoefficients.y.Add(yB);

                _displayConfig.SystemDisparityPercent = _leiaBacklightInstance.Call<float>("getSystemDisparityPercent");
                _displayConfig.SystemDisparityPixels = _leiaBacklightInstance.Call<float>("getSystemDisparityPixels");
            }
            catch {

                _displayConfig = base.GetDisplayConfig ();

            }
                        
            return _displayConfig;
        }

        public override bool IsConnected()
        {
            if (_leiaBacklightInstance != null)
            {
                return _leiaBacklightInstance.Call<bool>("isConnected");
            }
            return false;
        }

    }
}
