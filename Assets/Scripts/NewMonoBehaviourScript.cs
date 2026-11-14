using UnityEngine;

public class Movebox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    
    public float speed = 1f;

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }

}
