// FalseWall.cs
// Attach to the wall segment that hides the Room 3 entrance.
// The mesh looks identical to Room 1's walls (same material).
// When the player walks into it, the collider is disabled so they pass through.
// The mesh stays visible from outside — it only becomes passable from the Room 1 side.
//
// Place in Assets/Scripts/
// Attach to your "FalseWall" GameObject (a thin box with the rusty material).

using System.Collections;
using UnityEngine;

public class FalseWall : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How close the player must be to the wall center to trigger passthrough (metres).")]
    public float passRadius = 1.2f;

    [Tooltip("Seconds to wait before re-enabling the collider after player moves away.")]
    public float recollideDelay = 1.5f;

    [Tooltip("Optional: play a subtle creak/hiss SFX when first discovered.")]
    public string discoverySFX = "door_creak";

    private Collider _col;
    private Transform _playerTransform;
    private bool _discovered = false;
    private bool _open = false;

    private void Start()
    {
        _col = GetComponent<Collider>();
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj) _playerTransform = playerObj.transform;
    }

    private void Update()
    {
        if (_playerTransform == null) return;

        float dist = Vector3.Distance(transform.position, _playerTransform.position);
        bool playerNear = dist <= passRadius;

        if (playerNear && !_open)
        {
            OpenWall();
        }
        else if (!playerNear && _open)
        {
            StartCoroutine(CloseWallDelayed());
        }
    }

    private void OpenWall()
    {
        _open = true;
        _col.enabled = false;

        if (!_discovered)
        {
            _discovered = true;
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(discoverySFX, transform.position, 1f);
        }
    }

    private IEnumerator CloseWallDelayed()
    {
        _open = false;
        yield return new WaitForSeconds(recollideDelay);
        // Only re-enable if the player is still away
        if (_playerTransform != null)
        {
            float dist = Vector3.Distance(transform.position, _playerTransform.position);
            if (dist > passRadius)
                _col.enabled = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
        Gizmos.DrawSphere(transform.position, passRadius);
    }
}
