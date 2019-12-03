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
using System.Linq;
using System.Collections.Generic;

namespace LeiaLoft
{
	[UnityEditor.CustomEditor(typeof(LeiaAutoFocus))]
	public class LeiaAutoFocusEditor : UnityEditor.Editor
	{
		private LeiaAutoFocus _controller;

		void OnEnable()
		{
			if (_controller == null)
			{
				_controller = (LeiaAutoFocus)target;
			}
		}

		public override void OnInspectorGUI()
		{
			if (!_controller.enabled)
			{
				return;
			}

			showFocusSmoothing();
			showFocalOffset();
			showTrackingTypes();
		}

		private void showLookAtTarget()
		{
			bool lookAt = _controller.lookAtTarget;

			UndoableInputFieldUtils.BoolField (() => lookAt,
				v => {
					_controller.lookAtTarget = v;
				}, "Look at Target");
		}

		private void showSphereRadius()
		{
			UndoableInputFieldUtils.ImmediateFloatField(() => _controller.sphereRadius, v => _controller.sphereRadius = v, "Sphere Radius", _controller);
		}

		private void showFocalOffset()
		{
			UndoableInputFieldUtils.ImmediateFloatField(() => _controller.focusOffset, v => _controller.focusOffset = v, "Focus Offset", _controller);
		}

		private void showFocusSmoothing()
		{
			UndoableInputFieldUtils.ImmediateFloatField(() => _controller.focusSpeed, v => _controller.focusSpeed = v, "Focus Speed", _controller);
		}

		private void showMaxFocusDistance()
		{
			UndoableInputFieldUtils.ImmediateFloatField(() => _controller.maxFocusDistance, v => _controller.maxFocusDistance = v, "Max Focus Distance", _controller);
		}

		private void showTrackingTypes()
		{
			var values = LeiaAutoFocus.TRACKING_MODES;
			var previousIndex = _controller.trackingMode;
			UndoableInputFieldUtils.PopupLabeled(index => _controller.trackingMode = index, "Tracking Mode", previousIndex, values, _controller);

			if(_controller.trackingMode == LeiaAutoFocus.TARGET_TRACKING)
			{
				_controller.target = (GameObject)EditorGUILayout.ObjectField("Target", _controller.target,  typeof(GameObject), true);
				showLookAtTarget();
			} 
			else if(_controller.trackingMode == LeiaAutoFocus.AUTO_TRACKING)
			{
				EditorGUILayout.BeginVertical();
				showMaxFocusDistance();
				showRaycastTypes();
				EditorGUILayout.EndVertical();
			}
		}

		private void showRaycastTypes()
		{
			var values = LeiaAutoFocus.RAYCAST_TYPES;
			var previousIndex = values.ToList().IndexOf(_controller.raycastType);

			UndoableInputFieldUtils.PopupLabeled(index => _controller.raycastType = values[index], "Raycast Type", previousIndex, values, _controller);

			if(_controller.raycastType == LeiaAutoFocus.RAYCAST_TYPES[1])
			{
				showSphereRadius();
			}
		}
	}
}