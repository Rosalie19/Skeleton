using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private Animator playerAnim;
    //movement inputs
    private float horizontalInput;
    private float verticalInput;
    private float mouseHorizontal;
    private float mouseVertical;
    private float rotationSpeed = 500.0f;

    public float speed = 5.0f;

    [SerializeField]
    public static bool isAttacking = false;

    private CharacterController playerCt;

    [SerializeField]
    private Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody>();
        playerCt = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {   
        //get arrows input 
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        //get mouse inputs
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y"); //useful when shooting arrows

        //Player rotation follows the mouse 
        Vector3 rotationDirection  = new Vector3(mouseHorizontal, 0, 0);
        //rotationDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up)* rotationDirection;
        //rotationDirection.Normalize();
        //float rotationMagnitude = Mathf.Clamp01(rotationDirection.magnitude) *speed;
        Quaternion rotationQ = Quaternion.Euler(rotationDirection * Time.fixedDeltaTime);

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up)* movementDirection;
        movementDirection.Normalize();
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) *speed;

        //playerRb.MovePosition(transform.position + movementDirection * Time.deltaTime * speed);
        //transform.Translate(movementDirection * speed *Time.deltaTime, Space.World);
        if ( !isAttacking){
            playerRb.MoveRotation(playerRb.rotation * rotationQ);
            //playerCt.SimpleMove(movementDirection * magnitude );
        }
        
        if(movementDirection != Vector3.zero) {

            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        
            playerAnim.SetBool("Run_Bool", true);
        } else {
            playerAnim.SetBool("Run_Bool", false);
        } 

        if (Input.GetKeyDown(KeyCode.Space)){
            isAttacking = true;
            StartCoroutine(Attack());
        }
       
    }

    IEnumerator Attack(){
        playerCt.SimpleMove(new Vector3(0,0,0) * 0);
        playerAnim.SetTrigger("Attack_Trigger");
        yield return new WaitForSeconds(1);
        isAttacking = false ;
    }

    private void OnApplicationFocus( bool focus) {
        if (focus) {
            Cursor.lockState = CursorLockMode.Locked;
        } else {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void OnCollisionEnter(Collision other){
        if (other.gameObject.name == "Enemy"){
            if (playerAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Attack"){
                Debug.Log("ATTACK");
            }
        }
    }
}
