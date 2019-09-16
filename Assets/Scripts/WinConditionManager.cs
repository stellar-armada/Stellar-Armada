using UnityEngine;

public class WinConditionManager : MonoBehaviour
{
   public static WinConditionManager instance;
   
   public WinConditionDictionary winConditionDictionary;

   public void Awake()
   {
      instance = this;
   }
   
}
