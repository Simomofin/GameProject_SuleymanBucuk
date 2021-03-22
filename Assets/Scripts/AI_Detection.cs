using UnityEngine;

/// <summary>
/// This script is attached to ColliderParentForAI on Girl Gameobject to detect anything is in front of the opponent character
/// </summary>
public class AI_Detection : MonoBehaviour
{
    public static AI_Detection SharedInstance;

    public bool obstacleDetected = false;
    public bool barriedDetected = false;
    public bool rotatingStickDetected = false;

    private void OnEnable()
    {
        if (SharedInstance == null)
            SharedInstance = this;
        else
            Destroy(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag(HelperClass.Tags.obstacleTag))
        {
            obstacleDetected = true;
        }
        if(other.gameObject.CompareTag(HelperClass.Tags.barrier))
        {
            barriedDetected = true;
        }
        if(other.gameObject.CompareTag(HelperClass.Tags.rotatingStick))
        {

        }

    } // OnTriggerEnter

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(HelperClass.Tags.obstacleTag))
        {
            obstacleDetected = false;
        }
        if (other.gameObject.CompareTag(HelperClass.Tags.barrier))
        {
            barriedDetected = false;
        }
        if (other.gameObject.CompareTag(HelperClass.Tags.rotatingStick))
        {

        }

    } // OnTriggerExit

} // Class
