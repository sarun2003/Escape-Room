using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerObjectInteraction : MonoBehaviour
{
    LayerMask layerMask;
    GameObject heldObject;



    [SerializeField]
    GameObject hitPointVisualizerPrefab;
    GameObject hitPointVisualizer;



    [SerializeField]
    byte viewDist = 2;
    bool hitObject = false;
    float hitDistance = 0;
    Vector3 ObjectPointOffset;
    Vector3 ObjectDropPoint;



    RaycastHit hit;


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
        holdAction = objectActions["Holding"];
        //Only 3 for now
        QS1 = objectActions["Quickslot1"];
        QS2 = objectActions["Quickslot2"];
        QS3 = objectActions["Quickslot3"];

        hitPointVisualizer = null;
        
        layerMask = LayerMask.GetMask("Objects", "Player");
        hitPointVisualizer = Instantiate(hitPointVisualizerPrefab);
        
    }

    void TryDropObject(int index)
    {
        if (!GameManager.PlayerInventory.IndexInRange(index)) return; //Out of range index
        if (GameManager.PlayerInventory.EmptySlots() <= 0) return; //Empty inventory
        if (GameManager.PlayerInventory.GetObjectAtIndex(index) == null) return; //No object at index
        
        Item obj = GameManager.PlayerInventory.PopObjectAtIndex(index);
        obj.DropObject();
        obj.m_gameObject.SetActive(true);
        obj.m_gameObject.transform.position = ObjectDropPoint;
        
        
    }

    //Tries to store an object at index, otherwise picks next free slot or doesnt store object at all
    void TryStoreAtIndex(Item obj, int index)
    {
        if (GameManager.PlayerInventory.GetObjectAtIndex(index) == null)
        {
            //Object stored successfully, delete from scene
            if (obj.StoreObject(index) != -1)
            {
                obj.m_gameObject.SetActive(false);
                return; 
            }
                
        } else
        {   
            int status = obj.TryReassignObject();
            if (status != -1)
            {
                //Object restored successfully, delete from scene
                obj.m_gameObject.SetActive(false);
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
                    TryStoreAtIndex(heldObject.GetComponent<ObjectProperties>().m_self, 0);
                    
                } else if (QS2.WasPressedThisFrame())
                {
                    //Attempt to assign to Inv slot 2
                    TryStoreAtIndex(heldObject.GetComponent<ObjectProperties>().m_self, 1);

                } else if (QS3.WasPressedThisFrame())
                {
                    //Attempt to assign to Inv slot 3
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


        //Does the ray intersect any objects excluding the player layer
        hitObject = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, viewDist, layerMask);

        //Draw ray for debugging
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * viewDist, hitObject ? Color.yellow : Color.red);

        if (!hitObject && GameManager.Instance.CurrentPlayerInputState != PlayerInputState.HOLDING)
            heldObject = null;


        //Mainly for debugging
        if (hitObject)
        {
            hitPointVisualizer.SetActive(true);
            hitPointVisualizer.transform.position = hit.point;
            
        } else
        {
            hitPointVisualizer.SetActive(false);
        }


        switch (GameManager.Instance.CurrentPlayerInputState)
        {
            case PlayerInputState.NONE:
            //Do nothing
            break;

            case PlayerInputState.HOLDING:
            //Already holding an object, only update held object

            //Debugging
            hitPointVisualizer.SetActive(true);
            hitPointVisualizer.transform.position = ObjectPointOffset + heldObject.transform.position;
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            
            Vector3 targetPosition = transform.position + transform.forward * hitDistance;
            Vector3 trueTarget = targetPosition - ObjectPointOffset;

            Vector3 direction = trueTarget - heldObject.transform.position;
            heldObject.GetComponent<Rigidbody>().AddForce(direction * heldObject.GetComponent<ObjectProperties>().m_velocity, ForceMode.Force);
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