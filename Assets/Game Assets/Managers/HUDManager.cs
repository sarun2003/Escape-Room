using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; } //used for timers and states
    public GameObject reticle;
    public List<GameObject> InventorySlots;
    public List<RenderTexture> RTInventorySlots;
    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 

        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void Update()
    {
        if (GameManager.Instance.PlayerHoveringObject || GameManager.Instance.CurrentPlayerInputState == PlayerInputState.HOLDING)
        {
            reticle.GetComponent<Image>().color = Color.white;
        } else
        {
            reticle.GetComponent<Image>().color = Color.black;
        }
    }
}