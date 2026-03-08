using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayFPS : MonoBehaviour
{
    public TextMeshProUGUI text; 
    int m_frameCounter = 0;
    float m_timeCounter = 0.0f;
    public float m_refreshTime = 0.5f;

    void Update()
    {
#if UNITY_EDITOR
        if (m_timeCounter < m_refreshTime)
        {
            m_timeCounter += Time.deltaTime;
            m_frameCounter++;
        }
        else
        {
            text.text = "" + Mathf.Round((float)m_frameCounter / m_timeCounter);
            m_frameCounter = 0;
            m_timeCounter = 0.0f;
        }
#endif
    }
}
