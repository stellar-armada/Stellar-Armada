using UnityEngine;

namespace SpaceCommander.Ships.Test
{
    public class ExplosionTest : MonoBehaviour
    {
        private IExplodable explosion;

        bool CheckPlayer()
        {
            if (ShipManager.GetShips().Count < 1) return false;
            if (explosion == null) explosion = ShipManager.GetShips()[0].shipExplosion;
            return true;
        }

        public void Explode()
        {
            if (!CheckPlayer()) return;
            explosion.Explode();
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