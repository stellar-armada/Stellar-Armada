using System.Collections;
using System.Collections.Generic;
using StellarArmada.Ships;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShipTypePortraitDictionary :  SerializableDictionary<ShipType, Sprite>
{
    
}

public class FlagshipPortraitController : MonoBehaviour
{
    public static FlagshipPortraitController instance;
    
    [SerializeField] private Image flagshipPortrait;
    [SerializeField] private TextMeshProUGUI flagshipName;
    
    // Dictionary of shipyard portraits
    [SerializeField] private ShipTypePortraitDictionary shipTypePortraitDictionary;

    string name = "";

    void Awake()
    {
        instance = this;
        ClearFlagship();
    } 
    
    public void ClearFlagship()
    {
        flagshipName.name = "No Flagship Selected";
        flagshipPortrait.gameObject.SetActive(false);

    }
    
    public void SetFlagship(ShipType type)
    {
        Debug.Log("SetFlagship");

        switch (type)
        {
            case ShipType.Basilisk:
                name = "Basilisk";
                break;
            case ShipType.Dreadnaught:
                name = "Dreadnaught";
                break;
            case ShipType.Guardian:
                name = "Guardian";
                break;
        }
        flagshipPortrait.gameObject.SetActive(true);
        flagshipName.text = name;
        flagshipPortrait.sprite = shipTypePortraitDictionary[type];
    }
}
