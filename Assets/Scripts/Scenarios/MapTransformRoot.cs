using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTransformRoot : MonoBehaviour
{
    public static MapTransformRoot instance;

    void Awake()
    {
        instance = this;
    }
}
