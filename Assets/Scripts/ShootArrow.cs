using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootArrow : MonoBehaviour
{
    public float velocity = 600f;
    public GameObject arrow;
    public GameObject sphere;
    public GameObject player;
    public bool shift;
    private Vector3 direction;
    private Quaternion lookRotation;
    public float rotationSpeed = 50f;

    // Start is called before the first frame update
    void Start()
    {
        shift = PlayerBowController.shiftBool;
    }

    // Update is called once per frame
    void Update()
    {


        shift = PlayerBowController.shiftBool;
        if (shift){
            
            if (Input.GetMouseButtonDown(0)){

                direction = (sphere.transform.position - player.transform.position);
                direction.y -= 1.5f;
                Debug.Log(direction);
                Invoke("Shoot", 0.5f);
                
            }
        }
        
    }

    void Shoot() {
        GameObject launchedObject = Instantiate(arrow, transform.position, transform.rotation);
        launchedObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(direction.y*100,velocity, 0));
    }
}
