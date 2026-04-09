using UnityEngine;

namespace EscapeRoom
{
    public class WireColorInitializer : MonoBehaviour
    {
        [Header("Assign your plugs and sockets in order: Red, Blue, Yellow, Green")]
        public WireConnector[] plugs;    // RedPlug, BluePlug, YellowPlug, GreenPlug
        public WireSocket[]    sockets;  // RedSocket, BlueSocket, YellowSocket, GreenSocket

        private static readonly Color[] WireColors = new Color[]
        {
            new Color(0.9f, 0.1f, 0.1f),   // Red
            new Color(0.1f, 0.4f, 0.9f),   // Blue
            new Color(0.95f, 0.85f, 0.1f), // Yellow
            new Color(0.1f, 0.8f, 0.2f),   // Green
        };

        void Awake()
        {
            for (int i = 0; i < plugs.Length && i < WireColors.Length; i++)
            {
                if (plugs[i] == null) continue;
                ApplyColor(plugs[i].GetComponent<Renderer>(), WireColors[i]);
            }

            for (int i = 0; i < sockets.Length && i < WireColors.Length; i++)
            {
                if (sockets[i] == null) continue;

                // Color the socket renderer
                Renderer rend = sockets[i].socketRenderer != null
                    ? sockets[i].socketRenderer
                    : sockets[i].GetComponent<Renderer>();

                ApplyColor(rend, WireColors[i] * 0.5f); // darker shade so plug pops
            }
        }

        void ApplyColor(Renderer rend, Color color)
        {
            if (rend == null) return;

            // Works with URP Lit and Standard shader
            Material mat = new Material(rend.sharedMaterial != null
                ? rend.sharedMaterial
                : new Material(Shader.Find("Universal Render Pipeline/Lit")));

            mat.color = color;

            // URP base color property
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", color);

            rend.material = mat;
        }
    }
}
