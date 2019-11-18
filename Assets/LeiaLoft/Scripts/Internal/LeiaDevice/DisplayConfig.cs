using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LeiaLoft
{
	public class DisplayConfig
	{

		private XyPair<float> _dotPitchInMM;
		private XyPair<int> _panelResolution;
		private XyPair<int> _numViews;
		private XyPair<float> _alignmentOffset;
		private XyPair<List<float>> _actCoefficients;
		private XyPair<int> _viewResolution;
		private XyPair<int> _displaySizeInMm;

		private float _systemDisparityPercent;
		private float _systemDisparityPixels;

		private XyPair<float> _userDotPitchInMM;
		private XyPair<int> _userPanelResolution;
		private XyPair<int> _userNumViews;
		private XyPair<float> _userAlignmentOffset;
		private XyPair<List<float>> _userActCoefficients;
		private XyPair<int> _userViewResolution;
		private XyPair<int> _userDisplaySizeInMm;
		
		private float _userAspectRatio;
		private bool _userOrientationIsLandscape;

        public static readonly string DefaultRenderMode = "HPO";

        private List<string> _renderModes = new List<string>()
        {
            "HPO",
            "2D"
        };
		public XyPair<float> DotPitchInMm
		{
			get
			{
				return _dotPitchInMM;
			}
			set
			{
				_dotPitchInMM = value;
			}
		}

		public XyPair<int> PanelResolution
		{
			get
			{
				return _panelResolution;
			}
			set
			{
				_panelResolution = value;
			}
		}

		public XyPair<int> NumViews 
		{
			get
			{
				return _numViews;
			}
			set
			{
				_numViews = value;
			}
		}

		public XyPair<float> AlignmentOffset
		{
			get
			{
				return _alignmentOffset;
			}
			set
			{
				_alignmentOffset = value;
			}
		}

		public XyPair<List<float>> ActCoefficients
		{
			get
			{
				return _actCoefficients;
			}
			set
			{
				_actCoefficients = value;
			}
		}

		public float SystemDisparityPercent
		{
			get
			{
				return _systemDisparityPercent;
			}
			set
			{
				_systemDisparityPercent = value;
			}
		}

		public float SystemDisparityPixels
		{
			get
			{
                return _systemDisparityPixels;
			}
			set
			{
				_systemDisparityPixels = value;
			}
		}

		public XyPair<int> DisplaySizeInMm
		{
			get
			{
				return _displaySizeInMm;
			}
			set
			{
				_displaySizeInMm = value;
			}
		}

		public XyPair<int> ViewResolution 
		{
			get
			{
				return _viewResolution;
			}
			set
			{
				_viewResolution = value;
			}
		}

        public List<string> RenderModes
        {
            get
            {
                return _renderModes;    
            }
            set
            {
                _renderModes = value;    
            }
        }

		public XyPair<List<float>> GetActKernel() 
		{
			return null;
		}

		private List<float> actCoefficientsToKernel(float a, float b) 
		{
			return null;
		}

		/// <summary>
		///	The User values respect the current display orientation.
		/// </summary>
		/// <value>The User values.</value>

		public XyPair<float> UserDotPitchInMM
		{
			get
			{
				return _userDotPitchInMM;
			}
			// set by UserOrientationIsLandscape
			/*set
			{
				_userDotPitchInMM = value;
			}*/
		}

		public XyPair<int> UserPanelResolution 
		{
			get
			{
				return _userPanelResolution;
			}
			// set by UserOrientationIsLandscape
			/*set
			{
				_userPanelResolution = value;
			}*/
		}

		public XyPair<int> UserNumViews 
		{
			get
			{
				return _userNumViews;
			}
			// set by UserOrientationIsLandscape
			/*set
			{
				_userNumViews = value;
			}*/
		}

		public XyPair<float> UserAlignmentOffset
		{
			get
			{
				return _userAlignmentOffset;
			}
			// set by UserOrientationIsLandscape
			/*set
			{
				_userAlignmentOffset = value;
			}*/
		}

		public XyPair<List<float>> UserActCoefficients
		{
			get
			{
				return _userActCoefficients;
			}
			// set by UserOrientationIsLandscape
			/*set
			{
				_userActCoefficients = value;
			}*/
		}

		public XyPair<int> UserViewResolution 
		{
			get
			{
				return _userViewResolution;
			}
			// set by UserOrientationIsLandscape
			/*set
			{
				_userViewResolution = value;
			}*/
		}

		public XyPair<int> UserDisplaySizeInMm 
		{
			get
			{
				return _userDisplaySizeInMm;
			}
			// set by UserOrientationIsLandscape
			/*set
			{
				_userDisplaySizeInMm = value;
			}*/
		}

		public float UserAspectRatio
		{
			get
			{
				return _userAspectRatio;
			}
			// set by UserOrientationIsLandscape
			/*set
			{
				_userAspectRatio = value;
			}*/
		}

		public bool UserOrientationIsLandscape
		{
			get
			{
				return _userOrientationIsLandscape;
			}
			set
			{
				_userOrientationIsLandscape = value;

				_userActCoefficients = new XyPair<List<float>>(new List<float>(), new List<float>());
				_userActCoefficients.x = new List<float>();
				_userActCoefficients.y = new List<float>();

				if (_userOrientationIsLandscape == _panelResolution.x > _panelResolution.y) {
					_userDotPitchInMM		= new XyPair<float> (_dotPitchInMM.x, _dotPitchInMM.y);
					_userPanelResolution	= new XyPair<int>(_panelResolution.x, _panelResolution.y);
					_userNumViews			= new XyPair<int> (_numViews.x, _numViews.y);
					_userAlignmentOffset	= new XyPair<float> (_alignmentOffset.x, _alignmentOffset.y);
					_userActCoefficients.x.Add(_actCoefficients.x[0]);
					_userActCoefficients.x.Add(_actCoefficients.x[1]);
					_userActCoefficients.y.Add(_actCoefficients.y[0]);
					_userActCoefficients.y.Add(_actCoefficients.y[1]);
					_userViewResolution		= new XyPair<int> (_viewResolution.x, _viewResolution.y);
					_userDisplaySizeInMm	= new XyPair<int> (_displaySizeInMm.x, _displaySizeInMm.y);
				} else {
					_userPanelResolution	= new XyPair<int>(_panelResolution.y, _panelResolution.x);
					_userViewResolution		= new XyPair<int> (_viewResolution.y, _viewResolution.x);
					_userNumViews			= new XyPair<int> (_numViews.y, _numViews.x);
				}
				_userAspectRatio = (float)_userPanelResolution.x / (float)_userPanelResolution.y;
			}
		}
	}

	public class XyPair<T> 
	{
		public T x;

		public T y;

		public XyPair(T x, T y) {
			this.x = x;
			this.y = y;
		}
	}

}