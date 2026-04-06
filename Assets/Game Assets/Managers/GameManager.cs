using UnityEngine;

public enum PlayerState
{
    MENU,           //In a menu, such as a main menu
    PLAY,           //Playing game
    INTERFACE       //In an interface, such as a combination lock
};

public enum PlayerInputState
{
    NONE,           //Not holding any item, not holding use key
    HOLDING,        //Holding an object, cannot pick up more objects, holding use key
    BUFFERING       //Not holding an object, but holding use key. will auto pickup object on next frame where an object is available
};

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } //used for timers and states
    public static Inventory PlayerInventory { get; private set; }


    public PlayerState CurrentPlayerState { get; set; } = PlayerState.PLAY;
    
    public PlayerInputState CurrentPlayerInputState { get; set; } = PlayerInputState.NONE; 

    [SerializeField]
    public Material OutlineMaterial;


    [SerializeField]
    public int m_maxInv = 3;

    //Debugging States
    [SerializeField]
    private PlayerInputState state1;

    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 

            PlayerInventory = new(m_maxInv);
        }
    }

    private void Update()
    {
        state1 = CurrentPlayerInputState;
    }
}
