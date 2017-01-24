using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{

    private Vector3 m_BeginPosition;
    private Quaternion m_BeginRotation;

    private Vector3 originPosition;
    private Quaternion originRotation;

    [SerializeField]
    private float shake_decay;
    [SerializeField]
    private float shake_intensity;

    private void Awake()
    {
        m_BeginPosition = transform.position;
        m_BeginRotation = transform.rotation;
    }

    private void Update()
    {
        //Testing
        /*if (Input.GetButton("Fire1"))
        {
            Shake(0,0);
        }*/

        if (shake_intensity > 0)
        {
            transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
            transform.rotation = new Quaternion(
            originRotation.x + Random.Range(-shake_intensity, shake_intensity) * .2f,
            originRotation.y + Random.Range(-shake_intensity, shake_intensity) * .2f,
            originRotation.z + Random.Range(-shake_intensity, shake_intensity) * .2f,
            originRotation.w + Random.Range(-shake_intensity, shake_intensity) * .2f);
            shake_intensity -= shake_decay;
        }
        else
        {
            
            transform.position = m_BeginPosition;
            transform.rotation = m_BeginRotation;
        }
    }

    public void Shake(float intensity, float decay)
    {
        originPosition = transform.position;
        originRotation = transform.rotation;

        //Recommended = intensity = 0.3f decay = 0.002f
        shake_intensity = intensity;
        shake_decay = decay;
    }
}
