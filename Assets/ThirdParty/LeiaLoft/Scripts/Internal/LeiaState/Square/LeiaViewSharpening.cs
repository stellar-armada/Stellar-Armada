using UnityEngine;

namespace LeiaLoft
{
    public class LeiaViewSharpening : MonoBehaviour
    {
        public const string ShaderName = "LeiaLoft/ViewSharpening";
        public const string DisplayNotFound = "LeiaDisplay not found. ViewSharpening require LeiaDiplay on same GameObject.";

        private ScreenOrientation _lastOrientation;
		private LeiaDisplay _leiaDisplay; 
        private Material _material;

        void Start()
        {
			_leiaDisplay = LeiaDisplay.Instance;

			if (_leiaDisplay == null)
            {
                this.Error(DisplayNotFound);
                return;
			} 

        }

        /// <summary>
        /// Use to apply parameters
        /// </summary>
        public void UpdateParameters()
        {
            if (_material == null)
            {
                _material = new Material(Shader.Find(ShaderName));
            }

            if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
            {
				_material.SetFloat("_a", _leiaDisplay.GetDisplayConfig().ActCoefficients.x[0]);
				_material.SetFloat("_b", _leiaDisplay.GetDisplayConfig().ActCoefficients.x[1]);
            }
            else
            {
				_material.SetFloat("_a", _leiaDisplay.GetDisplayConfig().ActCoefficients.y[0]);
				_material.SetFloat("_b", _leiaDisplay.GetDisplayConfig().ActCoefficients.y[1]);
            }

            _lastOrientation = Screen.orientation;
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (_material == null || Screen.orientation != _lastOrientation)
            {
                UpdateParameters();
				Graphics.Blit(src, dest);
				return;
            }
            src.wrapMode = TextureWrapMode.Clamp;
            Graphics.Blit(src, dest, _material);
        }
    }
}
