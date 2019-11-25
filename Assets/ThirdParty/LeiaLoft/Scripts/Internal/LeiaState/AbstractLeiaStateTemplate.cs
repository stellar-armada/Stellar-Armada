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

namespace LeiaLoft
{
    /// <summary>
    /// ILeiaState implementation of common methods (independent of display type)
    /// </summary>
    public abstract class AbstractLeiaStateTemplate : ILeiaState
    {
        private const string SeparateTilesNotSupported = "RenderToSeparateTiles is not supporting more than 16 LeiaViews.";

        protected DisplayConfig _displayConfig;
        protected int _viewsWide;
        protected int _viewsHigh;
        protected float _deltaView;
        protected int _backlightMode;
        protected Material _material;
        protected string _shaderName;
        protected string _transparentShaderName;
        private Vector2[] _emissionPattern;

        public AbstractLeiaStateTemplate(DisplayConfig displayConfig)
        {
            _displayConfig = displayConfig;

            this.Debug("ctor()");
        }

        public virtual void SetViewCount(int viewsWide, int viewsHigh)
        {
            this.Debug(string.Format("SetViewCount( {0}, {1})", viewsWide, viewsHigh));
            _viewsWide = viewsWide;
            _viewsHigh = viewsHigh;
        }

        public void SetBacklightMode(int modeId)
        {
            _backlightMode = modeId;
        }

        public void SetShaderName(string shaderName, string transparentShaderName)
        {
            this.Debug(string.Format("SetShaderName( {0}, {1})", shaderName, transparentShaderName));
            _shaderName = shaderName;
            _transparentShaderName = transparentShaderName;
        }

        public abstract void GetFrameBufferSize(out int width, out int height);

        public abstract void GetTileSize(out int width, out int height);

        protected virtual Material CreateMaterial(bool alphaBlending)
        {
            var shaderName = alphaBlending ? _transparentShaderName : _shaderName;
            return new Material(Shader.Find(shaderName));
        }

        public virtual void DrawImage(LeiaCamera camera, LeiaStateDecorators decorators)
        {
            if (_material == null)
            {
                this.Trace("Creating material");
                _material = CreateMaterial(decorators.AlphaBlending);
            }

            _material.SetFloat("_viewRectX",camera.Camera.rect.x);
            _material.SetFloat("_viewRectY",camera.Camera.rect.y);
            _material.SetFloat("_viewRectW",camera.Camera.rect.width);
            _material.SetFloat("_viewRectH",camera.Camera.rect.height);

            if (_viewsHigh * _viewsWide > 16)
            {
                throw new NotSupportedException(SeparateTilesNotSupported);
            }

            for (int i = 0; i < camera.GetViewCount(); i++)
            {
                _material.SetTexture("_texture_" + i, camera.GetView(i).TargetTexture);
            }

            DrawQuad(decorators);
        }

        private void DrawQuad(LeiaStateDecorators decorators)
        {
            GL.PushMatrix();
            GL.LoadOrtho();
            _material.SetPass(0);
            GL.Begin(GL.QUADS);

            int o = 1;
            int z = 0;

            if (decorators.ParallaxOrientation.IsInv())
            {
                o = 0;
                z = 1;
            }

            GL.TexCoord2(z, z); GL.Vertex3(0, 0, 0);
            GL.TexCoord2(z, o); GL.Vertex3(0, 1, 0);
            GL.TexCoord2(o, o); GL.Vertex3(1, 1, 0);
            GL.TexCoord2(o, z); GL.Vertex3(1, 0, 0);

            GL.End();
            GL.PopMatrix();
        }

        protected virtual int YOffsetWhenInverted()
        {
            return 0;
        }

        protected virtual int XOffsetWhenInverted()
        {
            return 0;
        }

        private void RespectOrientation(LeiaStateDecorators decorators)
        {
            if (_viewsWide == _viewsHigh)
            {
                return;
            }

            var wide = _viewsWide > _viewsHigh;

            if (decorators.ParallaxOrientation.IsLandscape() != wide)
            {
                var tmp = _viewsWide;
                _viewsWide = _viewsHigh;
                _viewsHigh = tmp;
            }
        }

        private void UpdateEmissionPattern(LeiaStateDecorators decorators)
        {
            _emissionPattern = new Vector2[_viewsWide * _viewsHigh];
            float offsetX = -0.5f * (_viewsWide - 1.0f);
            float offsetY = -0.5f * (_viewsHigh - 1.0f);
            decorators.AdaptFOV.x = Mathf.Max(-_viewsWide + 1.0f, Mathf.Min(_viewsWide - 1.0f, decorators.AdaptFOV.x));

                for (int ny = 0; ny < _viewsHigh; ny++)
                {
                    for (int nx = 0; nx < _viewsWide; nx++)
                    {
                        float nxf = nx;
                        float nyf = ny;
                        _emissionPattern[nx + ny * _viewsWide].x = (offsetX + nxf);
                        _emissionPattern[nx + ny * _viewsWide].y = (offsetY + nyf);
                    }
                }

                if (decorators.AdaptFOV.x > 0.0f)
                {
                    for (int ny = 0; ny < _viewsHigh; ny++)
                    {
                        for (int nx = 0; nx < decorators.AdaptFOV.x; nx++)
                        {
                            float nxf = nx + _viewsWide;
                            float nyf = ny;
                            _emissionPattern[nx + ny * _viewsWide].x = (offsetX + nxf);
                            _emissionPattern[nx + ny * _viewsWide].y = (offsetY + nyf);
                        }
                    }
                }
                else if (decorators.AdaptFOV.x < 0.0f)
                {
                    for (int ny = 0; ny < _viewsHigh; ny++)
                    {
                        int nx0 = (int)(_viewsWide + decorators.AdaptFOV.x);

                        for (int nx = nx0; nx < _viewsWide; nx++)
                        {
                            float nxf = nx - _viewsWide;
                            float nyf = ny;
                            _emissionPattern[nx + ny * _viewsWide].x = (offsetX + nxf);
                            _emissionPattern[nx + ny * _viewsWide].y = (offsetY + nyf);
                        }
                    }
                }
        }

		public virtual void UpdateState(LeiaStateDecorators decorators, ILeiaDevice device)
        {
            this.Debug("UpdateState");
            if (_material == null)
            {
                _material = CreateMaterial(decorators.AlphaBlending);
            }

            RespectOrientation(decorators);
            UpdateEmissionPattern(decorators);
            var shaderParams = new ShaderFloatParams();

            shaderParams._width = _displayConfig.UserPanelResolution.x;
            shaderParams._height = _displayConfig.UserPanelResolution.y;
            shaderParams._viewResX = _displayConfig.UserViewResolution.x;
            shaderParams._viewResY = _displayConfig.UserViewResolution.y;

            var offset = device.CalibrationOffset;
            shaderParams._offsetX = offset[0] + (decorators.ParallaxOrientation.IsInv() ? XOffsetWhenInverted() : 0);
            shaderParams._offsetY = offset[1] + (decorators.ParallaxOrientation.IsInv() ? YOffsetWhenInverted() : 0);

            shaderParams._viewsX = _viewsWide;
            shaderParams._viewsY = _viewsHigh;

            shaderParams._orientation = decorators.ParallaxOrientation.IsLandscape() ? 1 : 0;
            shaderParams._adaptFOVx = decorators.AdaptFOV.x;
            shaderParams._adaptFOVy = decorators.AdaptFOV.y;
            shaderParams._enableSwizzledRendering = 1;
            shaderParams._enableHoloRendering = 1;
            shaderParams._enableSuperSampling = 0;
            shaderParams._separateTiles = 1;


            var is2d = shaderParams._viewsY == 1 && shaderParams._viewsX == 1;

            if (decorators.ShowTiles || is2d)
            {
                shaderParams._enableSwizzledRendering = 0;
                shaderParams._enableHoloRendering = 0;
            }

            shaderParams._showCalibrationSquares = decorators.ShowCalibration ? 1 : 0;
            shaderParams.ApplyTo(_material);
        }

        public virtual void UpdateViews(LeiaCamera leiaCamera)
        {
            this.Debug("UpdateViews");
			leiaCamera.SetViewCount(_viewsWide * _viewsHigh);

            int width, height;
            GetTileSize(out width, out height);

            for (int ny = 0; ny < _viewsHigh; ny++)
            {
                for (int nx = 0; nx < _viewsWide; nx++)
                {
                    int viewId = ny * _viewsWide + nx;
					var view = leiaCamera.GetView(viewId);

                    if (view.IsCameraNull)
                        continue;

                    view.SetTextureParams(width, height);
                }
            }
        }

        public virtual int GetViewsCount()
        {
            return _viewsWide * _viewsHigh;
        }

        public int GetBacklightMode()
        {
            return _backlightMode;
        }

        protected float GetEmissionX(int nx, int ny)
        {
            return _emissionPattern[nx + ny * _viewsWide].x;
        }

        protected float GetEmissionY(int nx, int ny)
        {
            return _emissionPattern[nx + ny * _viewsWide].y;
        }

        public virtual void Release()
        {
            this.Debug("Release()");

            if (_material != null)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(_material);
                }
                else
                {
                    GameObject.DestroyImmediate(_material);
                }
            }
        }

    }
}
