// ElectricalPanelController.cs
// The central FSM and puzzle logic for the Among-Us-style wire panel.
//
// FSM: Locked → Active → Solved → Unlocked
//                      ↘ Failed  (triggers TriggerInstantDeath)
//
// Place in Assets/Scripts/
// Attach to the root "ElectricalPanel" GameObject.
// Assign WireConnector (plugs) and WireSocket (sockets) children via Inspector.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EscapeRoom
{
    public class ElectricalPanelController : MonoBehaviour
    {
        // ──────────────────────────────────────────────────────────
        // Inspector fields
        // ──────────────────────────────────────────────────────────

        [Header("FSM")]
        [Tooltip("Set to Locked. Flip to Active at runtime via UnlockPanel().")]
        [SerializeField] private ElectricalPuzzleState _startState = ElectricalPuzzleState.Locked;

        [Header("Puzzle Setup")]
        [Tooltip("All LEFT-side wire plugs (WireConnector components).")]
        public List<WireConnector> wirePlugs = new List<WireConnector>();

        [Tooltip("All RIGHT-side sockets (WireSocket components).")]
        public List<WireSocket>    wireSockets = new List<WireSocket>();

        [Header("Interaction")]
        [Tooltip("How close the player must be to interact with the panel (metres).")]
        public float interactRadius = 2.5f;

        [Tooltip("The prompt text shown when player is in range.")]
        public string interactPrompt = "[E] Examine Electrical Panel";

        [Header("Timer Extension")]
        [Tooltip("Extra seconds added to the countdown on success.")]
        public float timerExtensionSeconds = 300f; // 5 min → 10 min = +5 min

        [Header("Audio (optional — hooks into Nicolas's SoundManager)")]
        public string sfxConnect   = "wire_connect";
        public string sfxSuccess   = "puzzle_success";
        public string sfxFail      = "explosion";
        public string sfxUnlock    = "panel_unlock";
        public string sfxWrongWire = "wire_wrong";

        [Header("Visual Feedback")]
        [Tooltip("Panel light that glows green on solve, red on fail.")]
        public Light statusLight;
        public Color lightSolved = Color.green;
        public Color lightFailed = Color.red;
        public Color lightActive = new Color(1f, 0.8f, 0f); // amber

        [Tooltip("Optional particle system that fires on solve.")]
        public ParticleSystem solveParticles;

        [Header("World-space UI (Canvas on panel face)")]
        [Tooltip("The Canvas that shows the wire puzzle UI. Hidden when Locked/Unlocked.")]
        public Canvas puzzleCanvas;

        [Tooltip("TextMeshPro label that shows puzzle status.")]
        public TextMeshProUGUI statusLabel;

        // ──────────────────────────────────────────────────────────
        // Runtime state
        // ──────────────────────────────────────────────────────────

        public ElectricalPuzzleState CurrentState { get; private set; }

        private WireConnector _selectedPlug = null;

        // Dictionary: color → socket that currently has that color plugged in
        private Dictionary<WireColor, WireSocket> _pluggedMap = new Dictionary<WireColor, WireSocket>();

        private Transform _playerTransform;
        private bool _playerInRange = false;
        private bool _canvasWasOpen = false;

        // ──────────────────────────────────────────────────────────
        // Unity lifecycle
        // ──────────────────────────────────────────────────────────

        private void Start()
        {
            // Find player (First Person Controller)
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj) _playerTransform = playerObj.transform;

            TransitionTo(_startState);

            if (puzzleCanvas) puzzleCanvas.gameObject.SetActive(false);
        }

        private void Update()
        {
            CheckPlayerProximity();

            if (_playerInRange && CurrentState == ElectricalPuzzleState.Active)
            {
                if (Input.GetKeyDown(KeyCode.E))
                    OpenPuzzleCanvas();
                if (Input.GetKeyDown(KeyCode.Escape))
                    ClosePuzzleCanvas();
            }
        }

        // ──────────────────────────────────────────────────────────
        // Proximity
        // ──────────────────────────────────────────────────────────

        private void CheckPlayerProximity()
        {
            if (_playerTransform == null) return;
            float dist = Vector3.Distance(transform.position, _playerTransform.position);
            bool inRange = dist <= interactRadius;

            if (inRange != _playerInRange)
            {
                _playerInRange = inRange;
                // You can hook this into a HUD prompt system here
            }
        }

        // ──────────────────────────────────────────────────────────
        // Public API
        // ──────────────────────────────────────────────────────────

        /// <summary>Call this from GameManager or a trigger volume to allow the player to start.</summary>
        public void UnlockPanel()
        {
            if (CurrentState == ElectricalPuzzleState.Locked)
                TransitionTo(ElectricalPuzzleState.Active);
        }

        // ──────────────────────────────────────────────────────────
        // Wire interaction (called by WireConnector / WireSocket)
        // ──────────────────────────────────────────────────────────

        public void OnWirePlugSelected(WireConnector plug)
        {
            if (CurrentState != ElectricalPuzzleState.Active) return;

            // Deselect previous
            if (_selectedPlug != null) _selectedPlug.SetSelected(false);

            _selectedPlug = plug;
            _selectedPlug.SetSelected(true);

            PlaySFX(sfxConnect);
            UpdateStatusLabel("Select a socket →");
        }

        public void OnSocketClicked(WireSocket socket)
        {
            if (CurrentState != ElectricalPuzzleState.Active) return;
            if (_selectedPlug == null) return;

            WireColor plugColor = _selectedPlug.wireColor;

            // Remove any existing wire from this socket
            if (socket.PluggedColor != WireColor.None)
                _pluggedMap.Remove(socket.PluggedColor);

            // Remove this color from any previously connected socket
            if (_pluggedMap.TryGetValue(plugColor, out WireSocket oldSocket))
            {
                oldSocket.SetPluggedWire(WireColor.None);
                _pluggedMap.Remove(plugColor);
            }

            // Connect
            socket.SetPluggedWire(plugColor);
            _pluggedMap[plugColor] = socket;

            // Deselect plug
            _selectedPlug.SetSelected(false);
            _selectedPlug = null;

            // Check win / immediate wrong-wire fail
            EvaluateConnections();
        }

        // ──────────────────────────────────────────────────────────
        // Evaluation
        // ──────────────────────────────────────────────────────────

        private void EvaluateConnections()
        {
            int totalSockets  = wireSockets.Count;
            int filledSockets = 0;
            int correctSockets = 0;

            foreach (var socket in wireSockets)
            {
                if (socket.PluggedColor != WireColor.None)
                {
                    filledSockets++;
                    if (socket.IsCorrect) correctSockets++;
                    else
                    {
                        // Wrong connection — immediate failure (Among Us style)
                        PlaySFX(sfxWrongWire);
                        StartCoroutine(DelayedFail(1.5f));
                        UpdateStatusLabel("⚠ WRONG CONNECTION!");
                        return;
                    }
                }
            }

            if (filledSockets == totalSockets && correctSockets == totalSockets)
            {
                // All wires correct!
                TransitionTo(ElectricalPuzzleState.Solved);
            }
            else
            {
                UpdateStatusLabel($"Connections: {correctSockets}/{totalSockets}");
            }
        }

        private IEnumerator DelayedFail(float delay)
        {
            yield return new WaitForSeconds(delay);
            TransitionTo(ElectricalPuzzleState.Failed);
        }

        // ──────────────────────────────────────────────────────────
        // FSM transitions
        // ──────────────────────────────────────────────────────────

        private void TransitionTo(ElectricalPuzzleState newState)
        {
            CurrentState = newState;

            switch (newState)
            {
                case ElectricalPuzzleState.Locked:
                    SetStatusLight(Color.gray);
                    ClosePuzzleCanvas();
                    UpdateStatusLabel("OFFLINE");
                    break;

                case ElectricalPuzzleState.Active:
                    SetStatusLight(lightActive);
                    PlaySFX(sfxUnlock);
                    UpdateStatusLabel("Connect the wires");
                    break;

                case ElectricalPuzzleState.Solved:
                    SetStatusLight(lightSolved);
                    PlaySFX(sfxSuccess);
                    UpdateStatusLabel("✔ POWER RESTORED");
                    if (solveParticles) solveParticles.Play();
                    ClosePuzzleCanvas();
                    OnSolved();
                    break;

                case ElectricalPuzzleState.Failed:
                    SetStatusLight(lightFailed);
                    PlaySFX(sfxFail);
                    UpdateStatusLabel("CRITICAL FAILURE");
                    ClosePuzzleCanvas();
                    OnFailed();
                    break;

                case ElectricalPuzzleState.Unlocked:
                    SetStatusLight(lightSolved);
                    break;
            }
        }

        // ──────────────────────────────────────────────────────────
        // Outcome handlers
        // ──────────────────────────────────────────────────────────

        private void OnSolved()
        {
            // 1. Extend the timer via GameManager
            GameManager gm = GameManager.Instance;
            if (gm != null)
            {
                gm.ExtendTimer(timerExtensionSeconds);   // +5 min (5→10)
                gm.OnPuzzleCompleted("electrical_panel");
            }

            // Transition to final inert state after a beat
            StartCoroutine(DelayedTransition(ElectricalPuzzleState.Unlocked, 2f));
        }

        private void OnFailed()
        {
            // Trigger explosion / flood lose state via GameManager
            GameManager gm = GameManager.Instance;
            if (gm != null)
                gm.TriggerInstantDeath("Electrical overload — the facility explodes!");
        }

        private IEnumerator DelayedTransition(ElectricalPuzzleState state, float delay)
        {
            yield return new WaitForSeconds(delay);
            TransitionTo(state);
        }

        // ──────────────────────────────────────────────────────────
        // Canvas helpers
        // ──────────────────────────────────────────────────────────

        private void OpenPuzzleCanvas()
        {
            if (puzzleCanvas && !_canvasWasOpen)
            {
                puzzleCanvas.gameObject.SetActive(true);
                _canvasWasOpen = true;
                // Lock cursor so player can click wires
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void ClosePuzzleCanvas()
        {
            if (puzzleCanvas && _canvasWasOpen)
            {
                puzzleCanvas.gameObject.SetActive(false);
                _canvasWasOpen = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        // ──────────────────────────────────────────────────────────
        // Helpers
        // ──────────────────────────────────────────────────────────

        private void SetStatusLight(Color color)
        {
            if (statusLight != null)
            {
                statusLight.color = color;
                statusLight.enabled = true;
            }
        }

        private void UpdateStatusLabel(string text)
        {
            if (statusLabel != null)
                statusLabel.text = text;
        }

        private void PlaySFX(string clipName)
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(clipName, transform.position, 1f);
        }

        // ──────────────────────────────────────────────────────────
        // Gizmo — show interact radius in Scene view
        // ──────────────────────────────────────────────────────────
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRadius);
        }
    }
}
