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
    public static PressurePuzzleManager Instance { get; private set; }

    [Header("Puzzle Settings")]
    public float minPressure = 0f;
    public float maxPressure = 100f;
    public float targetPressure = 65f;
    public float pressureStep = 5f;

    public PressurePuzzleState currentState { get; private set; } = PressurePuzzleState.INACTIVE;
    public PressureGauge gauge { get; private set; }

    public event System.Action<PressurePuzzleState> OnStateChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CreateNewGauge();
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        if (gauge != null)
            gauge.OnTargetReached -= HandleTargetReached;
    }

    void CreateNewGauge()
    {
        if (gauge != null)
            gauge.OnTargetReached -= HandleTargetReached;

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

    public void OnPlayerClose()
    {
        if (currentState != PressurePuzzleState.ACTIVE) return;

        TransitionTo(PressurePuzzleState.INACTIVE);
        CreateNewGauge();
    }

    void HandleTargetReached()
    {
        if (currentState != PressurePuzzleState.ACTIVE) return;
        TransitionTo(PressurePuzzleState.SOLVED);
    }

    void TransitionTo(PressurePuzzleState next)
    {
        currentState = next;
        OnStateChanged?.Invoke(currentState);

        switch (currentState)
        {
            case PressurePuzzleState.OPENING:
                StartCoroutine(OpenRoutine());
                break;

            case PressurePuzzleState.SOLVED:
                OnPuzzleSolved();
                break;
        }
    }

    IEnumerator OpenRoutine()
    {
        // Let UI respond via OnStateChanged
        yield return null;

        TransitionTo(PressurePuzzleState.ACTIVE);
    }

    void OnPuzzleSolved()
    {
        Debug.Log("Pressure puzzle solved!");
    }
}