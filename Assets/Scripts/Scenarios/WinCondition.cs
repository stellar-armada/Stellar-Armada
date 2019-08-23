using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Scenarios
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