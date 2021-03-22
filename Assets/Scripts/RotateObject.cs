using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float RotationSpeed;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime, Space.World); // Rotates around y axis.
    } // FixedUpdate
} // Class
