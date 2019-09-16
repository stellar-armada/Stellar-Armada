using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager instance;
    public void Awake() => instance = this;
    
    public Color defaultColor = Color.cyan;
    public Color friendlyColor = Color.blue;
    public Color enemyColor = Color.red;
    public Color neutralColor = Color.gray;
    public Color selectedColor = Color.green;
    public Color allyColor = Color.yellow;
}
