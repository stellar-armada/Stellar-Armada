using SpaceCommander.UI;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceCommander

{
    public class Clock
    {

        public Timer currentTimer;
        public UnityEvent OnUpdate;

        public void Update()
        {
            OnUpdate?.Invoke();
        }
    }
}