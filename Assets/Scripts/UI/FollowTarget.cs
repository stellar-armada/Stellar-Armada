using UnityEngine;
#pragma warning disable 0649
namespace StellarArmada
{
   public class FollowTarget : MonoBehaviour
   {
      [SerializeField] private Transform targetToFollow;

      void LateUpdate()
      {
            if (targetToFollow == null) return;
         transform.position = targetToFollow.position;
         transform.rotation = targetToFollow.rotation;
      }

   }
}