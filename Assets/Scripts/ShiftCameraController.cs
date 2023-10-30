using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftCameraController : MonoBehaviour
{

    private float horizontalInput;
    private float verticalInput;
    public float rotationSpeed = 50;
    // Start is called before the first frame update
    void Start()
    {
        horizontalInput = Input.GetAxis("Mouse X");
        verticalInput = Input.GetAxis("Mouse Y");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotationDirection  = new Vector3(horizontalInput, 0, verticalInput);
        transform.Rotate(rotationDirection*rotationSpeed, Space.Self);
    }
}
