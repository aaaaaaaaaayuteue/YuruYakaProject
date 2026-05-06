using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    Rigidbody rb;
    
    [SerializeField] float movespeed = 5;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 簡単な移動処理
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            transform.Translate(moveDirection * movespeed * Time.deltaTime);
        }
        
        
        
    }
}
