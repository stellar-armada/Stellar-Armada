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
using LeiaLoft;
public class QuadVideoSetup : MonoBehaviour 
{
	/// <summary>
	/// Pixel offset from left of screen that is used for correct interlacing.
	/// </summary>
    public float pixelOffsetX;
    
	float previousPixelOffsetX;
	private Shader quadInterlace;
	private Shader quad2D;
    Material material;
	public RawImage rawImage;

	void Start()
	{
		quadInterlace = Shader.Find("LeiaLoft/QuadInterlace");
		quad2D = Shader.Find("LeiaLoft/Quad2D");
		SetupImage();

	}

	void OnEnable()
	{

        if (!LeiaDisplay.InstanceIsNull)
        {
            LeiaDisplay.Instance.StateChanged += OnLeiaStateChanged;
        }
	}

	void OnDisable()
	{
        if (!LeiaDisplay.InstanceIsNull)
        {
            LeiaDisplay.Instance.StateChanged -= OnLeiaStateChanged;
        }
	}

	/// <summary>
	/// Listener for LeiaDisplay StateChanged
	/// </summary>
	void OnLeiaStateChanged()
	{
		UpdateState();
	}

	/// <summary>
	/// Changes video render mode based on LeiaDisplay render mode.
	/// </summary>
	void UpdateState() {
		if(LeiaDisplay.Instance.LeiaStateId == LeiaDisplay.HPO)
		{
			if(rawImage != null) 
			{
				if(rawImage.material.shader != quadInterlace)
				{
					rawImage.material.shader = quadInterlace;
				}
			}
		} 
		else
		{
			if(rawImage != null) 
			{
				if(rawImage.material.shader != quad2D)
				{
					rawImage.material.shader = quad2D;
				}
			}
		}
	}

	/// <summary>
	/// Sets raw image parameters
	/// </summary>
	private void SetupImage () 
    {
		if( rawImage == null)
		{
			rawImage = GetComponent<RawImage>();
		}

        float windowSizeUI = rawImage.rectTransform.rect.width;
        float windowSizePixels = UIPixelConverter.ToPixelCoordX(windowSizeUI);

		rawImage.material = new Material(rawImage.material) { name = rawImage.material.name + "(Clone)" };
		material = rawImage.material;
        material.SetFloat("_VideoViewWidth", Mathf.Round(windowSizePixels));
	
		material.SetFloat("_CalibrationOffsetX",LeiaDisplay.Instance.CalibrationOffset[0]);
	}

	void Update()
	{
		if(!Mathf.Approximately(pixelOffsetX, previousPixelOffsetX))
        {
			material.SetFloat("_PositionOffsetX", previousPixelOffsetX = pixelOffsetX);
        }
	}
}
