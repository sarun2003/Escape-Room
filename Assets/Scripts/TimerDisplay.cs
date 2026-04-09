// TimerDisplay.cs
// Attach to the TimerText GameObject inside HUD/Canvas.
// Reads TimeLeft from GameManager and displays it as MM:SS.
// Place in Assets/Scripts/

using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private void Awake()
    {
        if (timerText == null)
            timerText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;

        float timeLeft = Mathf.Max(0, GameManager.Instance.TimeLeft);
        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60f);

        timerText.text = string.Format("{0}:{1:00}", minutes, seconds);

        // Turn red when under 60 seconds
        if (timeLeft <= 60f)
            timerText.color = Color.red;
        else
            timerText.color = Color.white;
    }
}
