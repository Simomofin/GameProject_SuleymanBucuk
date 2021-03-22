using UnityEngine;

public class HorizontalObstacle : MonoBehaviour
{    
    [Tooltip("Assing maximum movement amount for the proper axis, for example, if you move the obstacle on X axis, assign maximum x coordinate to move to.")]
    public float xBorder, yBorder, zBorder;
    public enum WhichAxisToMove
    {
        X,
        Y,
        Z
    }
    public WhichAxisToMove whichAxisToMove = new WhichAxisToMove();
    public float speed;

    private Vector3 targetPos, targetPos1, targetPos2;    

    // Start is called before the first frame update
    void Start()
    {
        switch (whichAxisToMove)
        {
            case WhichAxisToMove.X:
                MoveAtXCoordinate();
                break;
            case WhichAxisToMove.Y:
                MoveAtYCoordinate();
                break;
            case WhichAxisToMove.Z:
                MoveAtZCoordinate();
                break;
            default:
                break;
        }
    } // Start

    private void FixedUpdate()
    {
        if (transform.position == targetPos2)
            targetPos = targetPos1;
        if (transform.position == targetPos1)
            targetPos = targetPos2;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.fixedDeltaTime);
    } // FixedUpdate

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("I'm colliding with: " + collision.gameObject.name);
        if(collision.gameObject.CompareTag(HelperClass.Tags.opponent))
        {
            collision.gameObject.GetComponent<OpponentController>().ReSpawn();
        }
    } // OnCollisionEnter

    private void MoveAtXCoordinate()
    {
        targetPos1 = new Vector3(transform.position.x + xBorder, transform.position.y, transform.position.z);
        targetPos2 = new Vector3(transform.position.x -xBorder, transform.position.y, transform.position.z);
        targetPos = targetPos1;
    } // MoveAtXCoordinate

    private void MoveAtYCoordinate()
    {
        targetPos1 = new Vector3(transform.position.x, transform.position.y + yBorder, transform.position.z);
        targetPos2 = new Vector3(transform.position.x, transform.position.y, transform.position.z); // -yBorder is not declared, not to shrink on the floor. 
        targetPos = targetPos1;
    } // MoveAtYCoordinate

    private void MoveAtZCoordinate()
    {
        targetPos1 = new Vector3(transform.position.x, transform.position.y, transform.position.z + zBorder);
        targetPos2 = new Vector3(transform.position.x, transform.position.y, transform.position.z -zBorder);
        targetPos = targetPos1;
    } // MoveAtZCoordinate


} // Class
