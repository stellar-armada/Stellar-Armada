/****************************************************************
*
* Copyright 2019 © Leia Inc.  All rights reserved.
*
* NOTICE:  All information contained herein is, and remains
* the property of Leia Inc. and its suppliers, if any.  The
* intellectual and technical concepts contained herein are
* proprietary to Leia Inc. and its suppliers and may be covered
* by U.S. and Foreign Patents, patents in process, and are
* protected by trade secret or copyright law.  Dissemination of
* this information or reproduction of this materials strictly
* forbidden unless prior written permission is obtained from
* Leia Inc.
*
****************************************************************
*/
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI Pixel converter should be attached to Canvas.
/// </summary>
[RequireComponent(typeof(CanvasScaler))]
public class UIPixelConverter : MonoBehaviour 
{
    static Vector2 referenceResolution = Vector2.zero;
    static Vector2 ReferenceResolution 
    {
        get
        {
            if(referenceResolution == Vector2.zero)
            {
                referenceResolution =  
                    FindObjectOfType<UIPixelConverter>().
                    GetComponent<CanvasScaler>().referenceResolution;
            }
            return referenceResolution;
        }
    }

    static float ToPixelCoord(float ui, float screen, float reference)
    {
        return InterpolateLinear(ui, 0, reference, 0, screen);
    }

    public static float ToPixelCoordX(float ui)
    {
        return ToPixelCoord(ui, Screen.width, ReferenceResolution.x);
    }

	public static float InterpolateLinear(float PROGRESS, float a, float b, float A, float B)
	{
		return A + (PROGRESS - a) * (B - A) / (b - a);
	}
}
