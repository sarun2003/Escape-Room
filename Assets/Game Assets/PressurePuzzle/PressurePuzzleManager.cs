using UnityEngine;
using System.Collections;

public enum PressurePuzzleState
{
    INACTIVE,
    OPENING,
    ACTIVE,
    SOLVED
}

public class PressurePuzzleManager : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public float minPressure    = 0f;
    public float maxPressure    = 100f;
    public float targetPressure = 65f;
    public float pressureStep   = 5f;

    public PressurePuzzleState currentState { get; private set; } = PressurePuzzleState.INACTIVE;
    public PressureGauge gauge { get; private set; }

    public event System.Action<PressurePuzzleState> OnStateChanged;

    void Awake()
    {
        gauge = new PressureGauge(minPressure, maxPressure, targetPressure, pressureStep);
        gauge.OnTargetReached += HandleTargetReached;
    }

    public void OnPlayerInteract()
    {
        if (currentState != PressurePuzzleState.INACTIVE) return;
        TransitionTo(PressurePuzzleState.OPENING);
    }

    public void OnIncrease()
    {
        if (currentState != PressurePuzzleState.ACTIVE) return;
        gauge.Increase();
    }

    public void OnDecrease()
    {
        if (currentState != PressurePuzzleState.ACTIVE) return;
        gauge.Decrease();
    }

    void HandleTargetReached()
    {
        if (currentState != PressurePuzzleState.ACTIVE) return;
        TransitionTo(PressurePuzzleState.SOLVED);
    }

    void TransitionTo(PressurePuzzleState next)
    {
        // on exit
        switch (currentState)
        {
            case PressurePuzzleState.ACTIVE:
                EnableButtons(false);
                break;
        }

        currentState = next;
        OnStateChanged?.Invoke(currentState);

        // on enter
        switch (currentState)
        {
            case PressurePuzzleState.OPENING:
                StartCoroutine(OpenRoutine());
                break;
            case PressurePuzzleState.ACTIVE:
                EnableButtons(true);
                break;
            case PressurePuzzleState.SOLVED:
                OnPuzzleSolved();
                break;
        }
    }

    IEnumerator OpenRoutine()
    {
        // FindObjectOfType<PressurePuzzleUI>().Open(); // uncomment to trigger UI open
        yield return null; // replace with yield return UI open animation coroutine
        TransitionTo(PressurePuzzleState.ACTIVE);
    }

    void EnableButtons(bool on)
    {
        // FindObjectOfType<PressurePuzzleUI>().SetButtonsInteractable(on);
    }

    void OnPuzzleSolved()
    {
        // play solve VFX, notify other game systems, etc.
        Debug.Log("Pressure puzzle solved!");
    }
}
