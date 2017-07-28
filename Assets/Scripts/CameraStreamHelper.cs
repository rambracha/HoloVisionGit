//  
// Copyright (c) 2017 Vulcan, Inc. All rights reserved.  
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
//

using HoloLensCameraStream;
using System;
using System.Linq;
using UnityEngine;

public class CameraStreamHelper : MonoBehaviour
{
    private event OnVideoCaptureResourceCreatedCallback m_VideoCaptureCreated;
    private static VideoCapture s_VideoCapture;
    private static CameraStreamHelper s_Instance;

    public static CameraStreamHelper Instance
    {
        get
        {
            return s_Instance;
        }
    }

    public void SetNativeISpatialCoordinateSystemPtr(IntPtr ptr)
    {
        s_VideoCapture.WorldOriginPtr = ptr;
    }

    public void GetVideoCaptureAsync(OnVideoCaptureResourceCreatedCallback onVideoCaptureAvailable)
    {
        if (onVideoCaptureAvailable == null)
        {
            Debug.LogError("You must supply the onVideoCaptureAvailable delegate.");
        }

        if (s_VideoCapture == null)
        {
            m_VideoCaptureCreated += onVideoCaptureAvailable;
        }
        else
        {
            onVideoCaptureAvailable(s_VideoCapture);
        }
    }

    public HoloLensCameraStream.Resolution GetHighestResolution()
    {
        if (s_VideoCapture == null)
        {
            throw new Exception("Please call this method after a VideoCapture instance has been created.");
        }
        return s_VideoCapture.GetSupportedResolutions().OrderByDescending((r) => r.width * r.height).FirstOrDefault();
    }

    public HoloLensCameraStream.Resolution GetLowestResolution()
    {
        if (s_VideoCapture == null)
        {
            throw new Exception("Please call this method after a VideoCapture instance has been created.");
        }
        return s_VideoCapture.GetSupportedResolutions().OrderBy((r) => r.width * r.height).FirstOrDefault();
    }

    public float GetHighestFrameRate(HoloLensCameraStream.Resolution forResolution)
    {
        if (s_VideoCapture == null)
        {
            throw new Exception("Please call this method after a VideoCapture instance has been created.");
        }
        return s_VideoCapture.GetSupportedFrameRatesForResolution(forResolution).OrderByDescending(r => r).FirstOrDefault();
    }

    public float GetLowestFrameRate(HoloLensCameraStream.Resolution forResolution)
    {
        if (s_VideoCapture == null)
        {
            throw new Exception("Please call this method after a VideoCapture instance has been created.");
        }
        return s_VideoCapture.GetSupportedFrameRatesForResolution(forResolution).OrderBy(r => r).FirstOrDefault();
    }

    private void Awake()
    {
        if (s_Instance != null)
        {
            Debug.LogError("Cannot create two instances of CamStreamManager.");
            return;
        }

        s_Instance = this;

#if WINDOWS_UWP
        VideoCapture.CreateAync(OnVideoCaptureInstanceCreated);
#endif

    }

    private void OnDestroy()
    {
        if (s_Instance == this)
        {
            s_Instance = null;
        }
    }

    private void OnVideoCaptureInstanceCreated(VideoCapture videoCapture)
    {
        if (videoCapture == null)
        {
            Debug.LogError("Creating the VideoCapture object failed.");
            return;
        }

        s_VideoCapture = videoCapture;
        if (m_VideoCaptureCreated != null)
        {
            m_VideoCaptureCreated(videoCapture);
        }
    }
}
