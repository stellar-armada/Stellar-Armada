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
    [UnityEditor.CustomEditor(typeof(LeiaDisplay))]
    public class LeiaDisplayEditor : UnityEditor.Editor
    {
        private const string ProfileSelectionLabel = "Profile to use if no device is connected:";
        private const string RenderModeLabel = "Render Mode";
        private const string AntiAliasingLabel = "Anti Aliasing";
        private const string DeferredNotSupportedLabel = "(not supported in deferred rendering mode)";
        private const string EnabledParallaxWarning = "Parallax Auto Rotation checkbox will be ignored if AutoRotation is enabled in PlayerSettings";
        private const string RenderModeFieldLabel = "LeiaLoft_LeiaDisplayEditor_RenderMode";
        private const string AntiAliasingFieldLabel = "LeiaLoft_LeiaDisplayEditor_AntiAliasing";
        private const string CalibrationSquaresFieldLabel = "Show Calibration Squares";
        private const string ParallaxFieldLabel = "Parallax Auto Rotation";
        private const string AlphaBlendingLabel = "Enable Alpha Blending";
        private const string CalibrationXFieldLabel = "Calibration X";
        private const string CalibrationYFieldLabel = "Calibration Y";
        private const string ShowTilesFieldLabel = "Show Tiles";
        private const string AdaptFovXFieldLabel = "Adapt. FOV X";
        private const string AdaptFovYFieldLabel = "Adapt. FOV Y";
        private const string SwitchModeOnSceneChangeLabel = "Switch to 2D mode on scene change";

        private LeiaDisplay _controller;

        void OnEnable()
        {
            if (_controller == null)
            {
                _controller = (LeiaDisplay)target;
            }
        }

        private void ShowRenderModeControl()
        {
            _controller.StateFactory.SetDisplayConfig(_controller.GetDisplayConfig());
            var idToDisplay = _controller.StateFactory.ValidateState(_controller.DesiredLeiaStateID);
            var leiaModes = LeiaStateFactory.AvailableStateIds;
            var list = leiaModes.ToList().Beautify();
            var previousIndex = list.IndexOf(idToDisplay, ignoreCase: true);

            if (previousIndex < 0)
            {
                list.Add(idToDisplay);
                previousIndex = list.Count-1;
            }

            UndoableInputFieldUtils.PopupLabeled(index => _controller.DesiredLeiaStateID = leiaModes[index], RenderModeLabel, previousIndex, list.ToArray(), _controller.Settings);
            if(_controller.DesiredLeiaStateID != _controller.LeiaStateId) 
                _controller.LeiaStateId = _controller.DesiredLeiaStateID;
        }

        private void ShowAntialiasingDropdown()
        {
            bool deferred = false;

            #if !UNITY_5_5_OR_NEWER
            deferred = PlayerSettings.renderingPath == RenderingPath.DeferredLighting;
            #else
            var settings = UnityEditor.Rendering.EditorGraphicsSettings.
                GetTierSettings(EditorUserBuildSettings.selectedBuildTargetGroup, Graphics.activeTier);
            deferred = settings.renderingPath == RenderingPath.DeferredLighting ||
                settings.renderingPath == RenderingPath.DeferredShading;
            #endif

            if (deferred)
            {
                GUILayout.Label(DeferredNotSupportedLabel);

                if (_controller.AntiAliasing != 1)
                {
                    _controller.AntiAliasing = 1;
                }

                return;
            }

            var values = AntiAliasingHelper.Values;
            var previousIndex = values.ToList().IndexOf(_controller.AntiAliasing);
            var stringOptions = AntiAliasingHelper.NamedValues;

            UndoableInputFieldUtils.PopupLabeled(index => _controller.AntiAliasing = values[index], AntiAliasingLabel, previousIndex, stringOptions, _controller.Settings);
        }


        private void ShowDecoratorsControls()
        {
            var decorators = _controller.Decorators;

            UndoableInputFieldUtils.BoolField(() => decorators.ParallaxAutoRotation, v =>
                {
                    decorators.ParallaxAutoRotation = v;

                    if (v)
                    {
                        this.Warning(EnabledParallaxWarning);
                    }
                }

                , ParallaxFieldLabel, _controller.Settings);


            UndoableInputFieldUtils.BoolField(() => decorators.AlphaBlending, v => decorators.AlphaBlending = v, AlphaBlendingLabel, _controller.Settings);
            UndoableInputFieldUtils.BoolField(() => _controller.SwitchModeOnSceneChange, v => _controller.SwitchModeOnSceneChange = v, SwitchModeOnSceneChangeLabel, _controller.Settings);


            _controller.Decorators = decorators;
        }

        public override void OnInspectorGUI()
        {
            if (!_controller.enabled)
            {
                return;
            }

            ShowRenderModeControl();
            ShowAntialiasingDropdown();
            ShowDecoratorsControls();
        }
    }
}