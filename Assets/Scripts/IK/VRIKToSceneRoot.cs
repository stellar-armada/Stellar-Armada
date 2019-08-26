using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.IK {

	public class VRIKToSceneRoot : MonoBehaviour
	{

		public VRIK ik;

		Vector3 lastPosition;
		Quaternion lastRotation = Quaternion.identity;
		private Transform root;
		void Start()
		{
			root = SceneRoot.instance.transform;
			lastPosition = root.position;
			lastRotation = root.rotation;
		}

		void Update()
		{
			ik.solver.AddPlatformMotion(root.position - lastPosition,
				root.rotation * Quaternion.Inverse(lastRotation), root.position);

			lastRotation = root.rotation;
			lastPosition = root.position;
		}
	}
}
