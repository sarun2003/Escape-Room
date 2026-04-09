using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PressurePuzzleUI : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;
    public RectTransform needle;
    public TextMeshProUGUI pressureText;
    public TextMeshProUGUI targetText;
    public Button increaseButton;
    public Button decreaseButton;
    public Button closeButton;

    [Header("Needle Settings")]
    public float needleMinAngle = -120f;
    public float needleMaxAngle = 120f;

    PressurePuzzleManager manager;

    void Start()
    {
        StartCoroutine(InitWhenReady());
    }

    IEnumerator InitWhenReady()
    {
        
        while (PressurePuzzleManager.Instance == null || PressurePuzzleManager.Instance.gauge == null)
            yield return null;

        manager = PressurePuzzleManager.Instance;

        // Buttons
        increaseButton.onClick.AddListener(() => manager.OnIncrease());
        decreaseButton.onClick.AddListener(() => manager.OnDecrease());

        closeButton.onClick.AddListener(() =>
        {
            manager.OnPlayerClose();
            Close();
        });

        // Events
        manager.gauge.OnPressureChanged += RefreshGaugeVisuals;
        manager.OnStateChanged += HandleStateChanged;

        panel.SetActive(false);
    }

    void OnDestroy()
    {
        if (manager != null)
        {
            if (manager.gauge != null)
                manager.gauge.OnPressureChanged -= RefreshGaugeVisuals;

            manager.OnStateChanged -= HandleStateChanged;
        }
    }

    public void Open()
    {
        if (manager == null || manager.gauge == null) return;

        targetText.text = $"Target: {manager.gauge.targetPressure:0}";
        RefreshGaugeVisuals(manager.gauge.currentPressure);

        panel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Close()
    {
        panel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetButtonsInteractable(bool on)
    {
        increaseButton.interactable = on;
        decreaseButton.interactable = on;
    }

    void RefreshGaugeVisuals(float pressure)
    {
        if (manager == null || manager.gauge == null) return;

        float t = Mathf.InverseLerp(manager.gauge.minPressure, manager.gauge.maxPressure, pressure);
        float angle = Mathf.Lerp(needleMinAngle, needleMaxAngle, t);
        needle.localRotation = Quaternion.Euler(0f, 0f, -angle);

        pressureText.text = $"{pressure:0} PSI";
    }

    void HandleStateChanged(PressurePuzzleState state)
    {
        switch (state)
        {
            case PressurePuzzleState.OPENING:
                Open();
                break;

            case PressurePuzzleState.SOLVED:
                SetButtonsInteractable(false);
                pressureText.text = "LOCKED";
                break;
        }
    }
}