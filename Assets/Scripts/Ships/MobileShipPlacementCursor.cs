using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Ships
{
    public class MobileShipPlacementCursor : ShipPlacementCursor
    {
        protected override void Initialize()
        {
            isInitialized = true;
            t.SetParent(null);

        }

    }
}