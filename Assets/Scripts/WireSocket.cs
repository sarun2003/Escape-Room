// WireSocket.cs
// Attach to each socket GameObject on the RIGHT side of the electrical panel.
// The correct wire color for each socket is set in the Inspector.
// Place in Assets/Scripts/

using UnityEngine;

namespace EscapeRoom
{
    /// <summary>
    /// A socket on the right side of the panel.
    /// Records which wire color is currently plugged in, and whether it is correct.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class WireSocket : MonoBehaviour
    {
        [Header("Socket Identity")]
        [Tooltip("The wire color that CORRECTLY belongs in this socket.")]
        public WireColor correctColor = WireColor.Red;

        [Header("Visuals")]
        [Tooltip("Renderer to tint green/red on connect.")]
        public Renderer socketRenderer;

        public Color correctColor3D = Color.green;
        public Color wrongColor3D   = Color.red;
        public Color emptyColor3D   = Color.gray;

        // Runtime
        public WireColor PluggedColor { get; private set; } = WireColor.None;
        public bool IsCorrect => PluggedColor != WireColor.None && PluggedColor == correctColor;

        private ElectricalPanelController _panel;

        private void Awake()
        {
            _panel = GetComponentInParent<ElectricalPanelController>();
            RefreshVisual();
        }

        private void OnMouseDown()
        {
            if (_panel == null) return;
            if (_panel.CurrentState != ElectricalPuzzleState.Active) return;

            _panel.OnSocketClicked(this);
        }

        /// <summary>Plug a wire into this socket (or clear it with WireColor.None).</summary>
        public void SetPluggedWire(WireColor color)
        {
            PluggedColor = color;
            RefreshVisual();
        }

        private void RefreshVisual()
        {
            if (socketRenderer == null) return;
            if (PluggedColor == WireColor.None)
                socketRenderer.material.color = emptyColor3D;
            else if (IsCorrect)
                socketRenderer.material.color = correctColor3D;
            else
                socketRenderer.material.color = wrongColor3D;
        }
    }
}
