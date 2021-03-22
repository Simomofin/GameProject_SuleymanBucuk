using TMPro.Examples;
using UnityEngine;

public class SwerveMechanic : MonoBehaviour
{
    private float _lastFingerPositionX;
    private float _lastFingerPositionY;
    private float _moveX, _moveZ;
    private float _newMoveX, _newMoveZ;

    private CameraFollower _camFollower;

    public float MoveX => _moveX;
    public float MoveZ => _moveZ;

    private void Start()
    {
        _camFollower = GameObject.FindGameObjectWithTag(HelperClass.Tags.mainCamera).GetComponent<CameraFollower>();
        if (_camFollower == null)
            Debug.LogError("No CameraController Found!");
    } // Start

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            _lastFingerPositionX = Input.mousePosition.x;
            _lastFingerPositionY = Input.mousePosition.y;
        }   
        else if(Input.GetMouseButton(0))
        {
            //Debug.Log("X :" + _moveX + "Y :" +_moveZ);

            if (_lastFingerPositionX == Input.mousePosition.x && _lastFingerPositionY == Input.mousePosition.y)
                return;

            //_newMoveX = _moveX;
            //_newMoveZ = _moveZ;
            if(_camFollower.followOnZAxis) // if camera and player move on z axis, swerve mechanic works like below
            {
                _newMoveX = Input.mousePosition.x - _lastFingerPositionX; // movement on x axis on screen, moves player on x axis straight
                _moveX = Mathf.Lerp(_moveX, _newMoveX, Time.deltaTime);
                _moveX = Mathf.Clamp(_moveX, -1, 1);
                _lastFingerPositionX = Input.mousePosition.x;

                _newMoveZ = Input.mousePosition.y - _lastFingerPositionY; // movement on y axis on screen, moves player on z axis straight
                _moveZ = Mathf.Lerp(_moveZ, _newMoveZ, Time.deltaTime);
                _moveZ = Mathf.Clamp(_moveZ, -1, 1);
                _lastFingerPositionY = Input.mousePosition.y;
            }
            else // if camera and player move on x axis, swerve mechanic works like below
            {
                _newMoveX = Input.mousePosition.y - _lastFingerPositionY; // movement on y axis on screen, moves player on x axis straight
                _moveX = Mathf.Lerp(_moveX, _newMoveX, Time.deltaTime);
                _moveX = Mathf.Clamp(_moveX, -1, 1);
                _lastFingerPositionY = Input.mousePosition.y;

                _newMoveZ = -(Input.mousePosition.x - _lastFingerPositionX); // movement on x axis on screen, moves player on z axis reversely.
                _moveZ = Mathf.Lerp(_moveZ, _newMoveZ, Time.deltaTime);
                _moveZ = Mathf.Clamp(_moveZ, -1, 1);
                _lastFingerPositionX = Input.mousePosition.x;
            }
            
        }
        
        if(Input.GetMouseButtonUp(0))
        {
            _moveX = 0;
            _moveZ = 0;
        }

    } // FixedUpdate    

} // Class
