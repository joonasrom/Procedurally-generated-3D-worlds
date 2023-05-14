using UnityEngine;

public class Objects : MonoBehaviour
{
    private void Start()
    {
        // Call the FindLand method to move the object to the ground
        FindLand();
    }

    private void FindLand()
    {
        // Create a new ray from the objects position pointing downwards
        Ray ray = new Ray(transform.position, -transform.up);
        
        // Cast the ray and check if it hits anything
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // If the ray hits something, move the object to the hit point
            transform.position = hitInfo.point;
        }
        else
        {
            // If the ray doesn't hit anything, cast a new ray upwards to find the ceiling
            ray = new Ray(transform.position, transform.up);
            
            // Cast the new ray and check if it hits anything
            if (Physics.Raycast(ray, out hitInfo))
            {
                // If the ray hits something, move the object to the hit point
                transform.position = hitInfo.point;
            }
        }
    }
}
