using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBowController : MonoBehaviour
{
    //player rigidbody
    private Rigidbody playerRb;
    //player controller
    private CharacterController playerCt;
    //Animators 
    private Animator playerAnim; //animation of the player 
    public Animator bowAnim; //animation of the bow 
    //Useful GameObjects
    public GameObject shiftCameraFocus; //used to reset the position; 
    public GameObject sphereFocus; //focus point of the camera in shoot mode
    public GameObject target; //target displayed when in shoot mode (UI)
    //Cameras
    public GameObject mainCamera; 
    public GameObject shiftCamera;
    //Player movement inputs
    private float horizontalInput;
    private float verticalInput;
    //mouse inputs
    private float mouseHorizontal;
    private float mouseVertical;
    //speeds
    private float rotationSpeed = 500.0f; //speed rotation when shift is'nt pressed
    private float rotationSpeedShift = 200.0f; //rotation speed when shift is pressed
    public float speed = 5.0f; //player speed
    private float headTiltSpeed = 50.0f;// rotation speed of the vertical tilt in shoot mode
    //Flags
    public static bool shiftBool; // true : shoot mode
    public static bool bowAttack = false; // true : attack
    //bounds of the vertical tilt in shoot mode
    private float topBoundTilt = 4f;
    private float bottomBoundTilt = -0.5f;
 
    
    [SerializeField]
    private Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        //get the components 
        playerAnim = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody>();
        playerCt = GetComponent<CharacterController>();

        //initialize the cameras (default is main camera)
        mainCamera.SetActive(true);
        shiftCamera.SetActive(false);
    }

    
    void Update()
    {   
        //get inputs
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        //rotation of the player in shoot mode
        Vector3 mouseDirection = new Vector3(mouseHorizontal, 0, 0);
        //vertical tilt in shoot mode
        Vector3 headDirection = new Vector3(0, 0, mouseVertical);
        //direction of the player 
        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);

        //Set the vectors
        if(shiftBool) {
            target.SetActive(true); //set the target visible when in shoot mode
            headDirection = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up)* headDirection;
            mouseDirection = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up)* mouseDirection;
            movementDirection = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up)* movementDirection;
        } else {
            target.SetActive(false); //reset the visibility of the target
            movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up)* movementDirection;
        }

        movementDirection.Normalize();
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) *speed;
        
        //Move the player
        if (!bowAttack){ // the player cannot move when shooting
            playerCt.Move(movementDirection * Time.deltaTime * magnitude ); //player movement 
            //if not in shoot mode, the player is facing where it goes
            if (!shiftBool &&((horizontalInput !=0 ) || (verticalInput != 0))){ 
                Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            //if in shoot mode, the player rotates with the mouse an the camera tilts with the mouse 
            } else if (shiftBool){
                //handle the camera vertical tilt 
                if (mouseVertical != 0) {
                    //keep the camera focus poitn inbounds
                    if (sphereFocus.transform.position.y <= topBoundTilt) {
                        if (sphereFocus.transform.position.y >= bottomBoundTilt){
                            sphereFocus.transform.Translate(Vector3.up * Time.deltaTime * mouseVertical * headTiltSpeed); // move the focus point vertically 
                        } else {
                            sphereFocus.transform.position  = new Vector3 ( sphereFocus.transform.position.x, bottomBoundTilt, sphereFocus.transform.position.z);
                        }
                        
                    } else {
                        sphereFocus.transform.position  = new Vector3 ( sphereFocus.transform.position.x, topBoundTilt, sphereFocus.transform.position.z);
                    }
                }
              
                //handle the player rotation
                if (mouseHorizontal != 0){
                    Quaternion toRotation = Quaternion.LookRotation(mouseDirection, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeedShift * Time.deltaTime);
                }
                

                //version with target box 
                /*
                if (mouseVertical != 0){
                    //move the target
                    if (Mathf.Abs(250f - Input.mousePosition.y) < 100f) {
                        target.transform.position = new Vector3(target.transform.position.x, target.transform.position.z, Input.mousePosition.y);
                    } 

                }

                if (mouseHorizontal != 0){

                    //move the target
                    if (Mathf.Abs(500f - Input.mousePosition.x) < 200f) {
                        target.transform.position = new Vector3(Input.mousePosition.x, target.transform.position.z, target.transform.position.y);
                    } else  if ((Input.mousePosition.x < 500f) && (mouseHorizontal < 0)){ //left

                        //rotate player
                        Quaternion toRotation = Quaternion.LookRotation(mouseDirection, Vector3.up);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeedShift * Time.deltaTime);
                    }else  if ((Input.mousePosition.x > 500f) && (mouseHorizontal > 0)) { //right

                        //rotate player
                        Quaternion toRotation = Quaternion.LookRotation(mouseDirection, Vector3.up);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeedShift * Time.deltaTime);
                    }
                
                }
                */
                
            }
        }
        
        //handle shift press
        if (Input.GetKey(KeyCode.LeftShift)){
            //animation trigger
            playerAnim.SetBool("Shift_Bool", true);
            //switch camera
            mainCamera.SetActive(false);
            shiftCamera.SetActive(true);
            //set boolean 
            shiftBool = true;
        } else if (Input.GetKeyUp(KeyCode.LeftShift)){
            
            //animation trigger
            playerAnim.SetBool("Shift_Bool", false);
            
            //reset focus position
            sphereFocus.transform.position = shiftCameraFocus.transform.position;
            //switch camera
            mainCamera.SetActive(true);
            shiftCamera.SetActive(false);
            //set booleans 
            shiftBool = false;
            bowAttack = false;
        }



        //handle animations and movement
        if(movementDirection != Vector3.zero) {
            //default movement animation : run 
            playerAnim.SetBool("Run_Bool", true);

            //Lock rotation if left shift is pressed
            if (shiftBool){
                
                //Handle the strafe animation
                if(verticalInput != 0){
                    if(verticalInput > 0){
                        playerAnim.SetInteger("Strafe_Int", 3);
                    } else {
                        playerAnim.SetInteger("Strafe_Int", 4);
                    }
                } else if(horizontalInput != 0){
                    if(horizontalInput > 0){
                        playerAnim.SetInteger("Strafe_Int", 1);
                    } else {
                        playerAnim.SetInteger("Strafe_Int", 2);
                    }
                }
            //unlock rotation    
            }  
        } else {
            //idle animation
            playerAnim.SetInteger("Strafe_Int", 0);
            playerAnim.SetBool("Run_Bool", false);
        } 

        //Player attck if mouse is clicked
        if (Input.GetMouseButtonDown(0)  && shiftBool){
            bowAttack = true;
            StartCoroutine(Attack());
        }
       
    }

    IEnumerator Attack(){
        //lock the player position
        playerCt.SimpleMove(new Vector3(0,0,0) * 0);
        //Animations handler
        playerAnim.SetTrigger("Attack_Trigger");
        bowAnim.SetTrigger("Attack_Trigger");
        
        //the player can move after 1 second 
        yield return new WaitForSeconds(1);
        bowAttack = false ;
    }

    //hide/show cursor on screen 
    private void OnApplicationFocus( bool focus) {
        if (focus) {
            Cursor.lockState = CursorLockMode.Locked;
        } else {
            Cursor.lockState = CursorLockMode.None;
        }
    }


    //Damage Handler
    private void OnCollisionEnter(Collision other){
        if (other.gameObject.name == "Enemy"){
            if (playerAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Attack"){
                Debug.Log("ATTACK");
            }
        }
    }
}
