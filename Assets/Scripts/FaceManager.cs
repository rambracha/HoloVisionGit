using UnityEngine;

#if WINDOWS_UWP
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Media.FaceAnalysis;
#endif

public class DetectFaces : MonoBehaviour
{
    [SerializeField]
    private FrameContainer m_Container;

#if WINDOWS_UWP
    private FaceDetector m_FaceDetector;
#endif

    private void Start()
    {

#if WINDOWS_UWP
        m_FaceDetector = FaceDetector.CreateAsync().AsTask().Result;
#endif

        m_Container.FramePopulated += Detect;
    }

    private void Detect()
    {

#if WINDOWS_UWP
        //List<DetectedFace> f = DetectFacesInImage().Result as List<DetectedFace>;
        //if (f.Count > 0)
        //{
        //    Debug.Assert(false, "t");
        //}
#endif

    }

#if WINDOWS_UWP
    private async Task<IList<DetectedFace>> DetectFacesInImage()
    {
        using (var stream = new InMemoryRandomAccessStream())
        {
            await stream.WriteAsync(m_Container.Image.AsBuffer());
            var decoder = await BitmapDecoder.CreateAsync(stream);
            var bitmap = await decoder.GetSoftwareBitmapAsync();
            return await m_FaceDetector.DetectFacesAsync(bitmap);
        }
    }
#endif

}
