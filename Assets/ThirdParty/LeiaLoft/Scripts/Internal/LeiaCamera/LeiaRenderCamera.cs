using UnityEngine;

namespace LeiaLoft
{
	public class LeiaRenderCamera : MonoBehaviour 
	{
		LeiaCamera _leiaCamera;

		public void setLeiaCamera(LeiaCamera leiaCamera)
		{
			_leiaCamera = leiaCamera;
		}


		void OnPostRender()
		{
			LeiaDisplay.Instance.RenderImage(_leiaCamera);
		}

	}
}