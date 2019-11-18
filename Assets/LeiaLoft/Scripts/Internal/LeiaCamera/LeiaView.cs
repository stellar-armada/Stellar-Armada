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

namespace LeiaLoft
{
    /// <summary>
    /// Wrapper around child LeiaCamera view camera.
    /// </summary>
    public class LeiaView
    {
        public const string ENABLED_NAME = "LeiaView";
        public const string DISABLED_NAME = "Disabled_LeiaView";

        private Camera _camera;

        public bool IsCameraNull
        {
            get { return _camera ? false : true; }
        }

        public GameObject Object
        {
            get { return _camera ? _camera.gameObject : default(GameObject); }
        }

        public Vector3 Position
        {
            get { return _camera.transform.localPosition; }
            set { _camera.transform.localPosition = value; }
        }

        public Matrix4x4 Matrix
        {
            get { return _camera.projectionMatrix; }
            set { _camera.projectionMatrix = value; }
        }

        public float FarClipPlane
        {
            get { return _camera.farClipPlane; }
            set { _camera.farClipPlane = value; }
        }

        public float NearClipPlane
        {
            get { return _camera.nearClipPlane; }
            set { _camera.nearClipPlane = value; }
        }

        public Rect ViewRect
        {
            get { return _camera.rect; }
            set { _camera.rect = value; }
        }

        public RenderTexture TargetTexture
        {
            get { return !_camera ? null : _camera.targetTexture;}
            set { if (_camera) _camera.targetTexture = value; }
        }

        public bool Enabled
        {
            get { return !_camera ? false : _camera.enabled; }
            set { if (_camera) _camera.enabled = value; }
        }

        public void SetTextureParams(int width, int height)
        {
            if (IsCameraNull)
                return;

            int antiAliasing = LeiaDisplay.Instance.AntiAliasing;

            if (_camera.targetTexture == null)
            {
                TargetTexture = CreateRenderTexture(width, height, antiAliasing);
            }
            else
            {
                if (TargetTexture.width != width ||
                    TargetTexture.height != height ||
                    TargetTexture.antiAliasing != antiAliasing )
                {
                    ReleaseTexture();
                    TargetTexture = CreateRenderTexture(width, height, antiAliasing);
                }
            }
        }

        private RenderTexture CreateRenderTexture(int width, int height, int antiAliasing)
        {
            this.Debug("Creating RenderTexture");
            this.Trace(width + "x" + height + ", antiAliasing: " + antiAliasing);
            var newTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            newTexture.anisoLevel = 1;
            newTexture.filterMode = FilterMode.Point;
            newTexture.antiAliasing = antiAliasing;
            newTexture.Create();

            return newTexture;
        }

        /// <summary>
        /// Gets parameters from root camera
        /// </summary>
        public void RefreshParameters(UnityCameraParams cameraParams)
        {
            if (IsCameraNull)
                return;

            _camera.clearFlags = cameraParams.ClearFlags;
            _camera.cullingMask = cameraParams.CullingMask;
            _camera.depth = cameraParams.Depth;
            _camera.backgroundColor = cameraParams.BackgroundColor;
            _camera.orthographic = cameraParams.Orthographic;
            _camera.orthographicSize = cameraParams.OrthographicSize;
            ViewRect = cameraParams.ViewportRect;
			#if UNITY_5_6_OR_NEWER
			_camera.allowHDR = cameraParams.AllowHDR;
			#else
			_camera.hdr = cameraParams.AllowHDR;
			#endif

        }

        public void Render()
        {
            if (IsCameraNull)
                return;

            _camera.Render();
        }



        public LeiaView(GameObject root, UnityCameraParams cameraParams)
        {
            this.Debug("ctor()");
            var rootCamera = root.GetComponent<Camera>();

            for (int i = 0; i < rootCamera.transform.childCount; i++)
            {
                var child = rootCamera.transform.GetChild(i);

                if (child.name == DISABLED_NAME)
                {
                    child.name = ENABLED_NAME;
                    child.hideFlags = HideFlags.None;
                    _camera = child.GetComponent<Camera>();
                    _camera.enabled = true;
					 
					#if UNITY_5_6_OR_NEWER
					_camera.allowHDR = cameraParams.AllowHDR;
					#else
					_camera.hdr = cameraParams.AllowHDR;
					#endif
                    break;
                }
            }

            if (_camera == null)
            {
                _camera = new GameObject(ENABLED_NAME).AddComponent<Camera>();
            }

            _camera.transform.parent = root.transform;
            _camera.transform.localPosition = Vector3.zero;
            _camera.transform.localRotation = Quaternion.identity;
            _camera.clearFlags = cameraParams.ClearFlags;
            _camera.cullingMask = cameraParams.CullingMask;
            _camera.depth = cameraParams.Depth;
            _camera.backgroundColor = cameraParams.BackgroundColor;
            _camera.fieldOfView = cameraParams.FieldOfView;
            _camera.depthTextureMode = DepthTextureMode.None;
            _camera.hideFlags = HideFlags.None;
            _camera.orthographic = cameraParams.Orthographic;
            _camera.orthographicSize = cameraParams.OrthographicSize;
            ViewRect = rootCamera.rect;
			#if UNITY_5_6_OR_NEWER
			_camera.allowHDR = cameraParams.AllowHDR;
			#else
			_camera.hdr = cameraParams.AllowHDR;
			#endif
            
        }

        public void ReleaseTexture()
        {
            var texture = TargetTexture;

            if (texture != null)
            {
                this.Debug("Destroying Texture");
                this.Trace(texture.width + "x" + texture.height);

                TargetTexture = null;

                if (Application.isPlaying)
                {
                    GameObject.Destroy(texture);
                }
                else
                {
                    GameObject.DestroyImmediate(texture);
                }
            }
        }
    }
}