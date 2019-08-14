using UnityEngine;
#pragma warning disable 0649
namespace SpaceCommander
{
   public class FollowTarget : MonoBehaviour
   {
      [SerializeField] private Transform targetToFollow;

      void LateUpdate()
      {
         transform.position = targetToFollow.position;
         transform.rotation = targetToFollow.rotation;
      }

   }
}