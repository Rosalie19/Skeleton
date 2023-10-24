using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootArrow : MonoBehaviour
{
    public float velocity = 600f;
    public GameObject arrow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)){
            Invoke("Shoot", 0.5f);
            
        }
    }

    void Shoot() {
        GameObject launchedObject = Instantiate(arrow, transform.position, transform.rotation);
        launchedObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,velocity, 0));
    }
}
