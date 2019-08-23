using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.UI
{
    /* UI Menu Screen object attached to all UI screens
     * Add fancy fade logic or special handlers when menu screens are activated or deactivated here
     * TO-DO: Make sure screens aren't being called by SetActive from buttons (should be OK)
     */
    public class MenuScreen : MonoBehaviour
    {

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);

        }

    }
}
