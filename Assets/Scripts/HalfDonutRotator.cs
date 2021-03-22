using UnityEngine;

public class HalfDonutRotator : MonoBehaviour
{
    private float timer;
    private Animator _anim;

    public float rotationInterval;
    public enum Location
    {
        RightSide,
        LeftSide
    }
    public Location location = new Location();

    private void Start()
    {
        _anim = GetComponent<Animator>();
    } // Start

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if(timer >= rotationInterval)
        {
            if (location == Location.RightSide)
                _anim.SetTrigger(HelperClass.AnimationParameters.halfDonutTriggerRight);
            else if (location == Location.LeftSide)
                _anim.SetTrigger(HelperClass.AnimationParameters.halfDonutTriggerLeft);

            timer = 0;
        }

    } // FixedUpdate
    /// <summary>
    /// Resets half donut' s rotation after animation is completed,prevent it to roll back to 90 degrees at X rot.
    /// </summary>
    private void ResetPosition()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(74, 0, 0)), 1f);
    } // ResetPosition
} // Class
