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

namespace LeiaLoft
{
    [System.Serializable]
    public struct LeiaCameraData
    {
        public float baseline;
        public float screenHalfHeight;
        public float screenHalfWidth;
        public float baselineScaling;
    }

    [System.Serializable]
    public struct LeiaBoundsData
    {
        public Vector3[] screen;
        public Vector3[] north;
        public Vector3[] south;
        public Vector3[] top;
        public Vector3[] bottom;
        public Vector3[] east;
        public Vector3[] west;
    }

    /// <summary>
    /// Uses first LeiaCamera instance to form a ray from screen position
    /// </summary>
    public static class LeiaCameraUtils
    {
    public static LeiaCameraData ComputeLeiaCamera(Camera camera, float convergenceDistance, float baselineScaling, DisplayConfig displayConfig)
    {
		LeiaCameraData leiaCameraData = new LeiaCameraData();
		displayConfig.UserOrientationIsLandscape = camera.pixelWidth > camera.pixelHeight;
		
		float f = displayConfig.UserViewResolution.y / 2f / Mathf.Tan(camera.fieldOfView * Mathf.PI / 360f);
		leiaCameraData.baseline         = displayConfig.SystemDisparityPixels * baselineScaling * convergenceDistance / f ;
		leiaCameraData.screenHalfHeight = convergenceDistance * Mathf.Tan(camera.fieldOfView * Mathf.PI / 360.0f);
		leiaCameraData.screenHalfWidth	= displayConfig.UserAspectRatio * leiaCameraData.screenHalfHeight;
		leiaCameraData.baselineScaling  = baselineScaling;

		return leiaCameraData;
    }

        public static LeiaBoundsData ComputeLeiaBounds(Camera camera, LeiaCameraData leiaCamera, float convergenceDistance, Vector2 cameraShift, DisplayConfig displayConfig)
        {
      LeiaBoundsData leiaBounds = new LeiaBoundsData();
      var localToWorldMatrix = camera.transform.localToWorldMatrix;

      localToWorldMatrix.SetColumn(0, localToWorldMatrix.GetColumn(0).normalized);
      localToWorldMatrix.SetColumn(1, localToWorldMatrix.GetColumn(1).normalized);
      localToWorldMatrix.SetColumn(2, localToWorldMatrix.GetColumn(2).normalized);

      if (camera.orthographic)
      {

        // assumes baseline = (baseline scaling) * (width of view in world units) * (system disparity in pixels) * (convergence distance) / (view width in pixels)

        float halfSizeY = camera.orthographicSize;
        float halfSizeX = halfSizeY * camera.aspect;


        Vector3 screenTopLeft     = localToWorldMatrix.MultiplyPoint (new Vector3 (-halfSizeX,  halfSizeY, convergenceDistance));
        Vector3 screenTopRight    = localToWorldMatrix.MultiplyPoint (new Vector3 ( halfSizeX,  halfSizeY, convergenceDistance));
        Vector3 screenBottomLeft  = localToWorldMatrix.MultiplyPoint (new Vector3 (-halfSizeX, -halfSizeY, convergenceDistance));
        Vector3 screenBottomRight = localToWorldMatrix.MultiplyPoint (new Vector3 ( halfSizeX, -halfSizeY, convergenceDistance));


        float negativeSystemDisparityZ = convergenceDistance - 1.0f / leiaCamera.baselineScaling;

        Vector3 nearTopLeft     = localToWorldMatrix.MultiplyPoint (new Vector3 (-halfSizeX,  halfSizeY, negativeSystemDisparityZ));
        Vector3 nearTopRight    = localToWorldMatrix.MultiplyPoint (new Vector3 ( halfSizeX,  halfSizeY, negativeSystemDisparityZ));
        Vector3 nearBottomLeft  = localToWorldMatrix.MultiplyPoint (new Vector3 (-halfSizeX, -halfSizeY, negativeSystemDisparityZ));
        Vector3 nearBottomRight = localToWorldMatrix.MultiplyPoint (new Vector3 ( halfSizeX, -halfSizeY, negativeSystemDisparityZ));


        float positiveSystemDisparityZ = convergenceDistance + 1.0f / leiaCamera.baselineScaling;

        Vector3 farTopLeft     = localToWorldMatrix.MultiplyPoint (new Vector3 (-halfSizeX,  halfSizeY, positiveSystemDisparityZ));
        Vector3 farTopRight    = localToWorldMatrix.MultiplyPoint (new Vector3 ( halfSizeX,  halfSizeY, positiveSystemDisparityZ));
        Vector3 farBottomLeft  = localToWorldMatrix.MultiplyPoint (new Vector3 (-halfSizeX, -halfSizeY, positiveSystemDisparityZ));
        Vector3 farBottomRight = localToWorldMatrix.MultiplyPoint (new Vector3 ( halfSizeX, -halfSizeY, positiveSystemDisparityZ));


        leiaBounds.screen = new Vector3[] { screenTopLeft,  screenTopRight,  screenBottomRight, screenBottomLeft };
        leiaBounds.south  = new Vector3[] { nearTopLeft,    nearTopRight,    nearBottomRight,   nearBottomLeft   };
        leiaBounds.north  = new Vector3[] { farTopLeft,     farTopRight,     farBottomRight,    farBottomLeft    };
        leiaBounds.top    = new Vector3[] { nearTopLeft,    nearTopRight,    farTopRight,       farTopLeft       };
        leiaBounds.bottom = new Vector3[] { nearBottomLeft, nearBottomRight, farBottomRight,    farBottomLeft    };
        leiaBounds.east   = new Vector3[] { nearTopRight,   nearBottomRight, farBottomRight,    farTopRight      };
        leiaBounds.west   = new Vector3[] { nearTopLeft,    nearBottomLeft,  farBottomLeft,     farTopLeft       };

      }

      else
      {

        cameraShift = leiaCamera.baseline * cameraShift;

        Vector3 screenTopLeft     = localToWorldMatrix.MultiplyPoint(new Vector3(-leiaCamera.screenHalfWidth, leiaCamera.screenHalfHeight, convergenceDistance));
        Vector3 screenTopRight    = localToWorldMatrix.MultiplyPoint(new Vector3( leiaCamera.screenHalfWidth, leiaCamera.screenHalfHeight, convergenceDistance));
        Vector3 screenBottomLeft  = localToWorldMatrix.MultiplyPoint(new Vector3(-leiaCamera.screenHalfWidth,-leiaCamera.screenHalfHeight, convergenceDistance));
        Vector3 screenBottomRight = localToWorldMatrix.MultiplyPoint(new Vector3( leiaCamera.screenHalfWidth,-leiaCamera.screenHalfHeight, convergenceDistance));

        float nearPlaneZ = (leiaCamera.baselineScaling * convergenceDistance) / (leiaCamera.baselineScaling + 1f);
        float nearRatio  = nearPlaneZ / convergenceDistance;
        float nearShiftRatio = 1f - nearRatio;

        Bounds localNearPlaneBounds = new Bounds(
          new Vector3(nearShiftRatio * cameraShift.x, nearShiftRatio * cameraShift.y, nearPlaneZ),
          new Vector3(leiaCamera.screenHalfWidth * nearRatio * 2, leiaCamera.screenHalfHeight * nearRatio * 2, 0));

        Vector3 nearTopLeft     = localToWorldMatrix.MultiplyPoint(new Vector3(localNearPlaneBounds.min.x, localNearPlaneBounds.max.y, localNearPlaneBounds.center.z));
        Vector3 nearTopRight    = localToWorldMatrix.MultiplyPoint(new Vector3(localNearPlaneBounds.max.x, localNearPlaneBounds.max.y, localNearPlaneBounds.center.z));
        Vector3 nearBottomLeft  = localToWorldMatrix.MultiplyPoint(new Vector3(localNearPlaneBounds.min.x, localNearPlaneBounds.min.y, localNearPlaneBounds.center.z));
        Vector3 nearBottomRight = localToWorldMatrix.MultiplyPoint(new Vector3(localNearPlaneBounds.max.x, localNearPlaneBounds.min.y, localNearPlaneBounds.center.z));

        float farPlaneZ = (leiaCamera.baselineScaling * convergenceDistance) / (leiaCamera.baselineScaling - 1f);
        farPlaneZ = 1f / Mathf.Max(1f / farPlaneZ, 1e-5f);

        float farRatio  = farPlaneZ / convergenceDistance;
        float farShiftRatio = 1f - farRatio;

        Bounds localFarPlaneBounds = new Bounds(
          new Vector3(farShiftRatio * cameraShift.x, farShiftRatio * cameraShift.y, farPlaneZ),
          new Vector3(leiaCamera.screenHalfWidth * farRatio * 2, leiaCamera.screenHalfHeight  * farRatio * 2, 0));

        Vector3 farTopLeft      = localToWorldMatrix.MultiplyPoint(new Vector3(localFarPlaneBounds.min.x, localFarPlaneBounds.max.y, localFarPlaneBounds.center.z));
        Vector3 farTopRight     = localToWorldMatrix.MultiplyPoint(new Vector3(localFarPlaneBounds.max.x, localFarPlaneBounds.max.y, localFarPlaneBounds.center.z));
        Vector3 farBottomLeft   = localToWorldMatrix.MultiplyPoint(new Vector3(localFarPlaneBounds.min.x, localFarPlaneBounds.min.y, localFarPlaneBounds.center.z));
        Vector3 farBottomRight  = localToWorldMatrix.MultiplyPoint(new Vector3(localFarPlaneBounds.max.x, localFarPlaneBounds.min.y, localFarPlaneBounds.center.z));

				leiaBounds.screen = new Vector3[] { screenTopLeft,  screenTopRight,  screenBottomRight, screenBottomLeft };
				leiaBounds.south  = new Vector3[] { nearTopLeft,    nearTopRight,    nearBottomRight,   nearBottomLeft   };
				leiaBounds.north  = new Vector3[] { farTopLeft,     farTopRight,     farBottomRight,    farBottomLeft    };
				leiaBounds.top    = new Vector3[] { nearTopLeft,    nearTopRight,    farTopRight,       farTopLeft       };
				leiaBounds.bottom = new Vector3[] { nearBottomLeft, nearBottomRight, farBottomRight,    farBottomLeft    };
				leiaBounds.east   = new Vector3[] { nearTopRight,   nearBottomRight, farBottomRight,    farTopRight      };
				leiaBounds.west   = new Vector3[] { nearTopLeft,    nearBottomLeft,  farBottomLeft,     farTopLeft       };
      }

              return leiaBounds;
        }

        public static Ray ScreenPointToRay(Vector2 position)
        {
      DisplayConfig displayConfig;

            if (Application.isPlaying)
        displayConfig = LeiaDisplay.Instance.GetDisplayConfig();
            else
        displayConfig = Object.FindObjectOfType<LeiaDisplay>().GetDisplayConfig();

      float x = position.x / displayConfig.UserPanelResolution.x;
      float y = position.y / displayConfig.UserPanelResolution.y;

            var leiaCamera = LeiaCamera.Instance;

            var leiaBounds = ComputeLeiaBounds(
                leiaCamera.GetComponent<Camera>(),
                ComputeLeiaCamera(LeiaCamera.Instance.GetComponent<Camera>(),
                    LeiaCamera.Instance.ConvergenceDistance,
                    LeiaCamera.Instance.BaselineScaling,
          displayConfig),
                    leiaCamera.ConvergenceDistance, leiaCamera.CameraShift,
                    displayConfig);

            var BR = leiaBounds.screen[2];
            var BL = leiaBounds.screen[3];
            var TL = leiaBounds.screen[0];

            var s = BR - BL;
            var t = TL - BL;

            var tpos = LeiaCamera.Instance.transform.position;
            var pos = (BL + (x * s + y * t));
            var dir = pos - tpos;

            return (new Ray(tpos, dir));
        }
    }
}
