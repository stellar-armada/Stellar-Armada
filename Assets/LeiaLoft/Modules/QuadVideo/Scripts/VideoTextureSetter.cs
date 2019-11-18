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
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer), typeof(RawImage))]
public class VideoTextureSetter : MonoBehaviour 
{
    void Awake()
    {
        GetComponent<VideoPlayer>().prepareCompleted += OnVideoPlayerPrepared;
    }

    void OnVideoPlayerPrepared(VideoPlayer player)
    {
        GetComponent<RawImage>().texture = player.texture;
    }
}
