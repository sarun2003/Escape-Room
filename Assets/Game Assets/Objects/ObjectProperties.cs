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
    public bool m_attractsGem = false;

    [Header("Outline Properties")]
    [SerializeField]
    private ObjectTypeColor m_colorType;
    [SerializeField]
    private Color m_customColor;
    [SerializeField]
    private float hoverRadius = 10f;
    public Vector3 camPos;


    [Header("Other")]
    [SerializeField]
    private bool ignorePlayerCollisions = false;

    private Outline outline;
    private GameObject gem = null;
    private Rigidbody rb = null;

    public virtual void Start()
    {   
        
        if (!gameObject.CompareTag("Gem")) 
        {
            gem = GameObject.FindWithTag("Gem");
            rb = GetComponent<Rigidbody>();
        }
        
        m_self = new()
        {
            m_gameObject = gameObject
        };
        if (m_colorType == ObjectTypeColor.OTHER)
        {
            m_self.SetCustomColor(m_customColor);
        } else
        {
            m_self.SetColorType(m_colorType);
        }
        m_self.DropObject();
        

        if (ignorePlayerCollisions)
        {
            gameObject.layer = LayerMask.NameToLayer("ObjectsIC");
        }
    }

    // Update is called once per frame
    public virtual void UpdateObjectOutline()
    {
        bool shouldHaveOutline = (m_hovered && GameManager.Instance.CurrentPlayerInputState != PlayerInputState.HOLDING) || (m_self.m_state == ObjectState.HELD && GameManager.Instance.CurrentPlayerInputState == PlayerInputState.HOLDING);

        if (shouldHaveOutline)
        {
            if (outline == null) // Only add once
            {
                outline = gameObject.AddComponent<Outline>();
                outline.enabled = false;
                outline.OutlineMode = Outline.Mode.OutlineAll;
                outline.OutlineWidth = hoverRadius;
                
            }
            
            outline.OutlineColor = m_self.GetColor();
            outline.enabled = true;
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

    public virtual void Update()
    {
        
    }

    public virtual void UpdateObjectAttraction()
    {
        if (gem != null)
        {
            if (m_magneticAttraction > 0 && gem.activeSelf)
            {   
                Vector3 direction = (gem.transform.position - transform.position);
                float distance = direction.magnitude;

                
                direction.Normalize();

                
                float forceStrength = m_magneticAttraction / Mathf.Max(distance, 0.1f);

                rb.AddForce(direction * forceStrength, ForceMode.Acceleration);

                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                rb.interpolation = RigidbodyInterpolation.Interpolate;

                rb.angularVelocity *= 0.95f;
                rb.linearVelocity *= 0.95f;
            }
        }
    }

    public virtual void FixedUpdate()
    {
        UpdateObjectAttraction();
        UpdateObjectOutline();
    }
}
