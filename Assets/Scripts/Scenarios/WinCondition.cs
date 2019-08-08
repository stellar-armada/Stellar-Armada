using UnityEngine;

namespace SpaceCommander.Scenarios
{
    public abstract class WinCondition : MonoBehaviour
    {
        public abstract void TestWinCondition();

        public delegate void WinConditionListener();

        public WinConditionListener winConditionListeners;

        public void AddWinConditionListener(WinConditionListener listener)
        {
        }
    }
}