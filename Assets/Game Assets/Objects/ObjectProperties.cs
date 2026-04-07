using UnityEngine;

public class ObjectProperties : MonoBehaviour
{
    //Higher = Lighter
    //Sorry
    [Header("Item Properties")]
    public float m_velocity = 10f;
    public Item m_self;
    public bool m_canBeStored = true;
    public bool m_hovered = false;

    [Header("Gem Properties")]
    public float m_magneticAttraction = 0;

    [Header("Outline Properties")]
    [SerializeField]
    private ObjectTypeColor m_colorType;
    [SerializeField]
    private float hoverRadius = 10f;


    [Header("Other")]
    [SerializeField]
    private bool ignorePlayerCollisions = false;

    private Outline outline;
    private GameObject gem = null;
    private Rigidbody rb = null;

    void Start()
    {   
        if (!gameObject.CompareTag("Gem")) 
        {
            gem = GameObject.FindWithTag("Gem");
            rb = GetComponent<Rigidbody>();
        }
        
        m_self = new()
        {
            m_gameObject = gameObject,
            m_type = m_colorType
        };
        m_self.DropObject();
        

        if (ignorePlayerCollisions)
        {
            gameObject.layer = LayerMask.NameToLayer("ObjectsIC");
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool shouldHaveOutline = m_hovered || m_self.m_state == ObjectState.HELD;

        if (shouldHaveOutline)
        {
            if (outline == null) // Only add once
            {
                outline = gameObject.AddComponent<Outline>();
                outline.OutlineMode = Outline.Mode.OutlineAll;
                outline.OutlineColor = m_self.GetColor();
                outline.OutlineWidth = hoverRadius;
            }
        }
        else
        {
            if (outline != null) // Only remove once
            {
                Destroy(outline);
                outline = null;
            }
        }
    }

    void FixedUpdate()
    {
        if (gem != null)
        {
            if (m_magneticAttraction > 0 && gem.activeSelf)
            {   
                Vector3 direction = (gem.transform.position - transform.position);
                float distance = direction.magnitude;

                // Normalize direction so force is consistent
                direction.Normalize();

                // Strength falls off with distance (optional but recommended)
                float forceStrength = m_magneticAttraction / Mathf.Max(distance, 0.1f);

                rb.AddForce(direction * forceStrength, ForceMode.Acceleration);

                // Physics settings (you don't need to set these every frame ideally)
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                rb.interpolation = RigidbodyInterpolation.Interpolate;

                rb.angularVelocity *= 0.95f;
                rb.linearVelocity *= 0.95f;
            }
        }
        
    }
}
