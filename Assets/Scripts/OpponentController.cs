using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class OpponentController : MonoBehaviour
{        
    private Animator _anim;
    private Rigidbody _rb;
    private NavMeshAgent _agent;
    private AI_Opponent _opponentAI;
    private GameObject _targetDestination = null;
    private bool _fSMState1Set = false;
    private bool _fSMState2Set = false;
    private bool _fSMState3Set = false;
    private bool isWPDeclared = false;
    private NavMeshPath _navMeshPath;
    private bool isHitByStick = false;    
    
    public bool isFinished = false;

    public enum FSMStates // Finit State Machine
    {
        FindingWP,
        TranslateToWP,
        ReachedDestination
    }
    public FSMStates fsmStates;


    private void OnEnable()
    {
        FinishLineController.OnOpponentWon += OnOpponentWonScenerio;
        FinishLineController.OnPlayerWon += OnPlayerWonScenerio;
        GameManager.OnWayPointsDecleared += WayPointsDecleared;
    } // OnEnable



    private void OnDisable()
    {
        FinishLineController.OnOpponentWon -= OnOpponentWonScenerio;
        FinishLineController.OnPlayerWon -= OnPlayerWonScenerio;
        GameManager.OnWayPointsDecleared += WayPointsDecleared;
    } // OnDisable

    // Start is called before the first frame update
    void Start()
    {
        this._opponentAI = this.GetComponent<AI_Opponent>();
        _navMeshPath = new NavMeshPath();

        _agent = this.GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        int rand = Random.Range(8, 11);
        _agent.speed = rand * 0.5f; // randomly adjust speed between 3, 3.5, 4..

        fsmStates = FSMStates.FindingWP; // Set fsm state to FindingWP at start.        

        _anim = this.GetComponent<Animator>();
        _rb = this.GetComponent<Rigidbody>();        
        
    } // Start
       

    private void FixedUpdate()
    {
        if (!isWPDeclared)
            return;
        if (isFinished)
            return;

        if(fsmStates == FSMStates.FindingWP)
        {
            //TODO: Find the nex WP
            /*Steps:
             * 1) Which WP are you on?
             */
            if(!_fSMState2Set) _fSMState2Set = true;
            if(!_fSMState3Set) _fSMState3Set = true;
             if(!_fSMState1Set)
                this._targetDestination = _opponentAI.NextWayPoint();
            if (_targetDestination != null)
            {
                this.fsmStates = FSMStates.TranslateToWP;
                _fSMState1Set = true;
                _fSMState2Set = false;
            }
            else
                _fSMState1Set = false;
            /* 2) Return next Wp?
            * 3) Submit the next way point to next state
            */
        } // if(fsmStates == FSMStates.FindingWP)

        else if(fsmStates == FSMStates.TranslateToWP)
        {
            // TODO: Translate to target WP
            /*Steps
            * 1) Get WP            
            * 2) Check if next WP is reachable (CalculatePath)
            * 3) Set agent's target WP (SetDestination())
            * 4) Submit next state
             */
            if (!_fSMState2Set)
            {
                
                _agent.enabled = true;
                _agent.SetDestination(_targetDestination.transform.position);
                if (_agent.CalculatePath(_targetDestination.transform.position, _navMeshPath))
                {
                    fsmStates = FSMStates.ReachedDestination;
                    _fSMState2Set = true;
                    _fSMState3Set = false;
                }
                else
                    _fSMState2Set = false;
            }

           
        } // else if(fsmStates == FSMStates.TranslateToWP)
        else if(fsmStates == FSMStates.ReachedDestination)
        {
            // TODO: Check if target destination is reached
            /*Steps
             * 1) Check if agent reached target destination
             * 2) If next wp is reached Submit next state             
             */
            if (!_fSMState3Set)
            {
                if ((transform.position - _targetDestination.transform.position).sqrMagnitude < 0.5f)
                {
                    //_agent.enabled = false;
                    _fSMState1Set = false;
                    fsmStates = FSMStates.FindingWP;
                    _fSMState3Set = true;
                    
                }
            }

        } // else if(fsmStates == FSMStates.ReachedDestination)       
    } // FixedUpdate

    private void LateUpdate()
    {
        if (_agent.velocity.sqrMagnitude > Mathf.Epsilon && !isHitByStick)
        {
            transform.rotation = Quaternion.LookRotation(_agent.velocity.normalized);
            _anim.SetBool(HelperClass.AnimationParameters.animIsRunningBoolean, true);
        }
        else
            _anim.SetBool(HelperClass.AnimationParameters.animIsRunningBoolean, false);
    } // LateUpdate


    private void OnCollisionEnter(Collision collision)
    {       
        if (collision.gameObject.CompareTag(HelperClass.Tags.rotatingStick))
        {
            if (isHitByStick)
                return;

            Debug.Log(this.name + " is colliding with: " + collision.gameObject.name);
            isHitByStick = true;
            _agent.enabled = false;            
            _anim.SetBool(HelperClass.AnimationParameters.animIsRunningBoolean, false);
            _anim.SetTrigger(HelperClass.AnimationParameters.animIsHitByStick);
            Vector3 dir = collision.contacts[0].point - transform.position;
            //Debug.Log("dir bf normalized: " + dir);
            dir = -dir.normalized; // normalize on contrariwise to push back the character           
            dir.y = 0f; // dont apply for on y axis
            //Debug.Log("dir aft normalized: " + dir);
            _rb.isKinematic = false;
            _rb.AddForce(dir * 30, ForceMode.Impulse);
            //throw new NullReferenceException();
            // TODO: check here for appliying force to opponent.
            StartCoroutine(EnableAgent());
        }

        //if (collision.gameObject.CompareTag(HelperClass.Tags.obstacleTag))
        //{
        //    //Debug.Log("I am colliding with: " + collision.gameObject.name);
        //    ReSpawn();
        //}

    } // OnCollisionEnter   

    IEnumerator EnableAgent()
    {
        yield return new WaitForSeconds(1.33f);
        _rb.isKinematic = true;
        isHitByStick = false;
        _fSMState1Set = false;
        fsmStates = FSMStates.FindingWP;
    } // EnableAgent



    private void OnTriggerEnter(Collider other)
    {       
        if (other.gameObject.CompareTag(HelperClass.Tags.obstacleTag))
        {
            //Debug.Log(this.name + " is colliding with: " + other.gameObject.name);
            ReSpawn();
        }        
    } // OnTriggerEnter

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag(HelperClass.Tags.rotatingPlatformTag))
        {
            Vector3 dir = new Vector3(0, 0, collision.gameObject.GetComponent<ObjectRotator>().zSpeed);
            Debug.Log("Dir on rotating platform: " + dir);
           
            _rb.AddForce(dir * 30f, ForceMode.Impulse);           
        }
    } // OnCollisionStay

    public void ReSpawn()
    {
        //Debug.Log(this.name + " is respawning");
        _agent.enabled = false;
        float xPOS = Random.Range(4, -5);
        float zPOS = Random.Range(0, -18);        
        transform.position = new Vector3(xPOS,0,zPOS);        

        _opponentAI.nextWayPoint = null;
        _targetDestination = null;
        fsmStates = FSMStates.FindingWP;
        
        //throw new NullReferenceException();
        //TODO: Check here...
    } // ReSpawn        

    private void OnPlayerWonScenerio()
    {
        // TODO: Disable opponent agent and movements.
        isFinished = true;
        _agent.ResetPath();
        _agent.enabled = false;
    } // OnPlayerWonScenerio

    private void OnOpponentWonScenerio()
    {
        isFinished = true;
        _agent.ResetPath();
        _agent.enabled = false;
        this._anim.SetBool(HelperClass.AnimationParameters.animIsRunningBoolean, false);
        this._anim.SetTrigger(HelperClass.AnimationParameters.winTheGame);
    } // OnOpponentWonScenerio

    private void WayPointsDecleared()
    {
        isWPDeclared = true;
    } // WayPointsDecleared
} // Class
