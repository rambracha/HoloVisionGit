using UnityEngine;

public class ImageCreator : MonoBehaviour
{
    [SerializeField]
    private Transform PlanePrefab;

    private int m_Counter = 0;

    private void Start()
    {
        if (FrameContainer.Instance == null)
        {
            Debug.LogError("Must have a FrameContainer somewhere in the scene.");
            return;
        }

        FrameContainer.Instance.FramePopulated += CreateImagePlane;
    }

    private void OnDestroy()
    {
        if (FrameContainer.Instance != null)
        {
            FrameContainer.Instance.FramePopulated -= CreateImagePlane;
        }
    }

    private void CreateImagePlane()
    {
        if (m_Counter < 300)
        {
            m_Counter++;
            return;
        }

        UnityEngine.WSA.Application.InvokeOnAppThread(() =>
        {
            var position = gameObject.transform.position + 3 * gameObject.transform.forward;
            var rotation = Quaternion.Euler(gameObject.transform.rotation.eulerAngles.x - 90, gameObject.transform.rotation.eulerAngles.y, gameObject.transform.rotation.eulerAngles.z);

            var imagePlane = Instantiate(PlanePrefab, position, rotation);

            var tex = new Texture2D(FrameContainer.Instance.Resolution.width, FrameContainer.Instance.Resolution.height, TextureFormat.BGRA32, false);
            tex.LoadRawTextureData(FrameContainer.Instance.Image);
            tex.Apply();

            imagePlane.GetComponent<Renderer>().material.mainTexture = tex;
        }, false);

        m_Counter = 0;
    }
}
