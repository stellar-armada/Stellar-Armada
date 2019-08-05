using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class WinCondition : MonoBehaviour
{

    public abstract void TestWinCondition();

    public delegate void WinConditionListener();

    public WinConditionListener winConditionListeners;

    public void AddWinConditionListener(WinConditionListener listener)
    {
        
    }
    
}
