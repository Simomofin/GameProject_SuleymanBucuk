namespace PaintMaster
{
    using System.Collections;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.UI;

    /// <summary>
    /// Notes: Make your baseTexture width and height 128 * 128 so paint process will be reduced according to pixel size
    /// Lower than 128 * 128 texture size makes feel like pixelized.
    /// </summary>
    public class TexturePaint : MonoBehaviour
    {        
        public Texture baseTexture;                  // used to deterimne the dimensions of the runtime texture
        public Material meshMaterial;                 // used to bind the runtime texture as the albedo of the mesh
        public GameObject meshGameobject;
        public Shader UVShader;                     // the shader usedto draw in the texture of the mesh
        public Mesh meshToDraw;
        public Shader ilsandMarkerShader;
        public Shader fixIlsandEdgesShader;        
        public static Vector3 mouseWorldPosition;
        public Texture2D paintedTextureForHolo;
        public Color paintColor = Color.red;
        public Slider paintSlider;
        public TextMeshProUGUI paintCompletedtext;
        public GameObject paintCompletedPanel;

        // --------------------------------

        private Camera mainCamera;
        private int clearTexture;
        private RenderTexture markedIlsandes;
        private CommandBuffer cb_markingIlsdands;
        private int numberOfFrames;
        private RaycastHit hit;
        private Ray ray;
        private PaintableTexture albedo;        
        private float paintedPercentageValue;
        public bool isPaintingFinished = false;
        public bool isPaintingStarted = false;
        
        void Start()
        {
            paintSlider.gameObject.SetActive(false);
            paintCompletedtext.gameObject.SetActive(false);
            
            Shader.SetGlobalColor("_BrushColor", Color.red);
            Shader.SetGlobalFloat("_BrushOpacity", 1f);
            Shader.SetGlobalFloat("_BrushSize", 0.85f); // Best fit
            Shader.SetGlobalFloat("_BrushHardness", 0.55f); // mostly best fit for smoothness
            


            mainCamera = GameObject.FindGameObjectWithTag(HelperClass.Tags.mainCamera).GetComponent<Camera>();
            if(mainCamera == null)
                Debug.LogError("Couldn' t find main camera");

            mainCamera.depthTextureMode = DepthTextureMode.Depth; // On mobile platforms, shadow cascades are forced on to be enabled depthTextureMode to .Depth, if you don't set this up,
                                                                  // painting on shader won' t work on mobile platforms, but on pc and webgl.
                                                                  //Also, you need to check #if UNITY_UV_STARTS_AT_TOP on shader codes, which is TexturePaint.shader on this case, on 57 line check this statement
                                                                  //before you manipulate uvRemapped on y axis.


            // Texture and Mat initalization ---------------------------------------------
            markedIlsandes = new RenderTexture(baseTexture.width, baseTexture.height, 0, RenderTextureFormat.R8);

            albedo = new PaintableTexture(Color.white, baseTexture.width, baseTexture.height, "_MainTex"
                , UVShader, meshToDraw, fixIlsandEdgesShader, markedIlsandes);
                
            meshMaterial.SetTexture(albedo.id, albedo.runTimeTexture);            

            cb_markingIlsdands = new CommandBuffer();
            cb_markingIlsdands.name = "markingIlsnads";


            cb_markingIlsdands.SetRenderTarget(markedIlsandes);
            Material mIlsandMarker = new Material(ilsandMarkerShader);
            cb_markingIlsdands.DrawMesh(meshToDraw, Matrix4x4.identity, mIlsandMarker);
            mainCamera.AddCommandBuffer(CameraEvent.AfterDepthTexture, cb_markingIlsdands);

            albedo.SetActiveTexture(mainCamera);

            paintedTextureForHolo = new Texture2D(albedo.paintedTexture.width, albedo.paintedTexture.height);

        }  // Start    

        private void FixedUpdate()
        {
            if (isPaintingFinished)
                return;

            if (numberOfFrames > 2) 
                mainCamera.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, cb_markingIlsdands);

            numberOfFrames++;

            albedo.UpdateShaderParameters(meshGameobject.transform.localToWorldMatrix);            
            
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Vector4 mwp = Vector3.positiveInfinity;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag(HelperClass.Tags.paintableWalls))
                {
                    mwp = hit.point;
                    mwp.w = Input.GetMouseButton(0) ? 1 : 0;
                    //Debug.Log(mwp);

                    if (!isPaintingStarted)
                        isPaintingStarted = true;
                }                
            }
                       
            mouseWorldPosition = mwp;
            Shader.SetGlobalVector("_Mouse", mwp);

            
                        
        } // FixedUpdate

        private void LateUpdate()
        {
            if (isPaintingStarted)
                StartCoroutine(EnablePaintSlider(true));
            if (isPaintingFinished)
                return;            

            paintedPercentageValue = TextureColorFillCalculator.CalculateFill(GetObjectTexture().GetPixels(), paintColor, 0);           
            paintSlider.value = paintedPercentageValue;
            paintCompletedtext.text = "PAINT COMPLETED: %" + (int)(paintedPercentageValue * 100);
            if (paintedPercentageValue >= 0.995f)
            {
                isPaintingFinished = true;
                paintCompletedPanel.SetActive(true);
                paintCompletedtext.text = "PAINT COMPLETED: %100";                
                Debug.Log("Painting completed");
                //TODO: Set a "Paint Finished" menu here
            }
        } // LateUpdate

        private Texture2D GetObjectTexture()
        {            
            Texture2D texture2D = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.RGBA32, false);

            RenderTexture renderTexture = new RenderTexture(albedo.paintedTexture.width, albedo.paintedTexture.height, 32);
            Graphics.Blit(albedo.paintedTexture, renderTexture);

            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            return texture2D;
        }      

        IEnumerator EnablePaintSlider(bool v)
        {            
            //Debug.Log("Enabling slider");
            yield return new WaitForSeconds(0.5f);
            paintSlider.gameObject.SetActive(v);
            paintCompletedtext.gameObject.SetActive(v);
        } // EnablePaintSlider
    } // TexturePaint Class



    [System.Serializable]
    public class PaintableTexture
    {
        public string id;
        public RenderTexture runTimeTexture;
        public RenderTexture paintedTexture;

        public CommandBuffer cb;

        private Material mPaintInUV;
        private Material mFixedEdges;
        private RenderTexture fixedIlsands;

        public PaintableTexture(Color clearColor, int width, int height, string id,
            Shader sPaintInUV, Mesh mToDraw, Shader fixIlsandEdgesShader, RenderTexture markedIlsandes)
        {
            this.id = id;

            runTimeTexture = new RenderTexture(width, height, 0)
            {
                anisoLevel = 0,
                useMipMap = false,
                filterMode = FilterMode.Bilinear
            };

            paintedTexture = new RenderTexture(width, height, 0)
            {
                anisoLevel = 0,
                useMipMap = false,
                filterMode = FilterMode.Bilinear
            };


            fixedIlsands = new RenderTexture(paintedTexture.descriptor);

            Graphics.SetRenderTarget(runTimeTexture);
            GL.Clear(false, true, clearColor);
            Graphics.SetRenderTarget(paintedTexture);
            GL.Clear(false, true, clearColor);


            mPaintInUV = new Material(sPaintInUV);
            if (!mPaintInUV.SetPass(0)) Debug.LogError("Invalid Shader Pass: ");
            mPaintInUV.SetTexture("_MainTex", paintedTexture);

            mFixedEdges = new Material(fixIlsandEdgesShader);
            mFixedEdges.SetTexture("_IlsandMap", markedIlsandes);
            mFixedEdges.SetTexture("_MainTex", paintedTexture);

            // ----------------------------------------------

            cb = new CommandBuffer();
            cb.name = "TexturePainting" + id;


            cb.SetRenderTarget(runTimeTexture);
            cb.DrawMesh(mToDraw, Matrix4x4.identity, mPaintInUV);

            cb.Blit(runTimeTexture, fixedIlsands, mFixedEdges);
            cb.Blit(fixedIlsands, runTimeTexture);
            cb.Blit(runTimeTexture, paintedTexture);

        }

        public void SetActiveTexture(Camera mainC)
        {
            mainC.AddCommandBuffer(CameraEvent.AfterDepthTexture, cb);
        }

        public void SetInactiveTexture(Camera mainC)
        {
            mainC.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, cb);
        }

        public void UpdateShaderParameters(Matrix4x4 localToWorld)
        {
            mPaintInUV.SetMatrix("mesh_Object2World", localToWorld); // Must be updated every time the mesh moves, and also at start
            
        }        

    } // PaintableTexture Class       

} // namespace
