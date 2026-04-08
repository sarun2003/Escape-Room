using UnityEngine;

public class LockedObjectCustom : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    LockedObjectProperties objP;
    public string mapKey = null;
    public bool deleteKey = true;
    void Start()
    {
        objP = GetComponent<LockedObjectProperties>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (mapKey != null)
        {
            if (GameManager.Instance.ProgressionItems.ContainsKey(mapKey) && collision.gameObject == GameManager.Instance.ProgressionItems[mapKey])
            {
                if (GameManager.Instance.ProgressionItems[mapKey] != null)
                {
                    objP.m_interactable.Unlock();
                    objP.m_interactable.DropObject();
                    if (deleteKey)
                    {
                        GameManager.Instance.Player.GetComponent<PlayerObjectInteraction>().heldObject = null;
                        GameManager.Instance.pickupObjects.Remove(GameManager.Instance.ProgressionItems[mapKey]);
                        Destroy(GameManager.Instance.ProgressionItems[mapKey]);
                        GameManager.Instance.ProgressionItems[mapKey] = null;
                    }
                    
                    
                } else
                {
                    objP.m_interactable.Unlock();
                }
            }
        }
        
            
    }
}
