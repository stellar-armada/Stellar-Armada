using System.Collections;
using System.Collections.Generic;
using SpaceCommander.Ships;
using UnityEngine;

namespace SpaceCommander.Ships.Test
{
    public class ExplosionTest : MonoBehaviour
    {
        private Explosion explosion;

        bool CheckPlayer()
        {
            if (ShipManager.GetShips().Count < 1) return false;
            if (explosion == null) explosion = ShipManager.GetShips()[0].explosion;
            return true;
        }

        public void Explode()
        {
            if (!CheckPlayer()) return;
            explosion.gameObject.SetActive(true);
            explosion.CmdExplode();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Explode();
            }
        }
    }
}