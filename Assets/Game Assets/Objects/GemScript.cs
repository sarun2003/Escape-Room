using System.Collections.Generic;
using UnityEngine;

public class GemScript : MonoBehaviour
{
    public List<GameObject> magneticObjects;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        magneticObjects = GameManager.Instance.magneticObjects;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Rigidbody gemRb = GetComponent<Rigidbody>();
        if (gemRb == null) return;

        foreach (GameObject obj in magneticObjects)
        {
            if (obj == null) continue;

            ObjectProperties props = obj.GetComponent<ObjectProperties>();
            if (props == null) continue;

            Vector3 targetPos;
            Collider col = obj.GetComponent<Collider>();
            if (col != null)
                targetPos = col.bounds.center;  // center of the collider
            else
                targetPos = obj.transform.position; // fallback

            Vector3 direction = (targetPos - transform.position).normalized;

            float distance = direction.magnitude;
            direction.Normalize();

            float forceStrength = 6 / Mathf.Max(distance, 0.1f);
            if (Vector3.Distance(transform.position, obj.transform.position) <= 2)
                gemRb.AddForce(direction * forceStrength, ForceMode.Acceleration);
        }

        gemRb.angularVelocity *= 0.95f;
        gemRb.linearVelocity *= 0.95f;

        // Physics settings (do once ideally, not every frame)
        gemRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        gemRb.interpolation = RigidbodyInterpolation.Interpolate;
    }
}
