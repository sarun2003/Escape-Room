using UnityEngine;

public class FixReactor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Wrench")
        {
            GameManager.Instance.ProgressionItems["TubeKey"].transform.position = GameManager.Instance.Player.transform.position;
            GameManager.Instance.ProgressionItems["TubeKey"].GetComponent<Rigidbody>().position = GameManager.Instance.Player.transform.position;
            GameManager.Instance.Player.GetComponent<PlayerObjectInteraction>().heldObject = null;
            GameManager.Instance.pickupObjects.Remove(GameManager.Instance.ProgressionItems["Wrench"]);
            Destroy(GameManager.Instance.ProgressionItems["Wrench"]);
            GameManager.Instance.ProgressionItems["Wrench"] = null;
        }
    }
}
