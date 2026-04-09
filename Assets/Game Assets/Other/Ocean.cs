using UnityEngine;
using UnityEngine.SceneManagement;

public class Ocean : MonoBehaviour
{
    

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SoundManager.Instance.FadeOutLoop("ambiance");
            GameManager.Instance.UpdateConditions();
        }
    }
}
