using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
namespace StellarArmada.UI
{
    // Nice little UI doodad to animate an image feel
    // Used on the SceneRootIndicator animation
    public class FillImageOverTime : MonoBehaviour
    {
        [SerializeField] float duration = .5f;
        [SerializeField] float delay;
        Image image;

        public void Activate()
        {
            StartCoroutine(Fill(1f));
        }

        public void Deactivate()
        {
            StartCoroutine(Fill(0f));

        }

        IEnumerator Fill(float amount)
        {
            image = GetComponent<Image>();

            yield return new WaitForSeconds(delay);

            float timer = 0;
            float currentAmount = image.fillAmount;
            do
            {
                timer += Time.deltaTime;
                image.fillAmount = Mathf.Lerp(currentAmount, amount, timer / duration);
                yield return null;
            } while (timer < duration);



        }

    }
}