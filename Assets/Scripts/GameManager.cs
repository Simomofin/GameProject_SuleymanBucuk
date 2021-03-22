using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable] // To see all racers in inspector in runtime.
public class Racers : IComparable<Racers>
{
    public GameObject racer;
    public float distToFinish;
    public int racerType; // 0 for opponents, 1 for player ==> string or enum isn't used bc fixedupdate or update is not appropriate to use these type, int rocks.
    public Canvas canvas;

    public Racers(GameObject racer, float distToFinish, int racerType)
    {
        this.racer = racer;
        this.distToFinish = distToFinish;
        this.racerType = racerType;
    }   

    public int CompareTo(Racers other)
    {
        return other.distToFinish.CompareTo(this.distToFinish);
    }
} // Racers

public class GameManager : MonoBehaviour
{
    public delegate void WaypointsDecleared();
    public static event WaypointsDecleared OnWayPointsDecleared;

    public static GameManager sharedInstance;  
    
    private GameObject[] _upsideFences;
    public TextMeshProUGUI rankingText;
    
    private Camera _mainCamera;    
    private List<GameObject> _opponentsList = new List<GameObject>();
    private GameObject _player;
    private bool isWPDecleared = false;
    private Dictionary<int, float> _racers = new Dictionary<int, float>();

    private float screenWidth;
    private float screenHeight;

    #region Ranking Variables
    private float _currentOpponentDist = 0;
    private Vector3 _finishLinePosition;
    private List<Racers> _racersList;
    #endregion

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (sharedInstance != null)
            Destroy(gameObject);
        else
            sharedInstance = this;

        

        _mainCamera = GameObject.FindGameObjectWithTag(HelperClass.Tags.mainCamera).GetComponent<Camera>();
        if (_mainCamera == null)
            Debug.Log("No Cam Found, pay attention here");
        SetScreenSizeToDevice();
    } // Awake   

    void Start()
    {

        _upsideFences = GameObject.FindGameObjectsWithTag(HelperClass.Tags.upsideFence);
       
        _player = GameObject.FindGameObjectWithTag(HelperClass.Tags.playerTag);

        foreach (GameObject go in GameObject.FindGameObjectsWithTag(HelperClass.Tags.opponent))
        {
            _opponentsList.Add(go);
        }
        //Debug.Log(_opponentsList.Count);
        SetOpponentWayToFollow();

        _finishLinePosition = GameObject.FindGameObjectWithTag(HelperClass.Tags.finishLine).transform.position;

        _opponentsList.Add(_player); // add player to opponent list to compare distance to finish line.
        _racersList = new List<Racers>();

        for (int i = 0; i < _opponentsList.Count; i++)
        {
            //_currentOpponentDist = Vector3.Distance(_opponentsList[i].transform.position, _finishLinePosition);
            if (i == _opponentsList.Count - 1) // we added the player to end of the list, so if i indicates the last index that means player
                _racersList.Add(new Racers(_opponentsList[i], _currentOpponentDist, 1)); // 1 = player
            else
                _racersList.Add(new Racers(_opponentsList[i], _currentOpponentDist, 0)); // 0 = opponent.            
        }
        //GameObject[] gos = GameObject.FindGameObjectsWithTag("FPSCamHolder");
        //foreach (GameObject item in gos)
        //{
        //    Debug.Log(item.name);
        //}

    } // Start

    private void LateUpdate()
    {
        if (!isWPDecleared) //  Wait for WP decleration
            return;
        UpdateRanking();
    } // FixedUpdate

    private void UpdateRanking()
    {
        /***************************************************************
         * Below codes works to check if player is in first rank or not, but not showing the rank in real time like 2nd 3th 4th etc.
        for (int i = 0; i < _opponentsList.Count; i++)
        {
            _currentOpponentDist = Vector3.Distance(_opponentsList[i].transform.position, _finishLinePosition);
            if(_currentOpponentDist < _minimumOpponentDist)
            {
                _minimumOpponentDist = _currentOpponentDist;
                _topRankingOpponent = _opponentsList[i];
            }
        }
        _playerDist = Vector3.Distance(_player.transform.position, _finishLinePosition);
        if (_playerDist <= _minimumOpponentDist)
            Debug.Log("Player Rank: 1");        
         *****************************************************************/
        
        for (int i = 0; i < _racersList.Count; i++)
        {
            _currentOpponentDist = Vector3.Distance(_racersList[i].racer.transform.position, _finishLinePosition);            
            _racersList[i].distToFinish = _currentOpponentDist;            
        }
        _racersList.Sort();
        _racersList.Reverse();

        for (int i = 0; i < _racersList.Count; i++)
        {
            if (_racersList[i].racerType == 1)
            rankingText.text = "PLAYER RANKING: " + (i + 1);
        }        
    } // UpdateRanking    

    private void SetOpponentWayToFollow()
    {
        List<int> wpIndexes = new List<int>();
        // 0 for Right1, 1 for Right2, 2 for Left1, 3 for Left2
        wpIndexes.Add(0);
        wpIndexes.Add(0);        
        wpIndexes.Add(1);
        wpIndexes.Add(1);
        wpIndexes.Add(2);
        wpIndexes.Add(2);
        wpIndexes.Add(3);
        wpIndexes.Add(3);
        wpIndexes.Add(1);
        wpIndexes.Add(2);


        for (int i = 0; i < _opponentsList.Count; i++)
        {
            int rand = UnityEngine.Random.Range(0, wpIndexes.Count);
            switch (wpIndexes[rand])
            {
                case 0:
                    _opponentsList[i].GetComponent<AI_Opponent>().whichWayToFollow = AI_Opponent.HighWays.Right1;
                    break;

                case 1:
                    _opponentsList[i].GetComponent<AI_Opponent>().whichWayToFollow = AI_Opponent.HighWays.Right2;
                    break;

                case 2:
                    _opponentsList[i].GetComponent<AI_Opponent>().whichWayToFollow = AI_Opponent.HighWays.Left1;
                    break;

                case 3:
                    _opponentsList[i].GetComponent<AI_Opponent>().whichWayToFollow = AI_Opponent.HighWays.Left2;
                    break;

                default:
                    break;
            } // switch            
            wpIndexes.RemoveAt(rand); // Delete the assigned WP from the list to prevent it's been used several time.
        } // for
        //Fire an event to inform opponent characters about their WP' s are decleared.
        OnWayPointsDecleared?.Invoke();
        isWPDecleared = true;
        
    } // SetOpponentWayToFollow
    

    /*Below codes are written to find next waypoint
     * but AI_Opponent.cs is written to control AI states and these are commented.Finit State Machines are more reliable.
     */
        //public Transform GiveMeAWaypoint(int lastWPIndex, OpponentController _opponent)
        //{
        //    int requestedWPIndex = lastWPIndex;   

        //    for (int i = lastWPIndex; i < wayPoints.Length; i++)
        //    {
        //        requestedWPIndex++;
        //        if (wayPoints[requestedWPIndex].howManyOpponentOnThisWP < 3/* && wayPoints[requestedWPIndex].howManyOpponentOnThisWP > 0*/) // if this WP has not more than 2 opponent
        //        {
        //            Debug.Log("Less than 2 opponent here");
        //            if (wayPoints[lastWPIndex].whichAxis == wayPoints[requestedWPIndex].whichAxis) // if current WP and next WP axis is same, calculate next WP distance
        //            {
        //                Debug.Log("On same axis");
        //                if (wayPoints[requestedWPIndex].whichAxis == WaypointManager.WhichAxisYouAreMoving.Z) // and if this WP increasin on Z axis
        //                {
        //                    Debug.Log("Moving on Z axis");
        //                    //Calculate the distance between this and nex wp
        //                    float distanceOnZ = wayPoints[lastWPIndex].gameObject.transform.position.z - wayPoints[requestedWPIndex].gameObject.transform.position.z;
        //                    if (Mathf.Abs(distanceOnZ) < 0.5f) // if distance on Z axis between current and next waypoint, ignore it and select next one
        //                    {
        //                        Debug.Log("Too close to choose this WP");
        //                        Debug.Log(distanceOnZ);
        //                        i++;
        //                    }
        //                    else
        //                    {
        //                        wayPoints[requestedWPIndex -1].howManyOpponentOnThisWP++; // increase this WP opponent count so opponents move other WP and they all don't rush on a single WP
        //                        _opponent.lastWaypointIndex = wayPoints[requestedWPIndex - 1].waypointNumber;                            
        //                        return wayPoints[requestedWPIndex -1].gameObject.transform;                            
        //                    }

        //                }
        //                else if (wayPoints[requestedWPIndex].whichAxis == WaypointManager.WhichAxisYouAreMoving.X)
        //                {
        //                    Debug.Log("Moving on X axis");
        //                    float distanceOnX = wayPoints[lastWPIndex].gameObject.transform.position.x - wayPoints[requestedWPIndex].gameObject.transform.position.x;                        
        //                    if (Mathf.Abs(distanceOnX) < 0.5f)
        //                    {
        //                        Debug.Log("WP on X axis is too close to choose");
        //                        i++;
        //                    }
        //                    else
        //                    {
        //                        wayPoints[requestedWPIndex - 1].howManyOpponentOnThisWP++;  // increase this WP opponent count so opponents move other WP and they all don't rush on a single WP
        //                        _opponent.lastWaypointIndex = wayPoints[requestedWPIndex - 1].waypointNumber;
        //                        return wayPoints[requestedWPIndex -1].gameObject.transform;
        //                    }

        //                }
        //            } // if whichaxis
        //            else // if this and next WP axis is not same, return next
        //            {
        //                Debug.Log("Not on same axis");
        //                wayPoints[requestedWPIndex].howManyOpponentOnThisWP++;  // increase this WP opponent count so opponents move other WP and they all don't rush on a single WP
        //                _opponent.lastWaypointIndex = wayPoints[requestedWPIndex].waypointNumber;
        //                return wayPoints[requestedWPIndex].gameObject.transform;
        //            }

        //        } // if howmanyopponent

        //        else
        //        {
        //            Debug.Log("More than 2 opponent here");
        //            i++;
        //        }

        //    }

        //    return null;
        //} // GiveMeAWaypoint

        public void ActivateUpsideFences()
    {
        _mainCamera.GetComponent<SphereCollider>().enabled = false;
        for (int i = 0; i < _upsideFences.Length; i++)
        {
            _upsideFences[i].SetActive(true);
            //Debug.Log(upsideFences[i].name + " fence activated");            
        }

        StartCoroutine(EnableCameraCollider());
        
    } // ActivateUpsideFences

    IEnumerator EnableCameraCollider()
    {
        yield return new WaitForSeconds(1.33f);
        _mainCamera.GetComponent<SphereCollider>().enabled = true;
    } // EnableCameraCollider

    private void SetScreenSizeToDevice()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Application.targetFrameRate = 60;
        Screen.SetResolution(540, 960, true); // To increase FPS. Display quality does not change that much you feel the difference.
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        _mainCamera.aspect = screenWidth / screenHeight; // make screen size appropriate to device screen size.
#endif

#if UNITY_STANDALONE
        Application.targetFrameRate = 300;

        screenWidth = Screen.width;
        screenHeight = Screen.height;
        Screen.SetResolution((int)screenWidth, (int)screenHeight, true);

        _mainCamera.aspect = screenWidth / screenHeight;
#endif
    } // SetScreenSizeToDevice
   
} // Class
