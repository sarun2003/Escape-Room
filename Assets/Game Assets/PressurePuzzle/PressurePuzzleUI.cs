using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PressurePuzzleUI : MonoBehaviour
{
    [Header("References")]
    public GameObject        panel;
    public RectTransform     needle;
    public TextMeshProUGUI   pressureText;
    public TextMeshProUGUI   targetText;
    public Button            increaseButton;
    public Button            decreaseButton;
    public Button            closeButton;

    [Header("Needle Settings")]
    public float needleMinAngle = -120f;  // rotation at min pressure
    public float needleMaxAngle =  120f;  // rotation at max pressure

    private PressurePuzzleManager manager;

    void Awake()
    {
        manager = FindObjectOfType<PressurePuzzleManager>();

        increaseButton.onClick.AddListener(manager.OnIncrease);
        decreaseButton.onClick.AddListener(manager.OnDecrease);
        closeButton.onClick.AddListener(Close);

        manager.gauge.OnPressureChanged += RefreshGaugeVisuals;
        manager.OnStateChanged += HandleStateChanged;

        panel.SetActive(false);
    }

    void OnDestroy()
    {
        manager.gauge.OnPressureChanged -= RefreshGaugeVisuals;
        manager.OnStateChanged -= HandleStateChanged;
    }

    public void Open()
    {
        targetText.text = $"Target: {manager.gauge.targetPressure:0}";
        RefreshGaugeVisuals(manager.gauge.currentPressure);
        panel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    public void Close()
    {
        panel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    public void SetButtonsInteractable(bool on)
    {
        increaseButton.interactable = on;
        decreaseButton.interactable = on;
    }

    void RefreshGaugeVisuals(float pressure)
    {
        // update needle
        float t = Mathf.InverseLerp(manager.gauge.minPressure, manager.gauge.maxPressure, pressure);
        float angle = Mathf.Lerp(needleMinAngle, needleMaxAngle, t);
        needle.localRotation = Quaternion.Euler(0f, 0f, -angle);

        // update readout
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
