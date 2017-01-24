using UnityEngine;
using System.Collections;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private int m_AmountOfObjects = 10;

    [SerializeField]
    private GameObject m_AP_Shell;
    [SerializeField]
    private GameObject m_HE_Shell;
    [SerializeField]
    private GameObject m_Shell;

    private GameObject[] m_AP_ShellPool;
    private GameObject[] m_HE_ShellPool;
    private GameObject[] m_ShellPool;

    private void Awake()
    {
        m_AP_ShellPool = new GameObject[m_AmountOfObjects];
        m_HE_ShellPool = new GameObject[m_AmountOfObjects];
        m_ShellPool = new GameObject[m_AmountOfObjects];
    }

    private void Start()
    {
        for (int i = 0; i < m_AmountOfObjects; i++)
        {
            //Pooling AP_Shells.
            GameObject AP_Shell = Instantiate(m_AP_Shell, transform.position, transform.rotation) as GameObject;
            m_AP_ShellPool[i] = AP_Shell;
            AP_Shell.transform.parent = transform;
            AP_Shell.SetActive(false);

            //Pooling AP_Shells.
            GameObject HE_Shell = Instantiate(m_HE_Shell, transform.position, transform.rotation) as GameObject;
            m_HE_ShellPool[i] = AP_Shell;
            HE_Shell.transform.parent = transform;
            HE_Shell.SetActive(false);

            //Pooling AP_Shells.
            GameObject Shell = Instantiate(m_Shell, transform.position, transform.rotation) as GameObject;
            m_AP_ShellPool[i] = AP_Shell;
            Shell.transform.parent = transform;
            Shell.SetActive(false);
        }
    }

    public GameObject RequestPrefab(int prefabNumber)
    {
        GameObject prefab = null;
        switch (prefabNumber)
        {
            case 0:
                for (int i = 0; i < m_AP_ShellPool.Length; i++)
                {
                    if (!m_AP_ShellPool[i].activeInHierarchy)
                    {
                        m_AP_ShellPool[i].SetActive(true);
                        prefab = m_AP_ShellPool[i];
                        break;
                    }
                }
                break;
            case 1:
                for (int i = 0; i < m_HE_ShellPool.Length; i++)
                {
                    if (m_HE_ShellPool[i] != null && !m_HE_ShellPool[i].activeInHierarchy)
                    {
                        m_HE_ShellPool[i].SetActive(true);
                        prefab = m_HE_ShellPool[i];
                        m_HE_ShellPool[i] = null;
                        break;
                    }
                }
                break;
            case 2:
                for (int i = 0; i < m_ShellPool.Length; i++)
                {
                    if (m_ShellPool[i] != null && !m_ShellPool[i].activeInHierarchy)
                    {
                        m_ShellPool[i].SetActive(true);
                        prefab = m_ShellPool[i];
                        m_ShellPool[i] = null;
                        break;
                    }
                }
                break;
        }
        return prefab;
    }

    public void RevertObjectInPool(GameObject gameobject)
    {
        Debug.Log(gameobject.name);
        if (gameobject.GetComponent<AP_BulletBehaviour>() != null)
        {
            for (int i = 0; i < m_AP_ShellPool.Length; i++)
            {
                if (m_AP_ShellPool[i] == null)
                {
                    m_AP_ShellPool[i] = gameobject;
                    m_AP_ShellPool[i].SetActive(false);
                    m_AP_ShellPool[i].transform.position = transform.position;
                    m_AP_ShellPool[i].transform.eulerAngles = new Vector3(0,0,0);
                    break;
                }
            }
        }

        if (gameobject.GetComponent<HE_BulletBehaviour>() != null)
        {
            for (int i = 0; i < m_HE_ShellPool.Length; i++)
            {
                if (m_HE_ShellPool[i] == null)
                {
                    m_HE_ShellPool[i] = gameobject;
                    m_HE_ShellPool[i].SetActive(false);
                    m_HE_ShellPool[i].transform.position = transform.position;
                    break;
                }
            }
        }
    }
}
