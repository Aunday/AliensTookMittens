using UnityEngine;

public class TextActive : MonoBehaviour
{
    //some of this is credit of TheGering http://answers.unity3d.com/questions/542646/3d-text-strokeoutline.html

    private int doubleResolution = 1024;
    public MeshRenderer shadow0;
    public MeshRenderer shadow1;
    //public TextMesh textMesh;
    public MeshRenderer meshRenderer;
    private int sortingOrder = 90;

    void Start()
    {
        meshRenderer.sortingOrder = sortingOrder;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        bool doublePixel = (Screen.width > doubleResolution || Screen.height > doubleResolution);

        Vector3 pixelOffset = new Vector3(1.0f, 1.0f, 0.0f) * (doublePixel ? 2.0f : 1.0f);
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint + pixelOffset);
        shadow0.transform.position = worldPoint;
        shadow0.sortingOrder = sortingOrder - 1;
        pixelOffset = new Vector3(-1.0f, -1.0f, 0.0f) * (doublePixel ? 2.0f : 1.0f);
        worldPoint = Camera.main.ScreenToWorldPoint(screenPoint + pixelOffset);
        shadow1.transform.position = worldPoint;
        shadow1.sortingOrder = sortingOrder - 1;
    }
}