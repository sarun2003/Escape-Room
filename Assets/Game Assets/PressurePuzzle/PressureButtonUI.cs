using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PressureButtonUI : MonoBehaviour
{
    [Tooltip("True = increase pressure, False = decrease pressure")]
    public bool isIncrement;

    void Awake()
    {
        var manager = FindObjectOfType<PressurePuzzleManager>();
        GetComponent<Button>().onClick.AddListener(
            isIncrement ? manager.OnIncrease : manager.OnDecrease
        );
    }
}
