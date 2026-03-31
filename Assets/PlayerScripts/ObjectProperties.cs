using UnityEngine;

public class ObjectProperties : MonoBehaviour
{
    //Higher = Lighter
    //Sorry
    public float m_velocity = 10f;
    public Item m_self;
    void Start()
    {
        m_self = new Item();
        m_self.m_gameObject = gameObject;
        m_self.DropObject();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
