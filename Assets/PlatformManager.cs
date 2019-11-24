using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{

    public enum PlatformType
    {
        MOBILE,
        MOBILE_LEIA,
        DESKTOP,
        VR
}

    public static PlatformManager instance;

    public PlatformType Platform;

    void Awake() => instance = this;


}
