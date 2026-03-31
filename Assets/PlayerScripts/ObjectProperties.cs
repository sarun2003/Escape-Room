using UnityEngine;

public class ObjectProperties : MonoBehaviour
{
    //Higher = Lighter
    //Sorry
    public float m_mass = 10f;
    public Object m_self;
    void Start()
    {
        m_self = new Object();
        m_self.DropObject();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
