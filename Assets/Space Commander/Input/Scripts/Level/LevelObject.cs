using System.Collections;
using UnityEngine;

namespace SpaceCommander.Level
{

    /* This is the actual level object that gets created for all players
     * 
     */
    public class LevelObject : MonoBehaviour
    {
        // Private fields serialized in inspector
        [SerializeField] Transform geometryRoot;
        [SerializeField] Renderer[] renderers;

        // Private fields
        float lerpTime = 1f;
        float materializationInvisibleValue = 1f;
        float materializationVisibleValue = 0f;
        string materialValue = "_DissolveAmount"; // This is the property on the shader to set, must be updated if you are using different shaders!
        bool levelIsActive = false;

        #region Initialization

        public void InitLevel()
        {
            levelIsActive = true;

            ActivateRenderers();
        }

        #endregion

        #region Deinitialization

        public void DeinitLevel()
        {
            DeactivateRenderers();
            levelIsActive = false;
        }

        #endregion

        #region Public Methods

        public bool IsLevelActive()
        {
            return levelIsActive;
        }

        public void SetLevelActiveState(bool state)
        {
            levelIsActive = state;
        }

        public void Materialize()
        {
            gameObject.SetActive(true);
            StartCoroutine(MatererializeIn());
            levelIsActive = true;
        }

        public void Dematerialize()
        {
            StartCoroutine(MaterializeOut());
            levelIsActive = false;
        }

        #endregion

        #region Private Methods
        IEnumerator MatererializeIn()
        {
            float timer = 0;
            ActivateRenderers();

            do
            {
                timer += Time.deltaTime;

                foreach (Renderer ren in renderers)
                {
                    ren.sharedMaterial.SetFloat(materialValue, Mathf.Lerp(materializationInvisibleValue, materializationVisibleValue, timer / lerpTime));
                }

                yield return null;
            } while (timer <= lerpTime);
        }

        IEnumerator MaterializeOut()
        {

            float timer = 0;

            do
            {
                timer += Time.deltaTime;
                foreach (Renderer ren in renderers)
                {
                    ren.sharedMaterial.SetFloat(materialValue, Mathf.Lerp(materializationInvisibleValue, materializationVisibleValue, 1.0f - timer / lerpTime));
                }
                yield return null;
            } while (timer <= lerpTime);
            
            DeactivateRenderers();
        }

        void ActivateRenderers()
        {

            foreach (Renderer ren in renderers)
            {
                ren.enabled = true;
            }

        }

        void DeactivateRenderers()
        {

            foreach (Renderer ren in renderers)
            {
                ren.enabled = false;
            }
        }
        #endregion
    }
}