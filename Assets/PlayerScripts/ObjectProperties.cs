using UnityEngine;

public class ObjectProperties : MonoBehaviour
{
    //Higher = Lighter
    //Sorry
    public float m_velocity = 10f;
    public Item m_self;
    public bool m_canBeStored = true;
    public bool m_hovered = false;

    void Start()
    {
        m_self = new()
        {
            m_gameObject = gameObject
        };
        m_self.DropObject();
        if (transform.localScale.x > 0.5f || transform.localScale.y > 0.5f || transform.localScale.z > 0.5f)
            m_canBeStored = false;

        Material[] mats = {GetComponent<Renderer>().materials[0], GetComponent<Renderer>().materials[0]};
        GetComponent<Renderer>().materials = mats;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_hovered || m_self.m_state == ObjectState.HELD)
        {
            Material[] matsList = GetComponent<Renderer>().materials;
            matsList[1] = GameManager.Instance.OutlineMaterial;
            GetComponent<Renderer>().materials = matsList;
        } else
        {
            Material[] matsList = GetComponent<Renderer>().materials;
            matsList[1] = GetComponent<Renderer>().materials[0];
            GetComponent<Renderer>().materials = matsList;
        }
    }
}
