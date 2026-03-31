using UnityEngine;
using UnityEngine.InputSystem;

public class PickupObjects : MonoBehaviour
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



    RaycastHit hit;


    public InputActionAsset objectActions;
    private InputAction holdAction;
    private void OnEnable()
    {
        holdAction.Enable();
    }

    private void OnDisable()
    {
        holdAction.Disable();
    }


    private void Awake()
    {
        holdAction = objectActions["Holding"];
        hitPointVisualizer = null;
        layerMask = LayerMask.GetMask("Objects", "Player");
        hitPointVisualizer = Instantiate(hitPointVisualizerPrefab);
        
    }

    //Player input states in a different loop to avoid conflicts
    void Update()
    {
        if (holdAction.IsPressed())
        {   
            Debug.Log("holding");
            //Buffer holding. will only change to HOLDING when an object is present and BUFFERING is the current state
            switch (GameManager.Instance.CurrentPlayerInputState)
            {
                case PlayerInputState.NONE:
                GameManager.Instance.CurrentPlayerInputState = PlayerInputState.BUFFERING;
                break;
            }
        } else
        {   
            //Drop Objects
            switch (GameManager.Instance.CurrentPlayerInputState)
            {
                case PlayerInputState.HOLDING:
                GameManager.Instance.CurrentPlayerInputState = PlayerInputState.NONE;
                break;

                case PlayerInputState.BUFFERING:
                GameManager.Instance.CurrentPlayerInputState = PlayerInputState.NONE;
                break;
            }
        }


        if (!hitObject && GameManager.Instance.CurrentPlayerInputState != PlayerInputState.HOLDING)
            heldObject = null;


        if (hitObject)
        {
            
            hitPointVisualizer.SetActive(true);
            hitPointVisualizer.transform.position = hit.point;
            
        } else
        {
            hitPointVisualizer.SetActive(false);
        }
        
        //Need to handle player inputs and swicth the states accordingly
        switch (GameManager.Instance.CurrentPlayerInputState)
        {
            case PlayerInputState.NONE:
            //Do nothing
            break;
            case PlayerInputState.HOLDING:
            //Already holding an object, only update object

            //Debugging
            hitPointVisualizer.SetActive(true);
            hitPointVisualizer.transform.position = ObjectPointOffset + heldObject.transform.position;
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            
            Vector3 targetPosition = transform.position + transform.forward * hitDistance;
            Vector3 trueTarget = targetPosition - ObjectPointOffset;

            Vector3 direction = trueTarget - heldObject.transform.position;
            heldObject.GetComponent<Rigidbody>().AddForce(direction * heldObject.GetComponent<ObjectProperties>().m_mass, ForceMode.Force);
            rb.angularVelocity *= 0.75f;

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

    //Cast a ray from viewport center to a max length and if an object is hit, get its info and give it options
    void FixedUpdate()
    {


        //Does the ray intersect any objects excluding the player layer
        hitObject = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, viewDist, layerMask);

        //Draw ray for debugging
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * viewDist, hitObject ? Color.yellow : Color.red);

        

    }
}






/*
cube center (world space)
hit point (world space)

calculate an offset relative from cube center to hit point (local)
move object center to target point - offset every frame
rotate around offset point (local space)






*/