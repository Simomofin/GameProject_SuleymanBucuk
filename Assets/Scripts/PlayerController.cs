using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   

    private SwerveMechanic _swerveMechanic;    
    private Animator _anim;
    private float _xMovementSpeed, _zMovementSpeed;        
    private GameObject _cam;
    private CameraFollower _camFollower;
    private bool isColliding = false; // declared to check multiple collisions, to prevent camera to rotate x and z axis etc. 
    private SphereCollider _sphereCollider;

    public float jumpForce;
    public Rigidbody _rb;
    public bool CanMove { get; private set; } = true;
    public bool GameHasFinished { get; private set; } = false;
    public bool IsWinOrLoseAnimationFinished { get; private set; } = false;
    public GameObject loseMenu;
    public GameObject paintableWall;
    public GameObject winMenu;

    private enum CharacterStatus
    {
        Idle,
        Running,
        Jumping,
        HittedBackByObstacle,
        Dead,
        Finished
    }
    private CharacterStatus charStatus;

    public float characterSpeed;

    private void OnEnable()
    {
        FinishLineController.OnPlayerWon += PlayerWonScenerio;
        FinishLineController.OnOpponentWon += OpponentWinScenerio;
    } // OnEnable

    private void OnDisable()
    {
        FinishLineController.OnPlayerWon -= PlayerWonScenerio;
    } // OnDisable

    // Start is called before the first frame update
    void Start()
    {
        paintableWall.SetActive(false);
        GameHasFinished = false;
        _swerveMechanic = GetComponent<SwerveMechanic>();
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        _cam = GameObject.FindGameObjectWithTag(HelperClass.Tags.mainCamera);
        _camFollower = _cam.GetComponent<CameraFollower>();
        //Debug.Log(_cam.name);      
        _sphereCollider = GetComponent<SphereCollider>();

    } // Start

    private void FixedUpdate()
    {
        if(CanMove && !GameHasFinished)
            Movement();        

        //Vector3 relativePOS = _finishLine.InverseTransformPoint(transform.position);
        //Debug.Log(relativePOS);
        //relativePOS.Normalize();
        //Debug.Log(relativePOS);
       
    } // FixedUpdate

    //private void Jump()
    //{
    //    _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    //    charStatus = CharacterStatus.Jumping;
    //}

    private void Movement()
    {
        _xMovementSpeed = _swerveMechanic.MoveX * characterSpeed * Time.fixedDeltaTime;
        _zMovementSpeed = _swerveMechanic.MoveZ * characterSpeed * Time.fixedDeltaTime;       

        if(_swerveMechanic.MoveX != 0 || _swerveMechanic.MoveZ != 0)
        {            

            if(charStatus != CharacterStatus.HittedBackByObstacle && charStatus != CharacterStatus.Dead && charStatus != CharacterStatus.Finished && charStatus != CharacterStatus.Jumping)
            {
                transform.Translate(new Vector3(_xMovementSpeed, 0, _zMovementSpeed), Space.World);
                _anim.SetBool(HelperClass.AnimationParameters.animIsRunningBoolean, true);
                // Rotate character face to moving direction.
                _rb.MoveRotation(Quaternion.LookRotation(new Vector3(_xMovementSpeed, 0, _zMovementSpeed)));
                if(charStatus != CharacterStatus.Running)
                    charStatus = CharacterStatus.Running;
            }
        }
        else if(_swerveMechanic.MoveX == 0 && _swerveMechanic.MoveZ == 0)
        {
            _anim.SetBool(HelperClass.AnimationParameters.animIsRunningBoolean, false);
        }
       
        // Clamp velocity between -1 and 1
        //if (_rb.velocity.magnitude > 1f)
        //    _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, 1f);              
    } // Movement   

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(HelperClass.Tags.rotatingStick))
        {
            //Debug.Log("I hit the stick");
            CanMove = false; // after hit, disable movement for a moment.
            charStatus = CharacterStatus.HittedBackByObstacle; // Change char status
            _anim.SetBool(HelperClass.AnimationParameters.animIsRunningBoolean, false);            
            Vector3 dir = collision.contacts[0].point - transform.position;
            //Debug.Log("dir bf normalized: " + dir);
            dir = -dir.normalized; // normalize on contrariwise to push back the character           
            dir.y = 0f; // dont apply for on y axis
            //Debug.Log("dir aft normalized: " + dir);
            _rb.AddForce(dir * 20, ForceMode.Impulse); 
            StartCoroutine(SetCanMove(true, 1.5f)); // enable movement after 1,5 sec, which is hit animation duration while we multiply anim speed by 1.33 in editor..
            _anim.SetTrigger(HelperClass.AnimationParameters.animIsHitByStick);
        }
        if(collision.gameObject.CompareTag(HelperClass.Tags.obstacleTag))
        {
            RespawnPlayer();
        }
        
    } // OnCollisionEnter

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag(HelperClass.Tags.rotatingPlatformTag))
        {
            Vector3 dir = new Vector3(-collision.gameObject.GetComponent<ObjectRotator>().zSpeed, 0, 0); // apply z rotation speed of rotating platform to characters x axis as force.
            //Debug.Log("Dir on rotating platform: " + dir);
            _rb.AddForce(dir * 0.5f, ForceMode.Force);
        }
    } // OnCollisionStay

    private void OnTriggerEnter(Collider other)
    {
        //if(other.gameObject.CompareTag(HelperClass.Tags.finishLine))
        //{
        //    GameHasFinished = true;
        //    CanMove = false;

        //}

        if (other.gameObject.CompareTag(HelperClass.Tags.obstacleTag))
        {
            RespawnPlayer();
        }

        if (other.gameObject.CompareTag(HelperClass.Tags.cameraRotator))
        {
            if(!isColliding)
            {
                isColliding = true;
                _camFollower.followOnZAxis = !_camFollower.followOnZAxis;
                //StartCoroutine(ResetCollingValue());
            }
        }
    } // OnTriggerEnter   

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(HelperClass.Tags.cameraRotator))
        {
            if (isColliding) // to prevent camera to rotate and translate continously and cause bug, switch isCollidng value on trigger exit,
                            // so camera does not glitch and swerve mechanic works effectively.
            {
                isColliding = false;                
            }
        }
    } // OnTriggerExit

    //IEnumerator ResetCollingValue()
    //{
    //    yield return new WaitForSeconds(0.33f);
    //    isColliding = false;
    //} // ResetCollingValue


    IEnumerator SetCanMove(bool val, float t)
    {
        yield return new WaitForSeconds(t);
        CanMove = val;
        charStatus = CharacterStatus.Idle;
    } // SetCanMove

    private void PlayerWonScenerio()
    {        
        GameHasFinished = true;
        CanMove = false;
        Debug.Log("Player Won the game");
        _anim.SetBool(HelperClass.AnimationParameters.animIsRunningBoolean, false);
        _anim.SetTrigger(HelperClass.AnimationParameters.winTheGame);
        winMenu.SetActive(true);

        StartCoroutine(LocateCamToFPS());

    } // PlayerWonScenerio

    private void OpponentWinScenerio()
    {
        // TODO: Set an loser animation here and disable movements. Set a UI screen to restart game.(Simply reload the scene)
        CanMove = false;
        _anim.SetTrigger(HelperClass.AnimationParameters.loseTheGame);
        _anim.SetBool(HelperClass.AnimationParameters.animIsRunningBoolean, false);
        loseMenu.SetActive(true);
        GameHasFinished = true;
        

        StartCoroutine(LocateCamToFPS());
    }

    IEnumerator LocateCamToFPS()
    {
        yield return new WaitUntil(() => IsWinOrLoseAnimationFinished == true); // Suspend the code till animation finishes, than show must go on.
        winMenu.SetActive(false);
        loseMenu.SetActive(false);
        //Debug.Log("I am working.");       
    }

    /// <summary>
    /// This func is decleared to inform LocateCamFPS coroutine about game end animation is finished to play. This is used on Editor -> Animation window.
    /// </summary>
    public void SetAnimationFinishedBoolean()
    {
        IsWinOrLoseAnimationFinished = true;
    } // SetAnimationFinishedBoolean

    private void RespawnPlayer()
    {
        _camFollower.followOnZAxis = true;
        transform.position = new Vector3(0, 0.1f, 0);

        GameManager.sharedInstance.ActivateUpsideFences();
    } // RespawnPlayer

} // Class
