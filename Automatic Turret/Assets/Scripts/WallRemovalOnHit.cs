using UnityEngine;
using System.Collections;

public class WallRemovalOnHit : MonoBehaviour
{

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<WB_BulletBehaviour>() != null)
        {
            Destroy(gameObject);
        }
    }
}
