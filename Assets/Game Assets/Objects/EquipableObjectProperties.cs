using Unity.VisualScripting;
using UnityEngine;

public class EquipableObjectProperties : ObjectProperties
{   
    public ObjectEquipable m_equipment;
   
    public override void Start()
    {
        base.Start();
        m_equipment = new();
        m_self = m_equipment; 
        m_self.m_gameObject = gameObject;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        m_canBeStored = true;

         
        
    }

    
    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        
        base.UpdateObjectOutline();
    }
}
