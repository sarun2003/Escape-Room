using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 


public class CheatCode
{
    public bool completedCode = false;
    public int codeStep = 0;
    public List<Key> inputs;

    public Action Cheat;

    public CheatCode(List<Key> inputs, Action Cheat)
    {
        this.inputs = inputs;
        this.Cheat = Cheat;
    }


    public void UpdateStep(Key key)
    {
        if (!completedCode)
        {
            if (key == inputs[codeStep])
            {
                codeStep++;
            } else
            {
                Reset();
            }
            if (codeStep >= inputs.Count)
            {
                completedCode = true;
                Cheat?.Invoke();
                Reset();
            }
        }
        
    }



    public void Reset()
    {
        codeStep = 0;
        completedCode = false;
    }



}

public class CheatCodeManager : MonoBehaviour
{
    public static CheatCodeManager Instance { get; private set; }

    public List<CheatCode> CheatCodes = new List<CheatCode>();
    private List<Key> KeysPressedThisFrame = new List<Key>();

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

            //Add Codes here
            CheatCodes.Add(new CheatCode(
                new List<Key> { Key.A, Key.B, Key.C },
                TPKey
            ));
        }
    }

    
    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
   
    void UpdateKD()
    {
        KeysPressedThisFrame.Clear();

        foreach (Key key in Enum.GetValues(typeof(Key)))
        {
            if (key == Key.None) continue;
            if (Keyboard.current[key].wasPressedThisFrame)
            {
                KeysPressedThisFrame.Add(key);
            }
        }
    }

    void Update()
    {
        UpdateKD();

        foreach (var cc in CheatCodes)
        {
            foreach (Key key in KeysPressedThisFrame)
            {
                cc.UpdateStep(key);
            }
        }
    }







    //Cheats
    void TPKey()
    {
        if (GameManager.Instance.ProgressionItems["Key"] != null)
        {
            GameManager.Instance.ProgressionItems["Key"].transform.position = GameManager.Instance.Player.transform.position;
            if (GameManager.Instance.ProgressionItems["Key"].TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.position = GameManager.Instance.Player.transform.position;
            }
        }
        
    }
}