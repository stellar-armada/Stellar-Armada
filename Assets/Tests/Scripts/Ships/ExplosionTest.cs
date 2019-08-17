using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Ships.Test
{
    public class ExplosionTest : MonoBehaviour
    {
        private IExplodable explosion;

        bool CheckPlayer()
        {
            if (EntityManager.GetEntities().Count < 1) return false;
            if (explosion == null) explosion = ((Ship)EntityManager.GetEntities()[0]).shipExplosion;
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