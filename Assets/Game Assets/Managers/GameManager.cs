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
    NONE,
    HOLDING,
    BUFFERING
};

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static Inventory PlayerInventory { get; private set; }

    public float TimeLeft = 5 * 60;
    private const float MaxTime = 10f * 60f;
    private HashSet<string> _solvedPuzzles = new HashSet<string>();

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

    [SerializeField]
    private PlayerInputState state1;

    public void UpdateConditions()
    {
        if (Instance.ProgressionItems.ContainsKey("DivingGear") &&
            PlayerInventory.ContainsObject(Instance.ProgressionItems["DivingGear"]))
        {
            Instance.CurrentPlayerState = PlayerState.WIN;
        }
        else
        {
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
                Destroy(HUDManager.Instance);
            
            
                Destroy(SoundManager.Instance.gameObject);
                Destroy(SoundManager.Instance);


                SceneManager.LoadScene("MainMenu");
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Destroy(Instance.gameObject);
                Destroy(Instance);
                break;
            case PlayerState.WIN:
                
                Destroy(HUDManager.Instance.gameObject);
                Destroy(HUDManager.Instance);
            
            
                Destroy(SoundManager.Instance.gameObject);
                Destroy(SoundManager.Instance);
            
                    
                
                SceneManager.LoadScene("MainMenu");
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Destroy(Instance.gameObject);
                Destroy(Instance);
                break;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private GameObject[] FindGameObjectsInLayer(int[] layers)
    {
        GameObject[] goArray = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        var goList = new List<GameObject>();
        for (int j = 0; j < layers.Length; j++)
        {
            for (int i = 0; i < goArray.Length; i++)
            {
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
                    magneticObjects.Add(obj.gameObject);
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

            // Safely start ambiance
            if (SoundManager.Instance != null)
                SoundManager.Instance.FadeInLoop("ambiance");
        }
    }

    private void Update()
    {
        if (CurrentPlayerState != PlayerState.ACTIVE) return;

        TimeLeft -= Time.deltaTime;

        if (TimeLeft <= 0f)
        {
            TimeLeft = 0f;
            TriggerInstantDeath("Timer ran out — the facility floods.");
        }
    }

    public void OnPuzzleCompleted(string puzzleId)
    {
        if (_solvedPuzzles.Contains(puzzleId)) return;
        _solvedPuzzles.Add(puzzleId);
        Debug.Log($"[GameManager] Puzzle completed: {puzzleId} ({_solvedPuzzles.Count} solved total)");
    }

    public void ExtendTimer(float extraSeconds)
    {
        TimeLeft = Mathf.Min(TimeLeft + extraSeconds, MaxTime);
        Debug.Log($"[GameManager] Timer extended by {extraSeconds}s → {TimeLeft:F0}s remaining.");
    }

    public void TriggerInstantDeath(string reason)
    {
        if (CurrentPlayerState != PlayerState.ACTIVE) return;
        Debug.Log($"[GameManager] INSTANT DEATH: {reason}");
        CurrentPlayerState = PlayerState.DEAD;
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX("explosion", Vector3.zero, 1f);
        UpdateScene();
    }
}