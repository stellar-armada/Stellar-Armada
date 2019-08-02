using System.Collections;
using UnityEngine;

namespace SpaceCommander.Utility
{
    // Simple UI movement animation component, used on the SceneRootIndicator

    public class GoToPoint : MonoBehaviour
    {

        [SerializeField] Vector3 startPos;
        [SerializeField] Vector3 endPos;
        [SerializeField] float duration = .5f;
        [SerializeField] float delay;
        [SerializeField] float endDelay;

        public void Activate()
        {
            StartCoroutine(MoveToPoint(endPos, false));
        }

        public void Deactivate()
        {
            StartCoroutine(MoveToPoint(startPos, true));
        }

        IEnumerator MoveToPoint(Vector3 posToGoTo, bool isEnd)
        {
            yield return new WaitForSeconds(isEnd ? endDelay : delay);

            float timer = 0;

            Vector3 currentPos = transform.position;
            do  {
                timer += Time.deltaTime;

                transform.localPosition = Vector3.Lerp(currentPos, posToGoTo, MathUtilities.EaseInOutQuad(0f, 1f, timer / duration));

                yield return null;

            } while (timer < duration);
        }
    }
}