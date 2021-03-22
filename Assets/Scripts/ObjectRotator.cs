using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    public bool rotateX, rotateY, rotateZ;
    public float xSpeed, ySpeed, zSpeed;

    private float rbFactor = 0.01f; // When player stands on a rotating obj, this factor applies this amount of force to player's rigidbody
                                    // if you wanna make the level harder to complete, increse this factor.
    private PlayerController _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        if(rotateX)
        {
            transform.Rotate(Vector3.right * xSpeed * Time.fixedDeltaTime);
        }

        if(rotateY)
        {
            transform.Rotate(Vector3.up * ySpeed * Time.fixedDeltaTime);
        }

        if(rotateZ)
        {
            transform.Rotate(Vector3.forward * zSpeed * Time.fixedDeltaTime);
        }

    } // FixedUpdate

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag(HelperClass.Tags.playerTag))
        {
            if (_player == null)
                Debug.LogError("An error here to find the Player!");
            else
            {
                _player._rb.velocity += new Vector3(-zSpeed * rbFactor, 0, 0);
                //Debug.Log("Adding rotation velocity: " + _player._rb.velocity);
            }
        }
    } // OnCollisionStay

} // Class
