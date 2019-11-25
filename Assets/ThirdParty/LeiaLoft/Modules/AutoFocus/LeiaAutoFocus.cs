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

/// <summary>
/// Script that automates the LeiaCamera's focus distance
/// </summary>
namespace LeiaLoft
{
	[ExecuteInEditMode]
	public class LeiaAutoFocus : MonoBehaviour
	{
		public static readonly string[] TRACKING_MODES = {"Target", "Auto"};
		public static readonly string[] RAYCAST_TYPES = {"Point", "Sphere"};

		public static readonly int TARGET_TRACKING = 0;
		public static readonly int AUTO_TRACKING = 1;

		public static readonly int POINT_RAYCAST = 0;
		public static readonly int SPHERE_RAYCAST = 1;

		/// <summary>
		/// The target GameObject to focus on.
		/// </summary>
		public GameObject target
		{
			get
			{
				return _target;
			}
			set
			{
				_target = value;
			}
		}
	 
		/// <summary>
		/// Offset from the automatic focus tracking.
		/// </summary>
		public float focusOffset
		{
			get
			{
				return _focusOffset;
			}
			set
			{
				_focusOffset = value;
			}
		}

		/// <summary>
		/// The radius of the sphere used in raycasting.
		/// </summary>
		public float sphereRadius
		{
			get
			{
				return _sphereRadius;
			}
			set
			{
				_sphereRadius = Mathf.Max(0.01f, value);
			}
		}

		/// <summary>
		/// The tracking mode used for determining focus.
		/// </summary>
		public int trackingMode
		{
			get
			{
				return _trackingMode;
			}
			set
			{
				_trackingMode = value;
			}
		}

		/// <summary>
		/// The auto focus racast mode for determining focus.
		/// </summary>
		public string raycastType
		{
			get
			{
				return _raycastType;
			}
			set
			{
				_raycastType = value;
			}
		}

		/// <summary>
		/// The max distance the ray should check for collisions.
		/// </summary>
		public float maxFocusDistance
		{
			get
			{
				return _maxFocusDistance;
			}
			set
			{
				_maxFocusDistance = Mathf.Max(value, 0f);
			}
		}

		/// <summary>
		/// The speed at which focus will change.
		/// </summary>
		public float focusSpeed
		{
			get
			{
				return _focusSpeed;
			}
			set
			{
				_focusSpeed = Mathf.Max(Mathf.Min(value, 1.0f), 0.05f);
			}
		}

		/// <summary>
		/// Uses the Transform.LookAt function to look at the target GameObject.
		/// </summary>
		public bool lookAtTarget
		{
			get
			{
				return _lookAtTarget;
			}
			set
			{
				_lookAtTarget = value;
			}
		}
			
		[SerializeField]
		private GameObject _target;

		[SerializeField]
		private bool _lookAtTarget = false;

		[SerializeField]
		private int _trackingMode = TARGET_TRACKING;

		[SerializeField]
		private string _raycastType = RAYCAST_TYPES[POINT_RAYCAST];

		[SerializeField]
		private float _focusOffset = 0f;

		[SerializeField]
		private float _sphereRadius = 0.3f;

		[SerializeField]
		private float _maxFocusDistance = 1000f;

		[SerializeField]
		private float _focusSpeed = 1f;

		[SerializeField]
		private float _targetConvergenceDistance = 0f;

		private LeiaCamera _LeiaCam;

		public void Start()
		{
			_LeiaCam = GetComponent<LeiaCamera>();

			if(_LeiaCam == null)
			{
				return;
			}
				
			_targetConvergenceDistance = _LeiaCam.ConvergenceDistance;
		}

		public void Update()
		{
			if(_LeiaCam == null)
			{
				return;
			}

			if(trackingMode == AUTO_TRACKING)
			{
				updateAutoFocus(_maxFocusDistance);
			}

			else if(_target != null) 
			{
				updateTargetTracking();
			}

			updateConvergenceDistance();
		}

		private void updateConvergenceDistance()
		{
			_LeiaCam.ConvergenceDistance += (_targetConvergenceDistance - _LeiaCam.ConvergenceDistance) * _focusSpeed;
		}

		private void updateTargetTracking()
		{
			_targetConvergenceDistance = getPerpandicularDistance() + focusOffset; 

			if(_lookAtTarget) 
			{
				this.transform.LookAt(_target.transform);
			}
		}
			
		private void updateAutoFocus(float maxDistance)
		{
			#if UNITY_EDITOR 
			if (!Application.isPlaying) return;
			#endif

			RaycastHit hit;
			Ray ray = new Ray(this.transform.position, this.transform.forward);

			if(raycastType == RAYCAST_TYPES[SPHERE_RAYCAST])
			{
				if(Physics.SphereCast(ray, sphereRadius, out hit, maxDistance))
				{
					setFocusToPoint(hit.point);
				}
			}
			else
			{
				if(Physics.Raycast(ray, out hit,  maxDistance)) 
				{
					setFocusToPoint(hit.point);
				}
			}
		}

		private void setFocusToPoint(Vector3 target)
		{
			_targetConvergenceDistance = Vector3.Distance(this.transform.position, target) + _focusOffset;
		}
			
		private float getPerpandicularDistance()
		{
			return Vector3.Dot(_target.transform.position - this.transform.position, this.transform.forward) / this.transform.forward.magnitude;
		}
	}
}