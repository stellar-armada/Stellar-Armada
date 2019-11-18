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
using System.Collections.Generic;
using UnityEngine;

namespace LeiaLoft
{
    /// <summary>
    /// ILeiaState implementation for Square-type displays
    /// </summary>
    public class SquareLeiaStateTemplate : AbstractLeiaStateTemplate
    {
		public const string OpaqueShaderName = "LeiaLoft/LeiaLoft_Square";
		public const string OpaqueShaderNameLimitedViews = "LeiaLoft/LeiaLoft_Square_Limited";
        public const string TransparentShaderName = "LeiaLoft/LeiaLoft_Square_Blending";
        public const string TransparentShaderNameLimitedViews = "LeiaLoft/LeiaLoft_Square_Limited_Blending";
        
        private LeiaViewSharpening _viewSharpening;

		public SquareLeiaStateTemplate(DisplayConfig displayConfig) : base(displayConfig)
        {
            // this method was left blank intentionally
        }

        public override void SetViewCount(int viewsWide, int viewsHigh)
        {
            _material = null;
            base.SetViewCount(viewsWide, viewsHigh);
        }

        protected override Material CreateMaterial(bool alphaBlending)
        {
            if (_shaderName == null)
            {
                if (_viewsHigh * _viewsWide <= 4)
                {
                    SetShaderName(OpaqueShaderNameLimitedViews, TransparentShaderNameLimitedViews);
                }
                else
                {
                    SetShaderName(OpaqueShaderName, TransparentShaderName);
                }
            }

            return base.CreateMaterial(alphaBlending);
        }

        public override void DrawImage(LeiaCamera camera, LeiaStateDecorators decorators)
        {
            if (_viewsHigh * _viewsWide <= 4 && !LeiaDisplay.InstanceIsNull)
            {
                if (LeiaDisplay.Instance.GetComponent<LeiaViewSharpening>()==null)
                {
                    _viewSharpening = LeiaDisplay.Instance.gameObject.AddComponent<LeiaViewSharpening>();
                }
            }
            base.DrawImage(camera, decorators);
        }

        public override void GetFrameBufferSize(out int width, out int height)
        {
			var tileWidth = _displayConfig.UserViewResolution.x;
			var tileHeight = _displayConfig.UserViewResolution.y;
            width = (int)(_viewsWide * tileWidth);
            height = (int)(_viewsHigh * tileHeight);
        }

        public override void GetTileSize(out int tileWidth, out int tileHeight)
        {
			tileWidth = _displayConfig.UserViewResolution.x;
			tileHeight =_displayConfig.UserViewResolution.y;
        }

        public override void UpdateViews(LeiaCamera leiaCamera)
        {
			base.UpdateViews(leiaCamera);
			var calculated = new CameraCalculatedParams(leiaCamera, _displayConfig);
			var near = Mathf.Max(1.0e-5f, leiaCamera.NearClipPlane);
			var far = Mathf.Max(near, leiaCamera.FarClipPlane);
            var halfDeltaX = calculated.ScreenHalfWidth;
            var halfDeltaY = calculated.ScreenHalfHeight;
            var deltaZ = far - near;

            Matrix4x4 m = Matrix4x4.zero;

            float posx, posy;

            for (int ny = 0; ny < _viewsHigh; ny++)
            {
                for (int nx = 0; nx < _viewsWide; nx++)
                {
                    var viewId = ny * _viewsWide + nx;
					var view = leiaCamera.GetView(viewId);

                    if (view.IsCameraNull)
                        continue;

					if(leiaCamera.Camera.orthographic) {
						float halfSizeX  = leiaCamera.Camera.orthographicSize * _displayConfig.UserAspectRatio;
						float halfSizeY  = leiaCamera.Camera.orthographicSize;
						int tileWidth, tileHeight;
						GetTileSize (out tileWidth, out tileHeight);

						float baseline = 2.0f * halfSizeX *  leiaCamera.BaselineScaling * _displayConfig.SystemDisparityPixels * leiaCamera.ConvergenceDistance / tileWidth;

						posx = GetEmissionX (nx, ny) * baseline + leiaCamera.CameraShift.x;
						posy = GetEmissionY (nx, ny) * baseline + leiaCamera.CameraShift.y;

                  			// row 0
                  			m [0, 0] = 1.0f / halfSizeX;
                  			m [0, 1] = 0.0f;
						m [0, 2] = -posx / (halfSizeX * leiaCamera.ConvergenceDistance);
                  			m [0, 3] = 0.0f;

                  			// row 1
                  			m [1, 0] = 0.0f;
                  			m [1, 1] = 1.0f / halfSizeY;
						m [1, 2] = -posy / (halfSizeY * leiaCamera.ConvergenceDistance);
                  			m [1, 3] = 0.0f;

                  			// row 2
                  			m [2, 0] = 0.0f;
                  			m [2, 1] = 0.0f;
                  			m [2, 2] = -2.0f / deltaZ;
                  			m [2, 3] = -(far + near) / deltaZ;

                  			// row 3
                  			m [3, 0] = 0.0f;
                  			m [3, 1] = 0.0f;
                  			m [3, 2] = 0.0f;
                  			m [3, 3] = 1.0f;
                    } else { // perspective
						posx = calculated.EmissionRescalingFactor * (GetEmissionX(nx, ny) + leiaCamera.CameraShift.x);
						posy = calculated.EmissionRescalingFactor * (GetEmissionY(nx, ny) + leiaCamera.CameraShift.y);

                        // row 0
						m[0, 0] = leiaCamera.ConvergenceDistance / halfDeltaX;
                        m[0, 1] = 0.0f;
  	                    m[0, 2] = -posx / halfDeltaX;
                        m[0, 3] = 0.0f;

                        // row 1
                        m[1, 0] = 0.0f;
						m[1, 1] = leiaCamera.ConvergenceDistance / halfDeltaY;
  	                    m[1, 2] = -posy / halfDeltaY;
                        m[1, 3] = 0.0f;

                        // row 2
                        m[2, 0] = 0.0f;
                        m[2, 1] = 0.0f;
                        m[2, 2] = -(far + near) / deltaZ;
                        m[2, 3] = -2.0f * far * near / deltaZ;

                        // row 3
                        m[3, 0] = 0.0f;
                        m[3, 1] = 0.0f;
                        m[3, 2] = -1.0f;
                        m[3, 3] = 0.0f;
					}

                    view.Position = new Vector3(posx, posy, 0);
                    view.Matrix = m;
                    view.NearClipPlane = near;
                    view.FarClipPlane = far;
                }
            }
        }

        public override void Release()
        {
            this.Debug("Release()");

            if (_viewSharpening != null)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(_viewSharpening);
                }
            }

            base.Release();
        }
    }
}
