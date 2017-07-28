using HoloToolkit.Unity;
using System;

public class FrameContainer : Singleton<FrameContainer>
{
    private byte[] m_LatestImageBytes;
    private float[] m_CameraToWorldMatrix;
    private float[] m_ProjectionMatrix;
    private HoloLensCameraStream.Resolution m_ImageResolution;
    
    public event Action FramePopulated;

    public byte[] Image
    {
        get
        {
            return m_LatestImageBytes;
        }
    }

    public float[] CameraToWorldMatrix
    {
        get
        {
            return m_CameraToWorldMatrix;
        }
    }

    public float[] ProjectionMatrix
    {
        get
        {
            return m_ProjectionMatrix;
        }
    }

    public HoloLensCameraStream.Resolution Resolution
    {
        get
        {
            return m_ImageResolution;
        }
    }

    public void PopulateFrame(byte[] latestImageBytes, float[] cameraToWorldMatrix, float[] projectionMatrix, HoloLensCameraStream.Resolution imageResolution)
    {
        m_LatestImageBytes = latestImageBytes;
        m_CameraToWorldMatrix = cameraToWorldMatrix;
        m_ProjectionMatrix = projectionMatrix;
        m_ImageResolution = imageResolution;

        if (FramePopulated != null)
        {
            FramePopulated.Invoke();
        }
    }
}
