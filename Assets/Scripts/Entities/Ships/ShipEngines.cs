using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Entities.Ships
{
	public class ShipEngines : MonoBehaviour
	{
		public float thrustAmount = 1.0f;
		private float maxSpeed;
		private float currentSpeed;
		
		
		public List<MeshRenderer> haloRenderers = new List<MeshRenderer>();
		public string shaderColorProperty = "_TintColor";
		public float minAlpha = 0.1f;
		public float maxAlpha = 1.0f;

		private List<Material> haloMaterials = new List<Material>();
		private Vector3 lastForward = Vector3.zero;

		private Ship _ship;


		void Awake()
		{
			_ship = GetComponent<Ship>();
			CollectMaterials();
		}

		/// Sets the progress and refresh all the halo materials.
		public void RefreshIntensity(float amount)
		{
			thrustAmount = Mathf.Clamp01(amount);
			RefreshHalosIntensity();
		}

		/// Refresh all the materials to reflect the new glowing intensity value.
		public void RefreshHalosIntensity()
		{
			float intensity = minAlpha + (maxAlpha - minAlpha) * thrustAmount;

			foreach (Material mat in haloMaterials)
			{
				Color currColor = mat.GetColor(shaderColorProperty);
				currColor.a = intensity;
				mat.SetColor(shaderColorProperty, currColor);
			}
		}
		
		[ContextMenu("RefreshHalosLookAt")]
		public void RefreshCameraLookAt()
		{
			if (haloRenderers.Count == 0 || Camera.main == null)
				return;

			lastForward = Camera.main.transform.forward;
			foreach (MeshRenderer render in haloRenderers)
				render.transform.forward = -lastForward;
		}

		[ContextMenu("CollectMaterials")]
		private void CollectMaterials()
		{
			Dictionary<Material, Material> instancedMaterials = new Dictionary<Material, Material>();
			foreach (MeshRenderer render in haloRenderers)
			{
				List<Material> newSharedMaterials = new List<Material>();
				foreach (Material source in render.sharedMaterials)
				{
					if (source == null)
						continue;
					Material instancedMaterial = null;
					if (!instancedMaterials.TryGetValue(source, out instancedMaterial))
					{
						instancedMaterial = new Material(source);
						instancedMaterials.Add(source, instancedMaterial);
					}

					newSharedMaterials.Add(instancedMaterial);
				}

				render.sharedMaterials = newSharedMaterials.ToArray();
			}

			HashSet<Material> uniqueMaterials = new HashSet<Material>(instancedMaterials.Values);
			haloMaterials = new List<Material>(uniqueMaterials);
		}
	}
}