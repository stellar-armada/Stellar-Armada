using System;
using UnityEngine;
using SpaceCommander.Pooling;
using UnityEngine.UI;
using Random = UnityEngine.Random;

#pragma warning disable 0649
namespace SpaceCommander.Weapons
{
    public class MissileLauncher :  WeaponSystem
    {
        public Transform missilePrefab;
        public Transform[] socket;
        public Transform explosionPrefab;

        private Missile.MissileType missileType;

        public Text missileTypeLabel;

        // Use this for initialization
        private void Start()
        {
            missileType = Missile.MissileType.Unguided;
            missileTypeLabel.text = "Missile type: Unguided";
        }

        // Spawns explosion
        public void SpawnExplosion(Vector3 position)
        {
            PoolManager.Pools["GeneratedPool"]
                .Spawn( explosionPrefab, position, Quaternion.identity, null);
        }


        // Processes input for launching missile
        private void ProcessInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var randomSocketId = Random.Range(0, socket.Length);
                var tMissile = PoolManager.Pools["GeneratedPool"].SpawnDamager(this, missilePrefab,
                    socket[randomSocketId].position, socket[randomSocketId].rotation, null);

                if (tMissile != null)
                {
                    var missile = tMissile.GetComponent<Missile>();

                    missile.launcher = this;
                    missile.missileType = missileType;

                    if (target != null)
                        missile.target = target;
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                missileType = Missile.MissileType.Unguided;
                missileTypeLabel.text = "Missile type: Unguided";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                missileType = Missile.MissileType.Guided;
                missileTypeLabel.text = "Missile type: Guided";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                missileType = Missile.MissileType.Predictive;
                missileTypeLabel.text = "Missile type: Predictive";
            }
        }

        // Update is called once per frame
        private void Update()
        {
            ProcessInput();
        }

        protected override void AcquireTarget()
        {
            throw new NotImplementedException();
        }

        protected override void StartFiring()
        {
            throw new NotImplementedException();
        }

        protected override void StopFiring()
        {
            throw new NotImplementedException();
        }

        public override void Impact(Vector3 point)
        {
            throw new NotImplementedException();
        }
    }
}