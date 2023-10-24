using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBowController : MonoBehaviour
{
    private Rigidbody playerRb;
    private Animator playerAnim;
    public Animator bowAnim;

    public GameObject hand;
    public GameObject head;
    public GameObject target;
    public GameObject mainCamera;
    public GameObject shiftCamera;
    private float horizontalInput;
    private float verticalInput;
    private float mouseHorizontal;
    private float mouseVertical;
    private float rotationSpeed = 500.0f; //speed rotation when shift is'nt pressed
    private float rotationSpeedShift = 200.0f; //rotation speed when Shift
    public float speed = 5.0f;
    public bool shiftBool;
 
    public static bool bowAttack = false;

    private CharacterController playerCt;

    [SerializeField]
    private Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody>();
        playerCt = GetComponent<CharacterController>();

        

        mainCamera.SetActive(true);
        shiftCamera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {   
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");


        Vector3 mouseDirection = new Vector3(mouseHorizontal, 0, 0);
        Vector3 headDirection = new Vector3(0, 0, mouseVertical);
        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);

        //Set the vectors
        if(shiftBool) {
            target.SetActive(true);
            headDirection = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up)* headDirection;
            mouseDirection = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up)* mouseDirection;
            movementDirection = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up)* movementDirection;
        } else {
            target.SetActive(false);
            movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up)* movementDirection;
        }

        //mouseDirection.Normalize();
        movementDirection.Normalize();
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) *speed;
        
        //Move the player
        if (!bowAttack){
            playerCt.Move(movementDirection * Time.deltaTime * magnitude );
            if (!shiftBool){
                //make the player look where it goes
                Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            } else {
        


                OnApplicationFocus(false);
                
                
                Debug.Log("position");
                Debug.Log( headDirection);

                

                if (mouseVertical != 0) {
                    //move the target
                    //rotate player
                    Quaternion toRotation = Quaternion.LookRotation(headDirection, Vector3.up);
                    head.transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeedShift * Time.deltaTime);

                }

                if (mouseHorizontal != 0){
                    //rotate player
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
            playerAnim.SetBool("Shift_Bool", true);
            //switch camera
            mainCamera.SetActive(false);
            shiftCamera.SetActive(true);
            shiftBool = true;
        } else {
            //go back to run animation
            playerAnim.SetBool("Shift_Bool", false);
            shiftBool = false;
            //switch camera
            mainCamera.SetActive(true);
            shiftCamera.SetActive(false);
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
        if (Input.GetMouseButtonDown(0)){
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
