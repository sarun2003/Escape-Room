using UnityEngine;

public class WrenchScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other){
        var gauge = other.GetComponent<PressureGaugeInteract>();
        if (gauge != null)
            gauge.OnInteract();
    }
}
