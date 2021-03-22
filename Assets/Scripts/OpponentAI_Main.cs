using UnityEngine;

public class OpponentAI_Main : MonoBehaviour
{

    public enum MoveDirection
    {
        Straight,
        Right,
        Left
    }
    public MoveDirection moveDirection = new MoveDirection();

    private RaycastHit _middleRayHit, _rightRayHit, _leftRayHit;
    private bool _middleHit, _rightHit, _leftHit;
    private float _hit1Distance, _hit2Distance, _hit3Distance;

    [SerializeField]
    private Transform ray1Transform, ray2Transform, ray3Transform;


    // Start is called before the first frame update
    void Start()
    {
        
    }
   
    private void LateUpdate()
    {
        if (AI_Detection.SharedInstance.obstacleDetected || AI_Detection.SharedInstance.barriedDetected)
            CastARay();
    } // LateUpdate

    private void CastARay()
    {
        float rayDistance = 0.5f;
        //1.Ray
        if (Physics.Raycast(ray1Transform.position, transform.TransformDirection(Vector3.forward), out _middleRayHit, rayDistance))
        {
            Debug.DrawRay(ray1Transform.position, transform.TransformDirection(Vector3.forward) * rayDistance, Color.yellow);
            _middleHit = true;
        }
        else
            _middleHit = false;
        //2.Ray
        if (Physics.Raycast(ray2Transform.position, transform.TransformDirection(Vector3.forward), out _rightRayHit, rayDistance))
        {
            Debug.DrawRay(ray2Transform.position, transform.TransformDirection(Vector3.forward) * rayDistance, Color.yellow);
            _rightHit = true;
        }
        else
            _rightHit = false;
        //3.Ray
        if (Physics.Raycast(ray3Transform.position, transform.TransformDirection(Vector3.forward), out _leftRayHit, rayDistance))
        {
            Debug.DrawRay(ray3Transform.position, transform.TransformDirection(Vector3.forward) * rayDistance, Color.yellow);
            _leftHit = true;
        }
        else
            _leftHit = false;

        MakeADecision(_middleRayHit, _rightRayHit, _leftRayHit);
    } // CastARay

    private void MakeADecision(RaycastHit midleRayHit, RaycastHit rightRayHit, RaycastHit leftRayHit)
    {
        //Debug.Log(midleRayHit.collider.name+ " " + rightRayHit.collider.name + " " + leftRayHit.collider.name);

        if(_leftHit && (!_middleHit && !_rightHit)) // if only left side is hit and the tag is barrier, avoid this contact.
        {
            if (leftRayHit.collider.gameObject.CompareTag(HelperClass.Tags.barrier))
                _leftHit = false;
        } 
        if(_rightHit && (!_middleHit && !_leftHit))
        {
            if (rightRayHit.collider.gameObject.CompareTag(HelperClass.Tags.barrier))
                _rightHit = false;
        }

        /*Conditions
         * 1) None of the rays hit
         * 2) Left is no hit other two hits
         * 3) Right is no hit other two hits
         * 4) Left and right no hit, middle hits
         * */
        //1)
        if (!_middleHit && !_rightHit && !_leftHit)
            moveDirection = MoveDirection.Straight;
        else if (!_leftHit && (_middleHit && _rightHit))
            moveDirection = MoveDirection.Left;
        else if (!_rightHit && (_middleHit && _leftHit))
            moveDirection = MoveDirection.Right;
        else if (_middleHit)
        {
            if (!_rightHit)
                moveDirection = MoveDirection.Right;
            else if (!_leftHit)
                moveDirection = MoveDirection.Left;
            else
            {
                int rand = Random.Range(0, 2);
                moveDirection = rand == 0 ? MoveDirection.Right : MoveDirection.Left;
            }
        }

    } // MakeADecision

    private void OnCollisionStay(Collision collision)
    {
        if (moveDirection == MoveDirection.Left && collision.collider.gameObject.CompareTag(HelperClass.Tags.barrier))
            moveDirection = MoveDirection.Right;
        if (moveDirection == MoveDirection.Right && collision.collider.gameObject.CompareTag(HelperClass.Tags.barrier))
            moveDirection = MoveDirection.Left;
    }
}
