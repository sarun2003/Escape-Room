using UnityEngine;
using UnityEngine.SceneManagement;

public class Ocean : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance.ProgressionItems.ContainsKey("DivingGear") && GameManager.PlayerInventory.ContainsObject(GameManager.Instance.ProgressionItems["DivingGear"]))
            {
                //Win
                SceneManager.LoadScene("scene1");
            } else
            {
                //Kill Player & reset
                Destroy(HUDManager.Instance.gameObject);
                Destroy(GameManager.Instance.gameObject);
                SceneManager.LoadScene("scene1");

            }
        }
    }
}
