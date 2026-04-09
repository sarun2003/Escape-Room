using UnityEngine;
namespace EscapeRoom
{
    [RequireComponent(typeof(Collider))]
    public class WireSocket : MonoBehaviour
    {
        [Header("Socket Identity")]
        public WireColor correctColor = WireColor.Red;

        [Header("Visuals")]
        public Renderer socketRenderer;
        public Material correctColor3D;
        public Material wrongColor3D;
        public Material emptyColor3D;

        // Runtime
        public WireColor PluggedColor { get; private set; } = WireColor.None;
        public bool IsCorrect => PluggedColor != WireColor.None && PluggedColor == correctColor;

        void Awake()
        {
            ApplyMaterial(emptyColor3D);
        }

        /// <summary>Set which wire is plugged in (or WireColor.None to clear).</summary>
        public void SetPluggedWire(WireColor color)
        {
            PluggedColor = color;

            if (color == WireColor.None)
                ApplyMaterial(emptyColor3D);
            else if (IsCorrect)
                ApplyMaterial(correctColor3D);
            else
                ApplyMaterial(wrongColor3D);
        }

        private void ApplyMaterial(Material mat)
        {
            if (socketRenderer != null && mat != null)
                socketRenderer.material = mat;
        }
    }
}