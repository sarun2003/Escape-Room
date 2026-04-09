
using UnityEngine;
using TMPro;

public class PanelInteraction : MonoBehaviour
{
    [Header("Settings")]
    public float interactDistance = 4f;
    public KeyCode interactKey = KeyCode.E;

    [Header("UI")]
    public TextMeshProUGUI promptText;
    public TextMeshProUGUI statusText;

    private Transform _playerCam;
    private bool _solved = false;
    private bool _failed = false;

    private void Start()
    {
        var cam = Camera.main;
        if (cam) _playerCam = cam.transform;
        if (promptText) promptText.gameObject.SetActive(false);
        if (statusText) statusText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_solved || _failed) return;
        if (_playerCam == null) return;

        // Raycast from camera center
        Ray ray = new Ray(_playerCam.position, _playerCam.forward);
        bool hitting = Physics.Raycast(ray, out RaycastHit hit, interactDistance);

        if (hitting && hit.transform.IsChildOf(transform) || 
            hitting && hit.transform == transform ||
            Vector3.Distance(transform.position, _playerCam.position) < interactDistance)
        {
            // Show prompt
            if (promptText)
            {
                promptText.gameObject.SetActive(true);
                promptText.text = "[E] Interact with Electrical Panel";
            }

            if (Input.GetKeyDown(interactKey))
            {
                SolvePuzzle();
            }
        }
        else
        {
            if (promptText) promptText.gameObject.SetActive(false);
        }
    }

    private void SolvePuzzle()
    {
        _solved = true;

        if (statusText)
        {
            statusText.gameObject.SetActive(true);
            statusText.text = "POWER RESTORED - Timer extended!";
            statusText.color = Color.green;
        }

        if (promptText) promptText.gameObject.SetActive(false);

        // Extend timer
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ExtendTimer(300f);
            GameManager.Instance.OnPuzzleCompleted("electrical_panel");
        }

        Debug.Log("[PanelInteraction] Puzzle solved! Timer extended by 5 minutes.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
