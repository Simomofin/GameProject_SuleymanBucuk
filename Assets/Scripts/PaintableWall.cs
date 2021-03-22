using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PaintableWall : MonoBehaviour
{
    private GameObject _camera;

    public TextMeshProUGUI paintTheWallText;

    private void Awake()
    {
        _camera = GameObject.FindGameObjectWithTag(HelperClass.Tags.mainCamera);        
    } // Awake


    public void SetWall()
    {        
        //transform.rotation = Quaternion.Euler(new Vector3(-7.15f, 0, 0));
        //StartCoroutine(MoveWall());
        StartCoroutine(EnablePaintText());
        DisableObjectsForPainting();
    } // SetPosition

    private void DisableObjectsForPainting()
    {
        GameObject[] opponents = GameObject.FindGameObjectsWithTag(HelperClass.Tags.opponent);
        GameObject[] upsideFences = GameObject.FindGameObjectsWithTag(HelperClass.Tags.upsideFence);
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag(HelperClass.Tags.obstacleTag);
        
        for (int i = 0; i < opponents.Length; i++) // disable opponents to prevent block paintable wall 
        {
            opponents[i].SetActive(false);
        }
        for (int i = 0; i < upsideFences.Length; i++)
        {
            upsideFences[i].SetActive(false);
        }
        for (int i = 0; i < obstacles.Length; i++)
        {
            obstacles[i].SetActive(false);
        }
    }

    //IEnumerator MoveWall()
    //{        
    //    while (transform.position.x > 3.55f)
    //    {
    //        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, 3.5f, transform.position.z), 3 * Time.fixedDeltaTime);
    //        yield return new WaitForFixedUpdate();
    //    }
    //} // MoveWall

    IEnumerator EnablePaintText()
    {
        yield return new WaitForSeconds(0.5f);
        paintTheWallText.gameObject.SetActive(true);
    } // EnablePaintText
} // Class
