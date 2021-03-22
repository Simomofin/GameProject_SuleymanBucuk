using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLineController : MonoBehaviour
{
    public delegate void WhoWonTheGame();
    public static event WhoWonTheGame OnPlayerWon;
    public static event WhoWonTheGame OnOpponentWon;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag(HelperClass.Tags.playerTag))
        {
            //player won the game
            OnPlayerWon?.Invoke(); // fire is calling you to hell
        }
        if(other.gameObject.CompareTag(HelperClass.Tags.opponent))
        {
            //opponent won the game
            OnOpponentWon?.Invoke();
        }
    } // OnTriggerEnter

} // Class
