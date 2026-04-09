// ElectricalPanelController.cs
// Among Us style: hold click on a plug, drag to socket, release to connect.
// Uses center-screen raycast + New Input System.
// Place in Assets/Scripts/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace EscapeRoom
{
    public class ElectricalPanelController : MonoBehaviour
    {
        public ElectricalPuzzleState CurrentState { get; private set; } = ElectricalPuzzleState.Locked;

        [Header("Puzzle Pieces")]
        public List<WireConnector> connectors = new();
        public List<WireSocket>    sockets    = new();

        [Header("Interaction")]
        public Camera playerCamera;

        [Header("UI")]
        public GameObject puzzleCanvas;
        public TMP_Text   statusLabel;
        public TMP_Text   promptLabel;

        [Header("Reward")]
        public GameObject wrenchPickup;

        [Header("Timer Extension")]
        public float bonusSeconds = 60f;

        // Runtime
        private WireConnector _heldConnector;  // plug currently being dragged
        private int _correctCount = 0;

        void Start()
        {
            if (puzzleCanvas != null) puzzleCanvas.SetActive(false);
            UnlockPanel();
        }

        void Update()
        {
            if (CurrentState != ElectricalPuzzleState.Active) return;

            if (Mouse.current == null) return;

            // ── PRESS: pick up a plug ─────────────────────────────────────
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                TryGrabPlug();
            }

            // ── HOLD: draw live wire preview ──────────────────────────────
            if (_heldConnector != null && Mouse.current.leftButton.isPressed)
            {
                DrawLiveWire();
            }

            // ── RELEASE: try to connect to socket ─────────────────────────
            if (Mouse.current.leftButton.wasReleasedThisFrame && _heldConnector != null)
            {
                TryConnectToSocket();
            }
        }

        public void UnlockPanel()
        {
            if (CurrentState != ElectricalPuzzleState.Locked) return;
            CurrentState = ElectricalPuzzleState.Active;
            if (puzzleCanvas != null) puzzleCanvas.SetActive(true);
            SetStatus("Hold and drag a colored wire to its matching socket!");
            SetPrompt("");
        }

        // ── Grab ─────────────────────────────────────────────────────────────

        void TryGrabPlug()
        {
            Ray ray = GetCenterRay();
            if (!Physics.Raycast(ray, out RaycastHit hit, 20f)) return;

            WireConnector conn = hit.collider.GetComponent<WireConnector>();
            if (conn == null || conn.IsConnected) return;

            // Drop previously held
            if (_heldConnector != null)
                _heldConnector.SetSelected(false);

            _heldConnector = conn;
            _heldConnector.SetSelected(true);

            Color c = WireConnector.WireColorToUnityColor(conn.wireColor);
            string hex = ColorUtility.ToHtmlStringRGB(c);
            SetStatus($"<color=#{hex}>Dragging {conn.wireColor} wire...</color>\nAim at the matching socket and release!");
        }

        // ── Live wire preview while dragging ─────────────────────────────────

        void DrawLiveWire()
        {
            if (_heldConnector.lineRenderer == null) return;

            Ray ray = GetCenterRay();
            Vector3 endPoint;

            // Snap to socket if hovering one
            if (Physics.Raycast(ray, out RaycastHit hit, 20f))
            {
                WireSocket sock = hit.collider.GetComponent<WireSocket>();
                endPoint = sock != null ? sock.transform.position : hit.point;
            }
            else
            {
                // Draw toward crosshair in world space
                endPoint = ray.origin + ray.direction * 3f;
            }

            var lr = _heldConnector.lineRenderer;
            if (!lr.enabled)
            {
                lr.enabled = true;
                lr.positionCount = 2;
                // Auto material
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.widthMultiplier = 0.05f;
                Color c = WireConnector.WireColorToUnityColor(_heldConnector.wireColor);
                lr.startColor = c;
                lr.endColor   = c;
            }
            lr.SetPosition(0, _heldConnector.transform.position);
            lr.SetPosition(1, endPoint);
        }

        // ── Release: connect ──────────────────────────────────────────────────

        void TryConnectToSocket()
        {
            Ray ray = GetCenterRay();

            if (Physics.Raycast(ray, out RaycastHit hit, 20f))
            {
                WireSocket sock = hit.collider.GetComponent<WireSocket>();
                if (sock != null && sock.PluggedColor == WireColor.None)
                {
                    ConnectWire(_heldConnector, sock);
                    return;
                }
            }

            // Released on nothing — drop wire back, keep line hidden
            _heldConnector.SetSelected(false);
            if (_heldConnector.lineRenderer != null)
                _heldConnector.lineRenderer.enabled = false;

            _heldConnector = null;
            SetStatus("Hold and drag a colored wire to its matching socket!");
        }

        void ConnectWire(WireConnector connector, WireSocket socket)
        {
            socket.SetPluggedWire(connector.wireColor);
            connector.SetConnected(socket);   // locks line in place
            _heldConnector = null;

            if (socket.IsCorrect)
            {
                _correctCount++;
                SetStatus($"✓ Correct! ({_correctCount}/4 matched)");
                if (_correctCount >= connectors.Count)
                    OnAllWiresMatched();
            }
            else
            {
                SetStatus("✗ Wrong socket! Try the other wires.");
            }
        }

        // ── Solve ─────────────────────────────────────────────────────────────

        void OnAllWiresMatched()
        {
            CurrentState = ElectricalPuzzleState.Solved;
            SetStatus("⚡ Grab the wrench! Time extended by 60 seconds!");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ExtendTimer(bonusSeconds);
                GameManager.Instance.OnPuzzleCompleted("electrical_panel");
            }

            if (wrenchPickup != null)
                wrenchPickup.SetActive(true);

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX("door_open", transform.position, 1f);

            Invoke(nameof(HideCanvas), 4f);
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        Ray GetCenterRay() => playerCamera.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f));

        void HideCanvas()
        {
            if (puzzleCanvas != null) puzzleCanvas.SetActive(false);
        }

        void SetStatus(string msg)
        {
            if (statusLabel != null) statusLabel.text = msg;
        }

        void SetPrompt(string msg)
        {
            if (promptLabel != null) promptLabel.text = msg;
        }
    }
}
