using UnityEngine;
using System.Collections;

public delegate void BulletPositionDelegate(Vector3 impactPosition, GameObject objectHit);

public class AP_BulletBehaviour : MonoBehaviour
{
    private event BulletPositionDelegate d_Delegates;
    public BulletPositionDelegate m_FollowDelagate
    {
        set { d_Delegates += value; }
        get { return d_Delegates; }
    }

    private float m_AliveTime = 10f;

    private Rigidbody m_Rigidbody;

    private bool m_Collided = false;

    private void Awake()
    {

        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.mass = 10;
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

    }

    private void Start()
    {
        m_AliveTime = 10.0f;
        m_Rigidbody.AddForce(transform.forward * m_Rigidbody.mass * 400, ForceMode.Impulse);
        transform.eulerAngles += new Vector3(90, 0, 0);

    }


    void FixedUpdate()
    {
        if (m_AliveTime <= 0)
        {
            Destroy(gameObject);
        }

        m_AliveTime -= Time.deltaTime;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (d_Delegates != null && other.gameObject.name != gameObject.name && m_Collided == false)
        {
            m_Collided = true;
            d_Delegates.Invoke(gameObject.transform.position, other.gameObject);
            Destroy(gameObject);
        }
    }
}
