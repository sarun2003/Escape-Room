// WireConnector.cs
// Attach to each wire-plug GameObject on the LEFT side of the panel.
// The player clicks a left plug, then clicks a right socket to connect them.
// Place in Assets/Scripts/

using UnityEngine;
using UnityEngine.EventSystems;

namespace EscapeRoom
{
    /// <summary>
    /// Represents one draggable wire plug on the left side of the panel.
    /// Notifies ElectricalPanelController when selected and when dropped on a socket.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class WireConnector : MonoBehaviour
    {
        [Header("Wire Identity")]
        [Tooltip("The color this wire plug represents.")]
        public WireColor wireColor = WireColor.Red;

        [Header("Visuals")]
        [Tooltip("The LineRenderer that draws this wire. Optional — used to show a drawn line.")]
        public LineRenderer lineRenderer;

        [Tooltip("Material tint applied when this wire is selected.")]
        public Color selectedHighlight = Color.white;

        // Internal state
        private bool _isSelected = false;
        private ElectricalPanelController _panel;
        private Color _originalColor;
        private Renderer _renderer;

        private void Awake()
        {
            _panel = GetComponentInParent<ElectricalPanelController>();
            _renderer = GetComponent<Renderer>();
            if (_renderer != null)
                _originalColor = _renderer.material.color;
        }

        /// <summary>Called by ElectricalPanelController to mark this wire as selected.</summary>
        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            if (_renderer != null)
                _renderer.material.color = selected ? selectedHighlight : _originalColor;
        }

        private void OnMouseDown()
        {
            if (_panel == null) return;
            if (_panel.CurrentState != ElectricalPuzzleState.Active) return;

            _panel.OnWirePlugSelected(this);
        }
    }
}
