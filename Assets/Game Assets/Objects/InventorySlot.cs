using UnityEngine;
using UnityEngine.UI;


class InventorySlot : MonoBehaviour
{
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public Camera camera;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    public RenderTexture rt;
    public GameObject position;
    public int index = 0;
    void Start()
    {
        rt = new RenderTexture(1920, 1080, 24);
        Camera renderCam = camera; // The camera rendering the object/scene
        renderCam.targetTexture = rt;
        GetComponent<RawImage>().texture = rt;
    }

    void Update()
    {
       
    }
}