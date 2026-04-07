using UnityEngine;
using UnityEngine.UI;


class InventorySlot : MonoBehaviour
{
    public Camera camera;
    public RenderTexture rt;
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