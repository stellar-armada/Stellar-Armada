using UnityEngine;

namespace Mirror
{
    // Modified version of component
    [DisallowMultipleComponent]
    public class NetworkTransform : NetworkTransformBase
    {
        protected override Transform targetComponent => transform;
    }
}
