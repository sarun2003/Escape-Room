using UnityEngine;

public class ObjectProperties : MonoBehaviour
{
    //Higher = Lighter
    //Sorry
    public float m_velocity = 10f;
    public Item m_self;
    public bool m_canBeStored = true;
    public bool m_hovered = false;

    [Header("Gem Properties")]
    public float m_magneticAttraction = 0;


    [SerializeField]
    private ObjectTypeColor m_colorType;
    [SerializeField]
    private float hoverRadius = 5f;



    [SerializeField]
    private bool ignorePlayerCollisions = false;

    private Outline outline;

    void Start()
    {
        m_self = new()
        {
            m_gameObject = gameObject,
            m_type = m_colorType
        };
        m_self.DropObject();
        if (transform.localScale.x > 1f || transform.localScale.y > 1f || transform.localScale.z > 1f)
            m_canBeStored = false;

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
}
