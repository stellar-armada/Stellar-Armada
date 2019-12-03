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
using System;
using UnityEngine;
using System.Collections.Generic;
namespace LeiaLoft
{
	/// <summary>
	/// Basic abstract implementation of ILeiaDevice with profile loading methods implemented
	/// and calibration saved inside unity editor/app(for builds) preferences.
	/// </summary>
	public abstract class AbstractLeiaDevice : ILeiaDevice
	{
		public const string PrefOffsetX = "LeiaLoft_UserOffsetX";
		public const string PrefOffsetY = "LeiaLoft_UserOffsetY";

		private bool _hasProfile;
		private float _systemScalingPercent;
		protected string _profileStubName;
		protected string _cachedProfileName;

		public delegate int BacklightModeChanged (int mode);

		public BacklightModeChanged backlightModeChangedDelegate;

		protected AbstractLeiaDevice()
		{
			if (CalibrationOffset == null)
			{
				CalibrationOffset = new int[2];
			}
		}

		public void SetProfileStubName(string name)
		{
			_profileStubName = name;
		}

		public abstract void SetBacklightMode(int modeId);

		public abstract void SetBacklightMode(int modeId, int delay);

		public abstract void RequestBacklightMode(int modeId);

		public abstract void RequestBacklightMode(int modeId, int delay);

		public abstract int GetBacklightMode();

		public virtual DisplayConfig GetDisplayConfig()
		{
			DisplayConfig displayConfig =  new DisplayConfig();

			displayConfig.AlignmentOffset = new XyPair<float>(0,0);
			displayConfig.DisplaySizeInMm = new XyPair<int>(0,0);
			displayConfig.DotPitchInMm = new XyPair<float>(0,0);
			displayConfig.NumViews = new XyPair<int>(4,4);
			displayConfig.PanelResolution = new XyPair<int>(1440, 2560);
			displayConfig.SystemDisparityPercent = 0.0125f;
			displayConfig.SystemDisparityPixels = 8f;
			displayConfig.ViewResolution = new XyPair<int>(360, 640);

			displayConfig.ActCoefficients = new XyPair<List<float>>(new List<float>(), new List<float>());

			displayConfig.ActCoefficients.x = new List<float>();
			displayConfig.ActCoefficients.x.Add(0.06f);
			displayConfig.ActCoefficients.x.Add(0.025f);

			displayConfig.ActCoefficients.y = new List<float>();
			displayConfig.ActCoefficients.y.Add(0.04f);
			displayConfig.ActCoefficients.y.Add(0.02f);

			return displayConfig;
		}

		public virtual string GetSensors()
		{
			return null;
		}

		public virtual int[] CalibrationOffset
		{
			get
			{
				#if UNITY_EDITOR
				var offset = new int[2];
				offset[0] = UnityEditor.EditorPrefs.GetInt(PrefOffsetX, offset[0]);
				offset[1] = UnityEditor.EditorPrefs.GetInt(PrefOffsetY, offset[1]);
				return offset;
				#else
				var offset = new int[2];
				offset[0] = UnityEngine.PlayerPrefs.GetInt(PrefOffsetX, offset[0]);
				offset[1] = UnityEngine.PlayerPrefs.GetInt(PrefOffsetY, offset[1]);
				return offset;
				#endif
			}
			set
			{
				#if UNITY_EDITOR
				UnityEditor.EditorPrefs.SetInt(PrefOffsetX, value[0]);
				UnityEditor.EditorPrefs.SetInt(PrefOffsetY, value[1]);
				#else
				UnityEngine.PlayerPrefs.SetInt(PrefOffsetX, value[0]);
				UnityEngine.PlayerPrefs.SetInt(PrefOffsetY, value[1]);
				#endif
			}
		}

		public virtual bool IsSensorsAvailable()
		{
			return false;
		}

		public virtual void CalibrateSensors()
		{
		}

		public virtual bool IsConnected()
		{
			return false;
		}
	}
}
