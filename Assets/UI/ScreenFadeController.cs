using UnityEngine;
using UnityEngine.UI;

public class ScreenFadeController : MonoBehaviour
{
    

    private Animator animatorComp;
    private Image imageComp;

    void Awake()
    {
        
        animatorComp = GetComponent<Animator>();
        imageComp = GetComponent<Image>(); 
        imageComp.enabled = true;
        animatorComp.SetBool("PlayerAlive", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (animatorComp.GetBool("PlayerAlive"))
        {
            if (GameManager.Instance.CurrentPlayerState == PlayerState.DEAD)
            {
                animatorComp.SetBool("PlayerDead", true);
            }
            if (GameManager.Instance.CurrentPlayerState == PlayerState.WIN)
            {
                animatorComp.SetBool("PlayerWin", true);
            }
        }
        
    }

    public void Disable()
    {
        imageComp.enabled = false;
    }

    public void Enable()
    {
        imageComp.enabled = true;
    }

    public void DisableAlive()
    {
        animatorComp.SetBool("PlayerAlive", false);
    }

    public void UpdateState()
    {
        
        Debug.Log(GameManager.Instance.CurrentPlayerState);
        GameManager.Instance.UpdateScene();
    }
}
