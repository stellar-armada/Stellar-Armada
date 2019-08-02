using System.Collections;
using UnityEngine;

namespace SpaceCommander.UI
{
    // Object that shows up in the text feed -- currently hidden (group canvas alpha 0) and not formatted for the canvas recently, but perfectly usable if you want in-game text feedback or console

    public class TextFeedObject : MonoBehaviour
    {
        [SerializeField] TMPro.TextMeshProUGUI textObject;

        public static int maxQueueSize = 200;

        static Queue queue = new Queue();

        public void SetText(string newText)
        {
            textObject.text = newText;
            queue.Enqueue(this);
            if (queue.Count > maxQueueSize)
            {
                TextFeedObject obj = (TextFeedObject)queue.Dequeue();
                Destroy(obj.gameObject);
            }
        }

    }
}
