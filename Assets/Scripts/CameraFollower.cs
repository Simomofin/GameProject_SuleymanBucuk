using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    private GameObject _player;
    private Vector3 _camOffsetOnZAxis, _camOffsetOnXAxis;
    private PlayerController _playerController;
    private float _cameraRotationSpeed = 5f;
    private float cameraFollowSpeed = 5.5f;
    
    public Transform playerFPSCamSlot;
    public GameObject paintableWall;
    public bool followOnZAxis;
    
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerController = _player.GetComponent<PlayerController>();
        followOnZAxis = true;


    } // Awake
    void Start()
    {
        _camOffsetOnZAxis = new Vector3(0f, -14.4f, 19.8f);
        _camOffsetOnXAxis = new Vector3(_camOffsetOnZAxis.z, _camOffsetOnZAxis.y, _camOffsetOnZAxis.x);
        //_camOffset = _player.transform.position - transform.position;
        //Debug.Log(_camOffset);
    } // Start

    private void FixedUpdate()
    {        
        if(followOnZAxis)
        {
            if (!_playerController.GameHasFinished)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(_player.transform.position.x, _player.transform.position.y - _camOffsetOnZAxis.y, _player.transform.position.z - _camOffsetOnZAxis.z),
                cameraFollowSpeed * Time.fixedDeltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(16f, 0f, 0f)), _cameraRotationSpeed * Time.fixedDeltaTime);
            }
                
            else if (_playerController.GameHasFinished && _playerController.IsWinOrLoseAnimationFinished) // if game has finished and win anim finished playing, move camera to FPS slot and rotate it to wall.
            {
                cameraFollowSpeed -= Time.fixedDeltaTime;
                transform.position = Vector3.Lerp(transform.position, playerFPSCamSlot.position, cameraFollowSpeed * 0.5f * Time.fixedDeltaTime);
                _cameraRotationSpeed += Time.fixedDeltaTime;
                transform.rotation = Quaternion.Lerp(transform.rotation, _player.transform.rotation, _cameraRotationSpeed * Time.fixedDeltaTime);
            }
        }
        else
        {
            if (!_playerController.GameHasFinished)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(_player.transform.position.x - _camOffsetOnXAxis.x, _player.transform.position.y - _camOffsetOnXAxis.y, _player.transform.position.z),
                   cameraFollowSpeed * Time.fixedDeltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(16f, 90f, 0f)), _cameraRotationSpeed * Time.fixedDeltaTime);
            }

            else if (_playerController.GameHasFinished && _playerController.IsWinOrLoseAnimationFinished) // if game has finished and win anim finished playing, move camera to FPS slot and rotate it to wall.
            {
                cameraFollowSpeed -= Time.fixedDeltaTime;
                transform.position = Vector3.Lerp(transform.position, playerFPSCamSlot.position, cameraFollowSpeed * 0.5f * Time.fixedDeltaTime);
                _cameraRotationSpeed += Time.fixedDeltaTime;
                transform.rotation = Quaternion.Lerp(transform.rotation, _player.transform.rotation, _cameraRotationSpeed * Time.fixedDeltaTime);
            }
        }
                 

    } // FixedUpdate    

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Cam is in fpsCamHolder");
        if(other.gameObject.CompareTag(HelperClass.Tags.fpsCameraHolder))
        {
            paintableWall.SetActive(true);
            paintableWall.GetComponent<PaintableWall>().SetWall();
        }
        if (other.gameObject.CompareTag(HelperClass.Tags.upsideFence))
        {
            //Debug.Log("Hit upside fence");           
            other.gameObject.SetActive(false);
        }
    } // OnTriggerEnter

} // Class
