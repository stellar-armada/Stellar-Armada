using System.Collections;
using System.Collections.Generic;
using SpaceCommander.Ships;
using UnityEngine;

namespace SpaceCommander.Ships.Test
{
    public class ExplosionTest : MonoBehaviour
    {
        private ShipExplosion _shipExplosion;

        bool CheckPlayer()
        {
            if (ShipManager.GetShips().Count < 1) return false;
            if (_shipExplosion == null) _shipExplosion = ShipManager.GetShips()[0].shipExplosion;
            return true;
        }

        public void Explode()
        {
            if (!CheckPlayer()) return;
            _shipExplosion.gameObject.SetActive(true);
            _shipExplosion.CmdExplode();
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