using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
   [SerializeField] private Transform targetToFollow;

   void LateUpdate()
   {
      transform.position = targetToFollow.position;
      transform.rotation = targetToFollow.rotation;
   }
   
}
