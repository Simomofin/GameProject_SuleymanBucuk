using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public int waypointNumber;
    public int howManyOpponentOnThisWP;
    public enum WhichAxisYouAreMoving
    {
        X,
        Y,
        Z
    }
    public WhichAxisYouAreMoving whichAxis;   

} // Class
