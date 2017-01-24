using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TurretControl : MonoBehaviour
{
    [SerializeField]
    private float m_LeftBarrelDampTime = 2;
    [SerializeField]
    private float m_RightBarrelDampTime = 2;

    [SerializeField]
    private float m_LeftBarrelReloadTime = 2;
    [SerializeField]
    private float m_RightBarrelReloadTime = 2;


    private Transform m_HorizontalTurnPoint;
    private Transform m_VerticalTurnPoint;

    private Transform m_LeftBarrel;
    private Transform m_RightBarrel;

    private Transform m_LeftyShellSpawn;
    private Transform m_RightShellSpawn;

    private Transform m_FirePlaceLeft;
    private Transform m_FirePlaceRight;

    [SerializeField]
    private GameObject m_Explosion;
    [SerializeField]
    private GameObject m_Bullet;
    [SerializeField]
    private GameObject m_Shell;
    [SerializeField]
    private GameObject m_AfterShotEffect;
    [SerializeField]
    private GameObject m_Camera;

    private bool m_LeftBarrelReloading = false;
    private bool m_RightBarrelReloading = false;

    private void Awake()
    {
        m_HorizontalTurnPoint = transform.FindChild("HorizontalTurnPoint");
        m_VerticalTurnPoint = transform.FindChild("HorizontalTurnPoint/VerticalTurnPoint");

        m_LeftBarrel = transform.FindChild("HorizontalTurnPoint/VerticalTurnPoint/TotalBarrelLeft/BarrelLeft/BarrelEndLeft");
        m_RightBarrel = transform.FindChild("HorizontalTurnPoint/VerticalTurnPoint/TotalBarrelRight/BarrelRight/BarrelEndRight");

        m_LeftyShellSpawn = transform.FindChild("HorizontalTurnPoint/VerticalTurnPoint/TotalBarrelLeft/BarrelLeft/ShellSpawnLeft");
        m_RightShellSpawn = transform.FindChild("HorizontalTurnPoint/VerticalTurnPoint/TotalBarrelRight/BarrelRight/ShellSpawnRight");

        m_FirePlaceLeft = transform.FindChild("HorizontalTurnPoint/VerticalTurnPoint/TotalBarrelLeft/BulletSpawn");
        m_FirePlaceRight = transform.FindChild("HorizontalTurnPoint/VerticalTurnPoint/TotalBarrelRight/BulletSpawn");
    }
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Scene1");
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }

        m_HorizontalTurnPoint.gameObject.transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal") * Time.deltaTime * 18, 0));
        m_VerticalTurnPoint.gameObject.transform.Rotate(new Vector3(Input.GetAxis("Vertical") * Time.deltaTime * 30, 0, 0));

        if (m_LeftBarrelReloading == true)
        {
            m_LeftBarrel.transform.localPosition = new Vector3(m_LeftBarrel.transform.localPosition.x, m_LeftBarrel.transform.localPosition.y, Mathf.PingPong(Time.time * 20f, -6) + 17);
        }

        if (m_RightBarrelReloading == true)
        {
            m_RightBarrel.transform.localPosition = new Vector3(m_RightBarrel.transform.localPosition.x, m_RightBarrel.transform.localPosition.y, Mathf.PingPong(Time.time * 20f, -6) + 17);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            m_HorizontalTurnPoint.transform.Translate(new Vector3(0, 0.02f, 0));
        }

        if (Input.GetKey(KeyCode.C))
        {
            m_HorizontalTurnPoint.transform.Translate(new Vector3(0, -0.02f, 0));
        }

        if (Input.GetButton("Fire1") && m_LeftBarrelReloading == false)
        {
            m_Camera.GetComponent<CameraShake>().Shake(0.07f, 0.002f);

            Instantiate(m_Explosion, m_FirePlaceLeft.transform.position, m_FirePlaceLeft.transform.rotation);
            GameObject AfterShotEffect = Instantiate(m_AfterShotEffect, m_FirePlaceLeft.transform.position, m_FirePlaceLeft.transform.rotation) as GameObject;
            AfterShotEffect.transform.parent = m_FirePlaceLeft;
            GameObject bullet = Instantiate(m_Bullet, m_FirePlaceLeft.transform.position, m_VerticalTurnPoint.transform.rotation) as GameObject;
            GameObject shell = Instantiate(m_Shell, m_LeftyShellSpawn.transform.position, m_VerticalTurnPoint.transform.rotation) as GameObject;


            m_LeftBarrelReloading = true;
            m_LeftBarrelDampTime = 0.4f;
            m_LeftBarrelReloadTime = 1.5f;
            
        }

        if (Input.GetButton("Fire2") && m_RightBarrelReloading == false)
        {
            m_Camera.GetComponent<CameraShake>().Shake(0.07f, 0.002f);

            Instantiate(m_Explosion, m_FirePlaceRight.transform.position, m_FirePlaceRight.transform.rotation);
            GameObject AfterShotEffect = Instantiate(m_AfterShotEffect, m_FirePlaceRight.transform.position, m_FirePlaceRight.transform.rotation) as GameObject;
            AfterShotEffect.transform.parent = m_FirePlaceRight;
            GameObject bullet = Instantiate(m_Bullet, m_FirePlaceRight.transform.position, m_FirePlaceRight.transform.rotation) as GameObject;
            GameObject shell = Instantiate(m_Shell, m_RightShellSpawn.transform.position, m_VerticalTurnPoint.transform.rotation) as GameObject;


            m_RightBarrelReloading = true;
            m_RightBarrelDampTime = 0.4f;
            m_RightBarrelReloadTime = 1.5f;
        }
        m_RightBarrelDampTime -= Time.deltaTime;
        m_LeftBarrelDampTime -= Time.deltaTime;

        m_RightBarrelReloadTime -= Time.deltaTime;
        m_LeftBarrelReloadTime -= Time.deltaTime;

        if (m_LeftBarrelReloadTime <= 0 && m_LeftBarrelReloading == true)
        {
            m_LeftBarrelReloading = false;
        }

        if (m_RightBarrelReloadTime <= 0 && m_RightBarrelReloading == true)
        {
            m_RightBarrelReloading = false;
        }

        if (m_LeftBarrelDampTime <= 0)
        {
            m_LeftBarrel.transform.localPosition = new Vector3(9.536743e-08f, -1.07688e-07f, 13);
        }

        if (m_RightBarrelDampTime <= 0)
        { 
            m_RightBarrel.transform.localPosition = new Vector3(1.430512e-07f, -1.07688e-07f, 13);
        }
    }
}
