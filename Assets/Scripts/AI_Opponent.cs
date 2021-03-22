using System;
using UnityEngine;

public class AI_Opponent : MonoBehaviour
{
    public GameObject[] wayPointsLeft1;
    public GameObject[] wayPointsLeft2;
    public GameObject[] wayPointsRight1;
    public GameObject[] wayPointsRight2;
    public GameObject nextWayPoint;
    public int _WPIndex;

    private OpponentController _opponentController;
    private GameObject _currentWaypoint;
    
    

    public enum HighWays
    {
        Right1,
        Right2,
        Left1,
        Left2
    }
    public HighWays whichWayToFollow;


    void Start()
    {
        _currentWaypoint = null;
        nextWayPoint = null;
        _opponentController = this.GetComponent<OpponentController>();
        /* Below lines are commented because random wp selection is made in Gamemanager.cs/SetOpponentWayToFollow.
         * "The reason to do this" is, preventing players to choose the same way and follow themselves like a train.
         * Gamemanager.cs/SetOpponentWayToFollow sets 2 opponents for 2 WP and 3 opponent for other 2 WP.(10 opponent)
        int rand1 = UnityEngine.Random.Range(0, 4); // To select a random way to follow
        switch (rand1)
        {
            case 0:
                this.whichWayToFollow = HighWays.Right1;
                break;

            case 1:
                this.whichWayToFollow = HighWays.Right2;
                break;

            case 2:
                this.whichWayToFollow = HighWays.Left1;
                break;

            case 3:
                this.whichWayToFollow = HighWays.Left2;
                break;

            default:
                break;
        } // switch Way Selection
        */

    } // Start

    public GameObject NextWayPoint()
    {
        if(this.whichWayToFollow == HighWays.Right1) 
        {
            if (nextWayPoint == null) // at start
            {
                nextWayPoint = wayPointsRight1[0];
                _WPIndex = 0;
                return nextWayPoint;
            }
            else // if nextwp is not null
            {
                _WPIndex++;
                nextWayPoint = wayPointsRight1[_WPIndex];
                _currentWaypoint = wayPointsRight1[_WPIndex - 1];
                return nextWayPoint;
            }

        }// Follow right1 way
        else if(this.whichWayToFollow == HighWays.Right2) 
        {
            if (nextWayPoint == null) // at start
            {
                nextWayPoint = wayPointsRight2[0];
                _WPIndex = 0;
                return nextWayPoint;
            }
            else
            {
                _WPIndex++;
                nextWayPoint = wayPointsRight2[_WPIndex];
                _currentWaypoint = wayPointsRight2[_WPIndex - 1];
                return nextWayPoint;
            }
        } // Follow right2 way
        else if(this.whichWayToFollow == HighWays.Left1)
        {
            if (nextWayPoint == null) // at start
            {
                nextWayPoint = wayPointsLeft1[0];
                _WPIndex = 0;
                return nextWayPoint;
            }
            else
            {
                _WPIndex++;
                nextWayPoint = wayPointsLeft1[_WPIndex];
                _currentWaypoint = wayPointsLeft1[_WPIndex - 1];
                return nextWayPoint;
            }            
        } // Follow left1 way
        else if(this.whichWayToFollow == HighWays.Left2)
        {
            if (nextWayPoint == null) // at start
            {
                nextWayPoint = wayPointsLeft2[0];
                _WPIndex = 0;
                return nextWayPoint;
            }
            else
            {
                _WPIndex++;
                nextWayPoint = wayPointsLeft2[_WPIndex];
                _currentWaypoint = wayPointsLeft2[_WPIndex - 1];
                return nextWayPoint;
            }
        } // Follow left2 way
        else
        {
            Debug.LogError("Something wrong here, choosing way to follow");
            return null;
        }

    } // NextWayPoint
} // Class
