using System;
using UnityEngine;

public class PaintTheWall : MonoBehaviour
{
    private Ray _ray;
    private RaycastHit _hit;
    private Vector3 _cursorPosition;
    private Camera _camera;
    private Vector3 brushPosition;
    private int _brushCount;
    private int _max_BrushCount = 150;
    private bool _saving;
    private Camera _mainCamera;

    public GameObject brushCursor;
    public GameObject brushPrefab;
    public float brushSize;
    public Color brushColor;
    public GameObject brushParent;
    public RenderTexture renderTexture;
    public Material baseMaterial;
    public Texture basetexture;
    public Camera textureCamera;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag(HelperClass.Tags.mainCamera).GetComponent<Camera>();               
        _mainCamera = GameObject.FindGameObjectWithTag(HelperClass.Tags.mainCamera).GetComponent<Camera>();
    } // Start

    private void FixedUpdate()
    {
        if (_saving)
            return;

        if (DidRayHitPrintableWall())
        {
            DrawBrushCursor();
            if (Input.GetMouseButton(0))
                PaintTheTexture();
        }
        else
            DisableCursor();
    } // FixedUpdate

    private void PaintTheTexture()
    {
        
         /* Commented out this metheod because it produce a pixelized movement
         * and it was not axpected to walk on this way. Also Texture2D.Apply() is heavy on performance when used in update or fixed.
         */
         /*
        Texture2D tex = (Texture2D)_hit.transform.gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture;
        if (tex == null)
        {
            Debug.Log("I am suffering here, in pain");
            Debug.Log(_hit.transform.gameObject.name);
            return;
        }
        tex.SetPixel((int)(uvCoordinates.x * tex.width), (int)(uvCoordinates.y * tex.height), brushColor);        
        tex.Apply();
        */
       

        GameObject newBrush = Instantiate(brushPrefab, _hit.point, Quaternion.identity) as GameObject;
        newBrush.transform.SetParent(brushParent.transform);
        newBrush.transform.localScale = Vector3.one * brushSize;
        newBrush.transform.position = brushPosition;        
        //newBrush.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
        newBrush.GetComponent<SpriteRenderer>().color = brushColor;
        brushColor.a = brushSize * 1.5f;
        _brushCount++;
        if(_brushCount > _max_BrushCount)
        {
            brushCursor.SetActive(false);
            _saving = true;
            Invoke("SaveTexture", 0.05f);
        }

    } // PaintTheTexture

    private void DrawBrushCursor()
    {
        /*PSUEDO
         * Set brush's position according to ray's hit point coordinates
         * on texture
         */
        if (!brushCursor.activeInHierarchy)
            EnableCursor();

        brushCursor.transform.position = brushPosition;

    } // DrawBrushSprite

    private bool DidRayHitPrintableWall()
    {
        _cursorPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        _ray = _camera.ScreenPointToRay(_cursorPosition);
        if (Physics.Raycast(_ray, out _hit, 30))
        {
            if (_hit.collider.gameObject.CompareTag(HelperClass.Tags.paintableWalls))
            {                             
                //Debug.Log("I hit the paintable wall");
                brushPosition = new Vector3(_hit.point.x, _hit.point.y, _hit.point.z - 0.5f);
                return true;
            }
            else
                return false;
        }
        else
            return false;
    } // DidRayHitPrintableWall

    private void SaveTexture()
    {
        _brushCount = 0;
        RenderTexture.active = renderTexture;

        Texture2D _texture = new Texture2D(renderTexture.width, renderTexture.height);
        _texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0, false);
        _texture.Apply();                        
        baseMaterial.mainTexture = _texture;
        RenderTexture.active = null; // Yol ver de gidek

        foreach (Transform child in brushParent.transform) // TODO: Also foreach calls Garbage Collection, use for loop instead.
        {
            Destroy(child.gameObject); // TODO: Destroy is not the way we walk on, for performance issues, use some kind of pooling system here.
        }        
        Invoke("EnableCursor", 0.1f);
        _saving = false;
    }

    
    private void EnableCursor()
    {
        
        brushCursor.SetActive(true);
    } // EnableCursor
    private void DisableCursor()
    {
        brushCursor.SetActive(false);
    } // DisableCursor

    private void OnDisable()
    {
        baseMaterial.mainTexture = basetexture;
    } // OnDisable

} // Class


