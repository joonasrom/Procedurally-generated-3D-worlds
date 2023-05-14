using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

public Transform orientation;
public float sensitivityX;
public float sensitivityY;

float rotationX;
float rotationY;

    // On start hides cursor and locks it in place
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Gets inputs of mouses Y and X axes and transforms rotation based on input
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.fixedDeltaTime * sensitivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.fixedDeltaTime * sensitivityY;
    
    rotationY += mouseX;
    rotationX += mouseY;
    rotationX = Mathf.Clamp(rotationX, -90f, 90f);

    transform.rotation = Quaternion.Euler(-rotationX, rotationY, 0);
    orientation.rotation = Quaternion.Euler(0, rotationY, 0);

    }
}
