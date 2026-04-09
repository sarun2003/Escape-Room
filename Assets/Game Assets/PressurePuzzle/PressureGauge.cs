using UnityEngine;

public class PressureGauge
{
    public float currentPressure { get; private set; }
    public float targetPressure  { get; private set; }
    public float minPressure     { get; private set; }
    public float maxPressure     { get; private set; }
    public float step            { get; private set; }

    public event System.Action<float> OnPressureChanged;
    public event System.Action        OnTargetReached;

    public PressureGauge(float min, float max, float target, float step)
    {
        minPressure     = min;
        maxPressure     = max;
        targetPressure  = target;
        currentPressure = min;
        this.step       = step;
    }

    public void Increase()
    {
        SetPressure(currentPressure + step);
    }

    public void Decrease()
    {
        SetPressure(currentPressure - step);
    }

    void SetPressure(float value)
    {
        currentPressure = Mathf.Clamp(value, minPressure, maxPressure);
        OnPressureChanged?.Invoke(currentPressure);

        if (Mathf.Approximately(currentPressure, targetPressure))
            OnTargetReached?.Invoke();
    }
}
