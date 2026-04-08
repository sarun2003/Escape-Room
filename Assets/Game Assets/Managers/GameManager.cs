using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum PlayerState
{
    ACTIVE,           
    DEAD,        
    WIN      
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
    public float TimeLeft = 5 * 60;
    //Add in inspector
    [SerializeField]
    public Dictionary<string, GameObject> ProgressionItems = new Dictionary<string, GameObject>();
    public List<GameObject> pickupObjects = new List<GameObject>();

    public PlayerState CurrentPlayerState { get; set; } = PlayerState.ACTIVE;
    public GameObject Player { get; private set; }
    public GameObject PlayerParent { get; private set; }
    
    public PlayerInputState CurrentPlayerInputState { get; set; } = PlayerInputState.NONE; 

    [SerializeField]
    public Material OutlineMaterial;
    public bool PlayerHoveringObject = false;
    public List<GameObject> magneticObjects;


    [SerializeField]
    public int m_maxInv = 3;

    //Debugging States
    [SerializeField]
    private PlayerInputState state1;

    public void UpdateConditions()
    {
        if (Instance.ProgressionItems.ContainsKey("DivingGear") && PlayerInventory.ContainsObject(Instance.ProgressionItems["DivingGear"]))
        {
            //Win
            Instance.CurrentPlayerState = PlayerState.WIN;
        } else
        {
            //Kill Player & reset
            Instance.CurrentPlayerState = PlayerState.DEAD;
        }
    }

    public void UpdateScene()
    {
        Debug.Log(Instance.CurrentPlayerState);
        switch (Instance.CurrentPlayerState)
        {
            case PlayerState.ACTIVE:
            break;

            case PlayerState.DEAD:
            
            Destroy(HUDManager.Instance.gameObject);
            Destroy(Instance.gameObject);
            SceneManager.LoadScene("scene1");
            break;

            case PlayerState.WIN:
            Destroy(HUDManager.Instance.gameObject);
            Destroy(SoundManager.Instance.gameObject);
            Destroy(Instance.gameObject);
            SceneManager.LoadScene("scene1"); //change to win scene here first
            break;

        }
    }




    private GameObject[] FindGameObjectsInLayer(int[] layers) {
        GameObject[] goArray = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        var goList = new System.Collections.Generic.List<GameObject>();
        for (int j = 0; j < layers.Length; j++)
        {
            for (int i = 0; i < goArray.Length; i++) {
                if (goArray[i].layer == layers[j]) goList.Add(goArray[i]);
            }
        }
        
        
        return goList.Count > 0 ? goList.ToArray() : null;
    }

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
            ObjectProperties[] allObjects = FindObjectsByType<ObjectProperties>(FindObjectsSortMode.None);
            GameObject[] progressionObjs = GameObject.FindGameObjectsWithTag("Progression");
            GameObject[] pickupObjs = FindGameObjectsInLayer(new int[] { 6, 7 });

            foreach (var obj in allObjects)
            {
                if (obj.m_attractsGem)
                {
                    magneticObjects.Add(obj.gameObject);
                }
            }

            foreach (var obj in progressionObjs)
            {
                ProgressionItems[obj.name] = obj;
                Debug.Log(ProgressionItems[obj.name].name);
            }

            foreach (var obj in pickupObjs)
            {
                pickupObjects.Add(obj);
                Debug.Log(obj.name);
            }

            Player = GameObject.FindGameObjectWithTag("CinemachineTarget");
            PlayerParent = GameObject.FindGameObjectWithTag("Player");

            SoundManager.Instance.FadeInLoop("ambiance");
        }
    }

    private void Update()
    {
        TimeLeft -= Time.deltaTime;
    }
}
