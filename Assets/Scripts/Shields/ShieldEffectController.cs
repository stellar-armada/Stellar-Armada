using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using StellarArmada.Pooling;

#pragma warning disable 0649
namespace StellarArmada
{
    public class ShieldEffectController : MonoBehaviour
    {
        // Manages the shield look and effect on ships
        public float shieldActivationSpeed = 1.0f;
        [SerializeField] float shieldActivationRim = 0.2f;
        [SerializeField] GameObject shieldHitPrefab;
        
        public float hitEffectDuration = 0.5f;

        public Color hitColor;

        private CommandBuffer cmdBuffer;
        [SerializeField] Renderer[] renderers;

        private float shieldActivationTime = 1.0f;
        private float shieldActivationDir;

        private int activationTimeProperty;
        private int shieldDirectionProperty;

        private int currentHitIndex = 1;

        // Reference variables to store
        private Transform t;

        // hold and reuse for gc
        private Vector4 dir4;
        private Vector3 hitLocalSpace;
        private Vector3 dir;
        private Vector3 tan1;
        private Vector3 tan2;
        private MeshRenderer mr;
        private ShieldHit hit;

        void Awake()
        {
            t = transform;

            shieldActivationDir = 0.0f;

            ResetActivationTime();

            SetShieldEffectDirection(new Vector3(1.0f, 0.0f, 0.0f));
        }

        public void SetShieldEffectDirection(Vector3 dir)
        {
            dir4 = new Vector4(dir.x, dir.y, dir.z, 0.0f);
            foreach (Renderer ren in renderers)
                ren.material.SetVector(shieldDirectionProperty, dir4);
        }

        void ResetActivationTime()
        {
            foreach (Renderer ren in renderers)
                ren.material.SetFloat(activationTimeProperty, 1.0f);
        }

        void Update()
        {
            if (shieldActivationDir > 0.0f)
            {
                shieldActivationTime += shieldActivationSpeed * Time.deltaTime;
                if (shieldActivationTime >= 1.0f)
                {
                    shieldActivationTime = 1.0f;
                    shieldActivationDir = 0.0f;
                }
            }
            else if (shieldActivationDir < 0.0f)
            {
                shieldActivationTime -= shieldActivationSpeed * Time.deltaTime;
                if (shieldActivationTime <= -shieldActivationRim)
                {
                    shieldActivationTime = -shieldActivationRim;
                    shieldActivationDir = 0.0f;
                    foreach (Renderer ren in renderers)
                        ren.enabled = false;
                }
            }

            foreach (Renderer ren in renderers)
                ren.material.SetFloat(activationTimeProperty, shieldActivationTime);
        }

        public bool IsDuringActivationAnim()
        {
            return shieldActivationDir != 0.0f;
        }

        public void SetShieldActive(bool active, bool animated = true)
        {
            shieldActivationDir = (active) ? 1.0f : -1.0f;
            foreach (Renderer ren in renderers)
            {
                ren.material.SetFloat(activationTimeProperty, shieldActivationTime);
                ren.material.SetFloat("_ActivationRim", shieldActivationTime);
            }
        }


        public void OnHit(Vector3 hitPos, float hitScale)
        {
            foreach (Renderer ren in renderers)
            {
                Transform rt = ren.transform;
                GameObject hitObject = PoolManager.Pools["GeneratedPool"]
                    .Spawn(shieldHitPrefab.transform, Vector3.zero, Quaternion.identity, null).gameObject;
                hitObject.transform.parent = rt;
                hitObject.transform.position = rt.position;
                hitObject.transform.localScale = Vector3.one;
                hitObject.transform.rotation = rt.rotation;

                hitLocalSpace = rt.InverseTransformPoint(hitPos);

                dir = hitLocalSpace.normalized;

                mr = hitObject.GetComponent<MeshRenderer>();

                mr.material.SetVector("_HitPos", hitLocalSpace);
                mr.material.SetFloat("_HitRadius", hitScale);
                mr.material.SetVector("_WorldScale", transform.lossyScale);
                mr.material.SetFloat("_PatternScale", ren.material.GetFloat("_PatternScale"));
                mr.material.SetColor("_Color", hitColor);

                hit = hitObject.GetComponent<ShieldHit>();
                hit.StartHitFX(hitEffectDuration);

                currentHitIndex++;
                if (currentHitIndex > 100)
                    currentHitIndex = 1;
            }
        }
    }
}