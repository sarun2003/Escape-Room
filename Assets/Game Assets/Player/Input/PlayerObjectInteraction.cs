using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerObjectInteraction : MonoBehaviour
{
    LayerMask layerMask;
    GameObject heldObject;
    GameObject hoveredObject;
    private bool hoveringObject = false;



    [SerializeField]
    GameObject hitPointVisualizerPrefab;
    GameObject hitPointVisualizer;



    [SerializeField]
    byte viewDist = 2;
    bool hitObject = false;
    bool hitAny = false;
    float hitDistance = 0;
    Vector3 ObjectPointOffset;
    Vector3 ObjectDropPoint;



    RaycastHit hit;
    RaycastHit dpHit;


    public InputActionAsset objectActions;
    private InputAction holdAction;
    private InputAction QS1;
    private InputAction QS2;
    private InputAction QS3;

    private void OnEnable()
    {
        holdAction.Enable();
        QS1.Enable();
        QS2.Enable();
        QS3.Enable();
    }

    private void OnDisable()
    {
        holdAction.Disable();
        QS1.Disable();
        QS2.Disable();
        QS3.Disable();
    }


    private void Awake()
    {   

        

        Physics.IgnoreLayerCollision(3, 7); //Ignore collisions between player and ObjectsIC (Ignore collision)
        holdAction = objectActions["Holding"];
        //Only 3 for now
        QS1 = objectActions["Quickslot1"];
        QS2 = objectActions["Quickslot2"];
        QS3 = objectActions["Quickslot3"];

        hitPointVisualizer = null;
        
        layerMask = LayerMask.GetMask("Objects", "ObjectsIC", "Player");
        hitPointVisualizer = Instantiate(hitPointVisualizerPrefab);

        hitPointVisualizer.SetActive(false);
        
    }

    void TryDropObject(int index)
    {
        

        if (!GameManager.PlayerInventory.IndexInRange(index)) return; //Out of range index
        if (GameManager.PlayerInventory.EmptySlots() <= 0) return; //Empty inventory
        if (GameManager.PlayerInventory.GetObjectAtIndex(index) == null) return; //No object at index
        
        Item obj = GameManager.PlayerInventory.PopObjectAtIndex(index);

        //If ray collided with anything, set that point - half of the stored objects width  to the new drop point, otherwise use the other formula
        hitAny = Physics.Raycast(transform.position, transform.forward, out dpHit, viewDist, ~0);

        if (hitAny)
        {
            ObjectDropPoint = dpHit.point + dpHit.normal * 0.1f;
        } else
        {
            ObjectDropPoint = transform.position + (transform.TransformDirection(Vector3.forward) * viewDist);
        }

        obj.DropObject();
        //obj.m_gameObject.SetActive(true);
        obj.m_gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        obj.m_gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        obj.m_gameObject.transform.position = ObjectDropPoint;

        if (heldObject == obj.m_gameObject)
            heldObject = null;

        obj.DropObject();
        
        
    }

    //Tries to store an object at index, otherwise picks next free slot or doesnt store object at all
    void TryStoreAtIndex(Item obj, int index)
    {
        if (GameManager.PlayerInventory.GetObjectAtIndex(index) == null)
        {
            //Object stored successfully, delete from scene
            if (obj.StoreObject(index) != -1)
            {
                //obj.m_gameObject.SetActive(false);
                Vector3 newPos = HUDManager.Instance.InventorySlots[index].GetComponent<InventorySlot>().camera.transform.parent.gameObject.transform.position;
                obj.m_gameObject.transform.position = newPos + new Vector3(2, 2, -2);
                obj.m_gameObject.transform.LookAt(HUDManager.Instance.InventorySlots[index].GetComponent<InventorySlot>().camera.transform.position);
                obj.m_gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                GameManager.Instance.CurrentPlayerInputState = PlayerInputState.NONE;
                return; 
            }
                
        } else
        {   
            int status = obj.TryReassignObject();
            if (status != -1)
            {
                //Object restored successfully, delete from scene
                //obj.m_gameObject.SetActive(false);
                Vector3 newPos = HUDManager.Instance.InventorySlots[status].GetComponent<InventorySlot>().camera.transform.parent.gameObject.transform.position;
                obj.m_gameObject.transform.position = newPos + new Vector3(2, 2, 2);
                obj.m_gameObject.GetComponent<ObjectProperties>().camPos = HUDManager.Instance.InventorySlots[index].GetComponent<InventorySlot>().camera.transform.position;
                obj.m_gameObject.transform.LookAt(obj.m_gameObject.GetComponent<ObjectProperties>().camPos);
                obj.m_gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                GameManager.Instance.CurrentPlayerInputState = PlayerInputState.NONE;
                return;
            } else
            {
                Debug.Log("Inventory Full");
            }
        }
        //Error occurs: most likely full inventory: do nothing
        //Warn player somehow
        
        
    }

    //Player input states in a different loop to avoid conflicts
    void Update()
    {   
        
        
        ObjectDropPoint = transform.position + (transform.TransformDirection(Vector3.forward) * viewDist);

        if (holdAction.IsPressed())
        {   
            //Buffer holding. will only change to HOLDING when an object is present and BUFFERING is the current state
            switch (GameManager.Instance.CurrentPlayerInputState)
            {
                case PlayerInputState.NONE:
                GameManager.Instance.CurrentPlayerInputState = PlayerInputState.BUFFERING;
                break;

                case PlayerInputState.HOLDING: //Check for quickslot inputs, Can add more later
                if (QS1.WasPressedThisFrame())
                {   
                    //Attempt to assign to Inv slot 1
                    if (heldObject.GetComponent<ObjectProperties>().m_canBeStored)
                        TryStoreAtIndex(heldObject.GetComponent<ObjectProperties>().m_self, 0);
                    
                } else if (QS2.WasPressedThisFrame())
                {
                    //Attempt to assign to Inv slot 2
                    if (heldObject.GetComponent<ObjectProperties>().m_canBeStored)
                        TryStoreAtIndex(heldObject.GetComponent<ObjectProperties>().m_self, 1);

                } else if (QS3.WasPressedThisFrame())
                {
                    //Attempt to assign to Inv slot 3
                    if (heldObject.GetComponent<ObjectProperties>().m_canBeStored)
                        TryStoreAtIndex(heldObject.GetComponent<ObjectProperties>().m_self, 2);

                }
                break;
            }
        } else
        {   
            //Drop Objects from holding
            switch (GameManager.Instance.CurrentPlayerInputState)
            {
                case PlayerInputState.HOLDING:
                GameManager.Instance.CurrentPlayerInputState = PlayerInputState.NONE;
                heldObject.GetComponent<ObjectProperties>().m_self.DropObject();
                heldObject = null;
                break;

                case PlayerInputState.BUFFERING:
                GameManager.Instance.CurrentPlayerInputState = PlayerInputState.NONE;
                break;
            }
            
            //Drop objects from inv
            if (QS1.WasPressedThisFrame())
            {
                TryDropObject(0);
            } else if (QS2.WasPressedThisFrame())
            {
                TryDropObject(1);
            } else if (QS3.WasPressedThisFrame())
            {
                TryDropObject(2);
            }
        }

        
    }

    //Cast a ray from viewport center to a max length and if an object is hit, get its info and give it options
    void FixedUpdate()
    {
        GameManager.Instance.PlayerHoveringObject = hoveringObject;

        //Does the ray intersect any objects excluding the player layer
        hitObject = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, viewDist, layerMask);

        //Draw ray for debugging
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * viewDist, hitObject ? Color.yellow : Color.red);

        if (!hitObject && GameManager.Instance.CurrentPlayerInputState != PlayerInputState.HOLDING)
            heldObject = null;


        if (hoveredObject != null)
        {
            if (hoveredObject.TryGetComponent(out ObjectProperties op))
                op.m_hovered = hitObject;
        }
        
        

        //Mainly for debugging
        /*
        if (hitObject)
        {

            hitPointVisualizer.SetActive(true);
            hitPointVisualizer.transform.position = hit.point;
            if (hit.collider != null)
                hoveredObject = hit.collider.gameObject;
                
        } else
        {  
            hitPointVisualizer.SetActive(false);  
            hoveredObject = null;
        }
        */
        if (hitObject)
        {
            if (hit.collider != null)  {
                hoveredObject = hit.collider.gameObject;
                hoveringObject = true;
                
            }
        } else
        {
            hoveredObject = null;
            hoveringObject = false;
        }
        

        
    

        switch (GameManager.Instance.CurrentPlayerInputState)
        {
            case PlayerInputState.NONE:
            //Do nothing
            break;

            case PlayerInputState.HOLDING:
            //Already holding an object, only update held object

            //Debugging
            //hitPointVisualizer.SetActive(true);
            //hitPointVisualizer.transform.position = ObjectPointOffset + heldObject.transform.position;
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            
            Vector3 targetPosition = transform.position + transform.forward * hitDistance;
            Vector3 trueTarget = targetPosition - ObjectPointOffset;

            Vector3 direction = trueTarget - heldObject.transform.position;
            rb.AddForce(direction * 10, ForceMode.Force);
            
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // prevents tunneling
            rb.interpolation = RigidbodyInterpolation.Interpolate; // smooth movement
            rb.maxDepenetrationVelocity = 10f; // optional: prevents high-speed clipping
            rb.angularVelocity *= 0.95f;
            rb.linearVelocity *= 0.95f;

            break;
            case PlayerInputState.BUFFERING:
            //Attempt to pick up object
            if (hitObject)
            {
                if (hit.collider.gameObject != null)
                {
                    heldObject = hit.collider.gameObject;
                    GameManager.Instance.CurrentPlayerInputState = PlayerInputState.HOLDING;
                    heldObject.GetComponent<ObjectProperties>().m_self.HoldObject();
                    
                    hitDistance = hit.distance;

                    
                    ObjectPointOffset = hit.point - heldObject.transform.position;
                } else
                {
                    Debug.LogWarning("object hit with raycast was null, and player attempted to pick it up");
                } 
            }
            
            
            break;

        }

    }
}






/*
cube center (world space)
hit point (world space)

calculate an offset relative from cube center to hit point (local)
move object center to target point - offset every frame
rotate around offset point (local space) <--- next up






*/