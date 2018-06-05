using UnityEngine;
using System.Collections;

public class ChangeShadingInfluence : MonoBehaviour
{

    public float duration = 10.0f;
    public float waitDuration = 3.0f;
    private float m_PassedTime;
    private CS_Cloud m_Cloud;

    void OnEnable()
    {
        m_Cloud = GetComponent<CS_Cloud>();

        if (m_Cloud != null)
        {
            StartCoroutine(Fading());
        }
        else
        {
            Debug.LogError("ChangeShadingInfluence script is not in a GameObject that contains a CS_Cloud!");
        }
    }

    IEnumerator Fading()
    {
        yield return 0;

        while (true)
        {

            // Fade out
            m_PassedTime = 0.0f;
            while (m_PassedTime < duration)
            {
                m_Cloud.ShadingGroupInfluence = 1.0f - (m_PassedTime / duration);
                m_PassedTime = m_PassedTime + Time.deltaTime;
                yield return 0;
            }
            m_Cloud.ShadingGroupInfluence = 0.0f;
            yield return (new WaitForSeconds(waitDuration));

            // Fade in
            m_PassedTime = 0.0f;
            while (m_PassedTime < duration)
            {
                m_Cloud.ShadingGroupInfluence = m_PassedTime / duration;
                m_PassedTime = m_PassedTime + Time.deltaTime;
                yield return 0;
            }
            m_Cloud.ShadingGroupInfluence = 1.0f;
            yield return (new WaitForSeconds(waitDuration));
        }
    }
}