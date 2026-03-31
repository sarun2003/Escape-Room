using UnityEngine;
using System.Collections.Generic;

public enum ObjectState
{
    STATIC,         //Not interacted with
    HELD,           //Being held by player
    STORED          //In player inveentory (not in scene)
};

public class Object
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
    public void StoreObject(int index)
    {
        if (m_state == ObjectState.HELD)
        {
            if (GameManager.PlayerInventory.Size() < GameManager.Instance.m_maxInv)
            {
                if (GameManager.PlayerInventory.GetObjectAtIndex(index) == null)
                {
                    GameManager.PlayerInventory.InsertObjectAt(this, index);
                }
               m_state = ObjectState.STORED; 
            } else
            {
                //Handle full inventory
                //Tell the player some way
            }
            
        }
            
        
    }

    

}

public class Inventory
{
    private Object[] m_items;
    
    //Initialize inventory with nulls
    public Inventory(int size)
    {
        m_items = new Object[size];
        for (int i = 0; i < size; i++)
        {
            m_items[i] = null;
        }
    }

    public Object GetObjectAtIndex(int index)
    {
        if (index < m_items.Length && index >= 0)
            return m_items[index];
        else
            return null; //Handle this in the future
    }
    
    public void InsertObjectAt(Object obj, int index)
    {
        if (obj == null) 
            return; //Handle in the future
        
        if (index < m_items.Length && index >= 0)
            m_items[index] = obj;
    }

    public void RemoveObjectAt(int index)
    {
        if (index < m_items.Length && index >= 0)
            m_items[index] = null;
    }

    public void RemoveObject(Object obj)
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

    public int Size()
    {
        return m_items.Length;
    }

}