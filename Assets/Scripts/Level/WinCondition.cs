using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Levels
{
    
    // Win condition base class
    // Win conditions report state to the match game mode, which reports whether or not the team is over and the match should end to the match manager
    // TO-DO: Implement
    public abstract class WinCondition : MonoBehaviour
    {
        public delegate void WinConditionListener();

        public WinConditionListener WinConditionListeners;

        public void AddWinConditionListener(WinConditionListener listener)
        {
        }
    }
}