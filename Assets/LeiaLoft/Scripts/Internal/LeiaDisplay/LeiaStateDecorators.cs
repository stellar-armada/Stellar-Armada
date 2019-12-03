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
using System;

namespace LeiaLoft
{
    /// <summary>
    /// Changeable settings that affect rendering.
    /// </summary>
    [Serializable]
    public struct LeiaStateDecorators
    {
        public bool ShowTiles;
        public bool ShowCalibration;
        public bool ParallaxAutoRotation;
        public bool ShouldParallaxBePortrait;
        public bool AlphaBlending;

        public ParallaxOrientation ParallaxOrientation
        {
            get
            {
                #if UNITY_ANDROID && !UNITY_EDITOR
                if (!ParallaxAutoRotation)
                    return ParallaxOrientation.Landscape;
                else if (ShouldParallaxBePortrait)
                    return DisplayInfo.IsLeiaDisplayInPortraitMode ? ParallaxOrientation.Landscape : ParallaxOrientation.Portrait;
                else
                    return !DisplayInfo.IsLeiaDisplayInPortraitMode ? ParallaxOrientation.Landscape : ParallaxOrientation.Portrait;
                #else
                return ParallaxOrientation.Landscape;
                #endif
            }
        }

        public Vector2 AdaptFOV;

        public static LeiaStateDecorators Default
        {
            get
            {
                return new LeiaStateDecorators();
            }
        }

        public override string ToString()
        {
            return string.Format("[LeiaStateDecorators: ShowTiles={0}, ShowCalibration={1},"
                + "ParallaxAutoRotation={2}, ShouldParallaxBePortrait={3}, ParallaxOrientation={4}, AdaptFOV={5}, AlphaBlending={6}]",
                ShowTiles, ShowCalibration, ParallaxAutoRotation, ShouldParallaxBePortrait, ParallaxOrientation, AdaptFOV, AlphaBlending);
        }
    }
}