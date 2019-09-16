using UnityEngine;

public enum WinConditionType
{
    TeamKill
}

public abstract class WinCondition : MonoBehaviour
{
    public abstract void SetupWinCondition();
    
}
