using UnityEngine;
using System.Collections.Generic;

public enum ObjectState
{
    STATIC,         //Not interacted with
    HELD,           //Being held by player
    STORED          //In player inveentory (not in scene)
};

public class Item
{
    private ObjectState m_state = ObjectState.STATIC;
    public GameObject m_gameObject;

    //Hold object from world or inventory
    public void HoldObject()
    {
        switch (m_state)
        {
            case ObjectState.STATIC:
            m_state = ObjectState.HELD;
            break;

            case ObjectState.STORED:
            m_state = ObjectState.HELD;
            break;

        }
    }

    //Drop object from held or inventory where you stand
    public void DropObject()
    {
        m_state = ObjectState.STATIC;
    }

    //Place an object into inventory
    //Attempt to store at index, but move to next best slot if full
    public int StoreObject(int index)
    {
        if (m_state == ObjectState.HELD)
        {
            if (GameManager.PlayerInventory.EmptySlots() < GameManager.Instance.m_maxInv)
            {
                if (GameManager.PlayerInventory.GetObjectAtIndex(index) == null)
                {
                    GameManager.PlayerInventory.InsertObjectAt(this, index);
                    m_state = ObjectState.STORED; 
                    return index;
                }
                
            } else
            {
                return TryReassignObject();
            }
            
        }
        return -1;
            
        
    }

    //Returns the new index of inserted object, otherwise returns -1 if full or fail
    public int TryReassignObject()
    {
        for (int i = 0; i < GameManager.PlayerInventory.ArraySize(); i++)
        {
            if (GameManager.PlayerInventory.GetObjectAtIndex(i) == null)
            {
                GameManager.PlayerInventory.InsertObjectAt(this, i);
                m_state = ObjectState.STORED; 
                return i;
            }
                
        }
        return -1;
    }

    

}

public class Inventory
{
    [SerializeField]
    private Item[] m_items;
    
    //Initialize inventory with nulls
    public Inventory(int size)
    {
        m_items = new Item[size];
        for (int i = 0; i < size; i++)
        {
            m_items[i] = null;
        }
    }

    //Does not update array
    public Item GetObjectAtIndex(int index)
    {
        if (IndexInRange(index))
            return m_items[index];
        else
            return null; //Handle this in the future
    }

    //Updates array accordingly
    public Item PopObjectAtIndex(int index)
    {
        if (IndexInRange(index))
        {
            Item i = m_items[index];
            m_items[index] = null;
            return i;
        }   
        else
        {
            return null;
        }
            
    }
    
    public void InsertObjectAt(Item obj, int index)
    {
        if (obj == null) 
            return; //Handle in the future
        
        if (IndexInRange(index))
            m_items[index] = obj;
    }

    public void RemoveObjectAt(int index)
    {
        if (IndexInRange(index))
            m_items[index] = null;
    }

    public void RemoveObject(Item obj)
    {
        
        for (int i = 0; i < m_items.Length; i++)
        {
            if (m_items[i] == obj)
            {
                m_items[i] = null;
                return;
            }
        }
        
    }

    public int EmptySlots()
    {
        int count = 0;
        for (int i = 0; i < m_items.Length; i++)
        {
            if (m_items[i] != null)
                count++;
        }
        return count;
    }

    public int ArraySize()
    {
        return m_items.Length;
    }

    public bool IndexInRange(int i)
    {
        return i >= 0 && i < ArraySize();
    }
}