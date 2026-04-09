using UnityEngine;

namespace EscapeRoom
{
    [RequireComponent(typeof(Collider))]
    public class WireConnector : MonoBehaviour
    {
        [Header("Wire Identity")]
        public WireColor wireColor = WireColor.Red;

        [Header("Visuals")]
        public LineRenderer lineRenderer;
        public Color selectedHighlight = Color.white;

        // State
        public bool IsConnected { get; private set; } = false;

        private Renderer _renderer;
        private Color _originalColor;

        void Awake()
        {
            _renderer = GetComponent<Renderer>();
            if (_renderer != null)
                _originalColor = _renderer.material.color;
        }

        /// <summary>Highlight or un-highlight this plug when selected.</summary>
        public void SetSelected(bool selected)
        {
            if (_renderer != null)
                _renderer.material.color = selected ? selectedHighlight : _originalColor;
        }

        /// <summary>
        /// Called by ElectricalPanelController once this plug is dropped into a socket.
        /// Draws a LineRenderer between plug and socket.
        /// </summary>
        public void SetConnected(WireSocket socket)
        {
            IsConnected = true;
            SetSelected(false);

            if (lineRenderer != null)
            {
                // Ensure the line has a visible material (uses Unity's built-in default)
                if (lineRenderer.material == null || lineRenderer.material.name.Contains("Default-Line") == false)
                    lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

                lineRenderer.enabled = true;
                lineRenderer.positionCount = 2;
                lineRenderer.widthMultiplier = 0.05f;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, socket.transform.position);

                Color c = WireColorToUnityColor(wireColor);
                lineRenderer.startColor = c;
                lineRenderer.endColor   = c;
            }
        }

        /// <summary>Converts WireColor enum to a Unity Color for line rendering.</summary>
        public static Color WireColorToUnityColor(WireColor wc)
        {
            return wc switch
            {
                WireColor.Red    => Color.red,
                WireColor.Blue   => Color.blue,
                WireColor.Yellow => Color.yellow,
                WireColor.Green  => Color.green,
                WireColor.Orange => new Color(1f, 0.5f, 0f),
                _                => Color.white
            };
        }
    }
}
