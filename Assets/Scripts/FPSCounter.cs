using System.Text;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof (Text))]
public class FPSCounter : MonoBehaviour
{
    const float fpsMeasurePeriod = 0.5f;
    private int m_FpsAccumulator = 0;
    private float m_FpsNextPeriod = 0;
    private int m_CurrentFps;
    private Text m_Text;
    private float timer;

    private void Start()
    {
        m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
        m_Text = GetComponent<Text>();        
    } // Start

    private void Update()
    {
        // measure average frames per second
        m_FpsAccumulator++;
        timer++;
        if (Time.realtimeSinceStartup > m_FpsNextPeriod)
        {
            m_CurrentFps = (int) (m_FpsAccumulator/fpsMeasurePeriod);
            m_FpsAccumulator = 0;
            m_FpsNextPeriod += fpsMeasurePeriod;                
            m_Text.text = "FPS: " + m_CurrentFps;                           
        }
    } // Update

}// Class

