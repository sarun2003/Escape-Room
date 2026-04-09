using UnityEngine;

namespace EscapeRoom
{
    [RequireComponent(typeof(Collider))]
    public class PanelInteractTrigger : MonoBehaviour
    {
        [Tooltip("The ElectricalPanelController to unlock when player enters.")]
        public ElectricalPanelController panel;

        private void Awake()
        {
            // Make sure the collider is a trigger
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (panel == null) return;

            panel.UnlockPanel();

            // One-shot: disable after first activation
            gameObject.SetActive(false);
        }
    }
}
