using Mirror;
using StellarArmada.Levels;
using StellarArmada.Player;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.IK {

	public class VRIKToSceneRoot : MonoBehaviour
	{
		[SerializeField] PlayerController playerController;
		public VRIK ik;

		Vector3 lastPosition;
		Quaternion lastRotation = Quaternion.identity;
		private Transform root;
		private bool isInitialized = false;

		void Awake()
		{
			playerController.OnLocalPlayerInitialized += Initialize;
		}
		
		void Initialize()
		{
			root = LocalPlayerBridgeSceneRoot.instance.transform;
			isInitialized = true;
			lastPosition = root.position;
			lastRotation = root.rotation;
		}

		void Update()
		{
			if (!isInitialized) return;
			ik.solver.AddPlatformMotion(root.position - lastPosition,
				root.rotation * Quaternion.Inverse(lastRotation), root.position);

			lastRotation = root.rotation;
			lastPosition = root.position;
		}
	}
}
