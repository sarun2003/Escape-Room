using UnityEngine;

public class GenericSounds : MonoBehaviour
{
    public float SoundInterval = 0;
    public float MinImpactForce = 2f;   // Threshold for triggering sound
    public float MinVelocity = 0.5f;    // Threshold for sliding sounds
    public float volume = 1f;
    public SFXType type = SFXType.Metal;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        TryPlayImpactSound(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        TryPlaySlidingSound(collision);
    }

    void Update()
    {
        if (SoundInterval > 0)
        {
            SoundInterval -= Time.deltaTime;
        }
    }

    void TryPlayImpactSound(Collision collision)
    {
        if (SoundInterval > 0) return;

        float impact = collision.relativeVelocity.magnitude;
        if (impact >= MinImpactForce)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.SFXtoString(type), transform.position, volume);
            SoundInterval = 0.15f; // cooldown
        }
    }

    void TryPlaySlidingSound(Collision collision)
    {
        if (SoundInterval > 0) return;

        // Only play if the object is moving along the surface
        Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (horizontalVel.magnitude >= MinVelocity)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.SFXtoString(type), transform.position, volume);
            SoundInterval = 0.25f; // slightly longer cooldown for sliding
        }
    }
}