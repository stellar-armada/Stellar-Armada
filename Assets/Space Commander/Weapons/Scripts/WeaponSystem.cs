using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander
{

public abstract class  WeaponSystem : MonoBehaviour
{

    public IPlayer owningPlayer;

    public Transform target;

    [SerializeField] private float damagePerHit;
    
    public Animator[] Animators;

    // Timer reference                
    [HideInInspector] public int timerID = -1;
    
    public float timer = 0f;
    public float tickRate = .2f;
    public float maxRange = .25f;
    public LayerMask damageableLayerMask;
    public LayerMask allRaycastableLayersMask;

    RaycastHit hitInfo; // Raycast structure
    public bool isFiring; // Is turret currently in firing state
    
    public IPlayer GetPlayer()
    {
        return owningPlayer;
    }

    public void SetPlayer(IPlayer player)
    {
        owningPlayer = player;
    }

    public float GetDamage()
    {
        return damagePerHit;
    }

    public abstract void AcquireTarget();

    // Fire turret weapon
    public abstract void StartFiring();

    // Stop firing 
    public abstract void StopFiring();

    public abstract void Fire();
    
    public void CheckForFire()
    {

        if (target == null) isFiring = false;

        // Fire turret
        if (!isFiring && target != null)
        {
            Debug.Log("Raycasting to see if we should fire");
            // if we raycast, start shooting. otherwise, wait til next check :)
            if (Physics.Raycast(transform.position, (target.position - transform.position), out hitInfo, maxRange, allRaycastableLayersMask))
            {
                Debug.Log("Raycast successful, shooting at " + hitInfo);
                isFiring = true;
                StartFiring();
            }
        }

        // Stop firing
        else if (isFiring && target == null)
        {
            isFiring = false;
            StopFiring();
        }
    }

    public void PlayAnimation()
    {
        for (int i = 0; i < Animators.Length; i++)
            Animators[i].SetTrigger("FireTrigger");
    }

    public void PlayAnimationLoop()
    {
        for (int i = 0; i < Animators.Length; i++)
            Animators[i].SetBool("FireLoopBool", true);
    }

    public void StopAnimation()
    {
        for (int i = 0; i < Animators.Length; i++)
            Animators[i].SetBool("FireLoopBool", false);
    }


}
}
