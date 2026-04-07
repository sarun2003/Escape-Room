using Unity.VisualScripting;
using UnityEngine;

public class LockedObjectProperties : ObjectProperties
{   
    public ObjectInteractable m_interactable;
    [SerializeField]
    private bool m_startLocked = true;
   
    public override void Start()
    {
        base.Start();
        m_interactable = new();
        m_self = m_interactable; 

        if (m_startLocked) m_interactable.Lock();
        else m_interactable.Unlock();

        m_canBeStored = false;

         
        
    }

    
    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        
        if (m_interactable.m_lockedState != LockedObjectState.LOCKED)
        {
            base.UpdateObjectAttraction();
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        } else
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
        base.UpdateObjectOutline();
    }
}
