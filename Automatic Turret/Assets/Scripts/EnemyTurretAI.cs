using UnityEngine;
using System.Collections;

public delegate void EnemyDiedDelegate();

public class EnemyTurretAI : MonoBehaviour
{
    private event EnemyDiedDelegate d_Delegates;

    public EnemyDiedDelegate m_FollowDelagate
    {
        set { d_Delegates += value; }
        get { return d_Delegates; }
    }

    private float health = 100;

    private void Update()
    {
        if (health <= 0 && d_Delegates != null)
        {
            d_Delegates.Invoke();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<AP_BulletBehaviour>() != null)
        {
            health -= 50;

        }
    }
}
