using System.Collections.Generic;
using System.Collections;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class SFXObject
{
    public string Name;             
    public List<AudioClip> Clips;   
    public float Volume = 1f;

    public SFXObject(string name, List<AudioClip> clips)
    {
        Name = name;
        Clips = clips;
    }
}

//Only for singletons
public class SFXIntervals
{
    public float walk = 0;
}

public enum SFXType
{
    Walking,
    Metal,
    Glass,
    Ambient,
    MetalDoor

}





public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }


    public List<SFXObject> SoundObjects = new List<SFXObject>();
    private Dictionary<string, AudioSource> loopSources = new();
    public SFXIntervals intervals = new();
    public bool[] SoundWarnings = {};

    private string[] sfxFolders = { "Walking", "Metal", "Glass", "Ambient", "Metal Door" }; 

    public string SFXtoString(SFXType type)
    {
        return Instance.sfxFolders[(int)type];
    }


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
   
        LoadAllAudio();
        LoadAllLoops();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void LoadAllAudio()
    {
        

        foreach (var folder in sfxFolders)
        {
            AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio/" + folder);
            if (clips.Length > 0)
            {
                SoundObjects.Add(new SFXObject(folder, new List<AudioClip>(clips)));
            }
        }

       
    }

    private void LoadAllLoops()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio/Loops");

        foreach (var clip in clips)
        {
            if (clip == null) continue;

            // Create a dedicated AudioSource for this clip
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;
            source.playOnAwake = false;
            source.volume = 1f;

            // Store in dictionary by clip name
            loopSources[clip.name] = source;

            // Start playing automatically
            source.Play();
        }
    }

    public void PlayLoop(string name)
    {
        if (loopSources.TryGetValue(name, out var source))
        {
            if (!source.isPlaying)
                source.Play();
        }
    }

    public void StopLoop(string name)
    {
        if (loopSources.TryGetValue(name, out var source))
        {
            source.Stop();
        }
    }


    public void FadeInLoop(string name, float duration = 1f, float targetVolume = 1f)
    {
        if (loopSources.TryGetValue(name, out var source))
        {
            StopCoroutine(nameof(FadeAudio)); 
            StartCoroutine(FadeAudio(source, source.volume, targetVolume, duration, false));
        }
    }

    public void FadeOutLoop(string name, float duration = 1f)
    {
        if (loopSources.TryGetValue(name, out var source))
        {
            StopCoroutine(nameof(FadeAudio)); 
            StartCoroutine(FadeAudio(source, source.volume, 0f, duration, true));
        }
    }

    // Stop all loops
    public void StopAllLoops()
    {
        foreach (var source in loopSources.Values)
            source.Stop();
    }

    
    public void PlaySFX(string sfxName, Vector3 position, float volume = 1f)
    {
        var sfx = SoundObjects.Find(x => x.Name == sfxName);
        if (sfx != null && sfx.Clips.Count > 0)
        {
            AudioClip clip = sfx.Clips[Random.Range(0, sfx.Clips.Count)];

            float finalVolume = sfx.Volume * volume; // 👈 combine default + override

            AudioSource.PlayClipAtPoint(clip, position, finalVolume);

            Debug.Log($"Playing {clip.name} at volume {finalVolume}");
        }
        else
        {
            Debug.LogWarning($"SFX '{sfxName}' not found or empty!");
        }
    }

    void Update()
    {
        
        if (Instance.gameObject != null)
        {
            if (Instance.intervals.walk <= 0)
            {
                if (GameManager.Instance.PlayerParent.GetComponent<StarterAssetsInputs>().move != Vector2.zero && GameManager.Instance.PlayerParent.GetComponent<FirstPersonController>().Grounded)
                {
                    if (GameManager.Instance.PlayerParent.GetComponent<StarterAssetsInputs>().sprint)
                    {
                        Instance.intervals.walk = 0.3f;
                    } else
                    {
                        Instance.intervals.walk = 0.45f;
                    }
                    
                    Instance.PlaySFX("Walking", GameManager.Instance.Player.transform.position, 0.5f);
                } else
                {
                    Instance.intervals.walk = 0;
                }
            } else
            {
                Instance.intervals.walk -= Time.deltaTime;
            }  
        }
        
        
    }



    

    IEnumerator FadeAudio(AudioSource source, float startVol, float endVol, float duration, bool stopAfter)
    {
        float time = 0f;

        // If fading in, make sure it's playing
        if (!source.isPlaying)
            source.Play();

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // Smooth fade (better than Lerp)
            source.volume = Mathf.SmoothStep(startVol, endVol, t);

            yield return null;
        }

        source.volume = endVol;

        if (stopAfter)
            source.Stop();
    }

    




}