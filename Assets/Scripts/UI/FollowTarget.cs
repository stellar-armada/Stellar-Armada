using UnityEngine;

public class FollowTarget : MonoBehaviour
{
   [SerializeField] private Transform targetToFollow;

   void LateUpdate()
   {
      transform.localPosition = targetToFollow.localPosition;
      transform.localPosition = targetToFollow.localPosition;
   }
   
}
