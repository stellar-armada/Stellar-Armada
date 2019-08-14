using UnityEngine;
using UnityEngine.EventSystems;

#pragma warning disable 0649
namespace Wacki {
    public class LaserPointerEventData : PointerEventData
    {
        public GameObject current;
        public IUILaserPointer controller;
        public LaserPointerEventData(EventSystem e) : base(e) { }

        public override void Reset()
        {
            current = null;
            controller = null;
            base.Reset();
        }
    }
}

