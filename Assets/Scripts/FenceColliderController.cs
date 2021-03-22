using System.Collections;
using UnityEngine;

public class FenceColliderController : MonoBehaviour
{
    private CapsuleCollider _collider;


    private void OnEnable()
    {
        if (!_collider)
            _collider = GetComponent<CapsuleCollider>();
        _collider.enabled = false;
        Debug.Log("Collider disabled");
        StartCoroutine(EnableColliders());
    } // OnEnable

    private void Start()
    {
        _collider = GetComponent<CapsuleCollider>();
    } // Start    

    IEnumerator EnableColliders()
    {
        yield return new WaitForSeconds(1.5f);
        _collider.enabled = true;
        Debug.Log("Colliders enabled");
    } // EnableColliders
} // Class
