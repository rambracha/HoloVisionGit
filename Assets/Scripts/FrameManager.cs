//  
// Copyright (c) 2017 Vulcan, Inc. All rights reserved.  
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
//

using HoloLensCameraStream;
using System;
using UnityEngine;
using UnityEngine.VR.WSA;

public class FrameManager : MonoBehaviour
{
    private byte[] m_LatestImageBytes;
    private HoloLensCameraStream.Resolution m_Resolution;
    private VideoCapture m_VideoCapture;
    private IntPtr m_SpatialCoordinateSystemPtr;

    private void Start()
    {
        if (FrameContainer.Instance == null)
        {
            Debug.LogError("Must have a FrameContainer somewhere in the scene.");
            return;
        }

        if (CameraStreamHelper.Instance == null)
        {
            Debug.LogError("Must have a CameraStreamHelper somewhere in the scene.");
            return;
        }

        //Fetch a pointer to Unity's spatial coordinate system if you need pixel mapping
        m_SpatialCoordinateSystemPtr = WorldManager.GetNativeISpatialCoordinateSystemPtr();

        //Call this in Start() to ensure that the CameraStreamHelper is already "Awake".
        CameraStreamHelper.Instance.GetVideoCaptureAsync(OnVideoCaptureCreated);
    }

    private void OnDestroy()
    {
        if (m_VideoCapture != null)
        {
            m_VideoCapture.FrameSampleAcquired -= OnFrameSampleAcquired;
            m_VideoCapture.Dispose();
        }
    }

    private void OnVideoCaptureCreated(VideoCapture videoCapture)
    {
        if (videoCapture == null)
        {
            Debug.LogError("Did not find a video capture object. You may not be using the HoloLens.");
            return;
        }

        m_VideoCapture = videoCapture;

        //Request the spatial coordinate ptr if you want fetch the camera and set it if you need to 
        CameraStreamHelper.Instance.SetNativeISpatialCoordinateSystemPtr(m_SpatialCoordinateSystemPtr);

        m_Resolution = CameraStreamHelper.Instance.GetLowestResolution();
        float frameRate = CameraStreamHelper.Instance.GetHighestFrameRate(m_Resolution);
        videoCapture.FrameSampleAcquired += OnFrameSampleAcquired;

        CameraParameters cameraParams = new CameraParameters()
        {
            cameraResolutionHeight = m_Resolution.height,
            cameraResolutionWidth = m_Resolution.width,
            frameRate = Mathf.RoundToInt(frameRate),
            enableHolograms = false
        };

        videoCapture.StartVideoModeAsync(cameraParams, OnVideoModeStarted);
    }

    private void OnVideoModeStarted(VideoCaptureResult result)
    {
        if (result.success == false)
        {
            Debug.LogWarning("Could not start video mode.");
            return;
        }

        Debug.Log("Video capture started.");
    }

    private void OnFrameSampleAcquired(VideoCaptureSample sample)
    {
        //When copying the bytes out of the buffer, you must supply a byte[] that is appropriately sized.
        //You can reuse this byte[] until you need to resize it (for whatever reason).
        if (m_LatestImageBytes == null || m_LatestImageBytes.Length < sample.dataLength)
        {
            m_LatestImageBytes = new byte[sample.dataLength];
        }
        sample.CopyRawImageDataIntoBuffer(m_LatestImageBytes);

        //If you need to get the cameraToWorld matrix for purposes of compositing you can do it like this
        float[] cameraToWorldMatrix;
        if (sample.TryGetCameraToWorldMatrix(out cameraToWorldMatrix) == false)
        {
            return;
        }

        //If you need to get the projection matrix for purposes of compositing you can do it like this
        float[] projectionMatrix;
        if (sample.TryGetProjectionMatrix(out projectionMatrix) == false)
        {
            return;
        }

        sample.Dispose();

        FrameContainer.Instance.PopulateFrame(m_LatestImageBytes, cameraToWorldMatrix, projectionMatrix, m_Resolution);
    }
}
