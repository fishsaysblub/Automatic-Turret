using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Use these following commands to make it clear: FIXME: comment, WORKINGON: comment, TODO: comment.

namespace AI_TurretController
{

    public class AI_TurretController : MonoBehaviour
    {
        private State m_CurrentState;
        public State m_GetSetCurrentState
        {
            get { return m_CurrentState; }
            set { m_CurrentState = value; }
        }

        private AI_TurretController m_AI_TurretController;

        [SerializeField]
        private GameObject m_AP_Shell;
        public GameObject m_Get_AP_Shell
        {
            get { return m_AP_Shell; }
        }

        [SerializeField]
        private GameObject m_HE_Shell;
        public GameObject m_Get_HE_Shell
        {
            get { return m_HE_Shell; }
        }

        [SerializeField]
        private GameObject m_WB_Shell;
        public GameObject m_Get_WB_Shell
        {
            get { return m_WB_Shell; }
        }

        private void Awake()
        {

            m_AI_TurretController = this;

            m_CurrentState = new Moving(m_AI_TurretController);
        }

        private void Update()
        {
            m_CurrentState.Update();
        }
    }

    public interface State
    {
        void Update();
    }

    public class Moving : State
    {
        AI_TurretController m_AI_Controller;

        private Vector3 m_targetedMovementSpot = Vector3.zero;

        private GameObject m_CurrentMovingObject;

        private NavMeshAgent m_NavmeshAgent;

        private List<GameObject> m_MovingSpots = new List<GameObject>();

        public Moving(AI_TurretController controller)
        {

            m_AI_Controller = controller;

            m_MovingSpots.AddRange(GameObject.FindGameObjectsWithTag("MovementSpot"));

            m_NavmeshAgent = controller.gameObject.GetComponent<NavMeshAgent>();

            //Selecting closest hiding spot.
            for (int i = 0; i < m_MovingSpots.Count; i++)
            {

                if (m_targetedMovementSpot != Vector3.zero &&
                    Vector3.Distance(controller.gameObject.transform.position, m_MovingSpots[i].transform.position) <=
                    Vector3.Distance(controller.gameObject.transform.position, m_targetedMovementSpot))
                {
                    m_targetedMovementSpot = m_MovingSpots[i].transform.position;
                    m_CurrentMovingObject = m_MovingSpots[i];
                }

                if (m_targetedMovementSpot == Vector3.zero)
                {
                    m_targetedMovementSpot = m_MovingSpots[i].transform.position;
                    m_CurrentMovingObject = m_MovingSpots[i];

                }
            }

            m_NavmeshAgent.SetDestination(m_targetedMovementSpot);
        }
        public void Update()
        {
            if (m_NavmeshAgent.pathPending == false && m_NavmeshAgent.hasPath == false)
                 
            {
                GameObject.Destroy(m_CurrentMovingObject);
                m_AI_Controller.m_GetSetCurrentState = new FindTarget(m_AI_Controller);
            }
        }

      
    }

    public class AimAndCalculateTrajectory : State
    {
        private AI_TurretController m_AI_TurretController;

        private Vector3 m_TargetPosition;

        private GameObject m_Target;

        private Transform m_TurnPoint;

        private float m_RetryTimer = 10.0f;

        

        public AimAndCalculateTrajectory(AI_TurretController controller, GameObject target, Vector3 newAim)
        {
            m_AI_TurretController = controller;
            m_Target = target;

            m_TurnPoint = m_AI_TurretController.gameObject.transform.FindChild("turret/HorizontalTurnPoint/VerticalTurnPoint");

            m_TargetPosition = target.transform.position;

            if (newAim != Vector3.zero)
            {

                m_TargetPosition = new Vector3(target.transform.position.x, target.transform.position.y + (target.transform.position.y - newAim.y), target.transform.position.z);
            }
        }

        public void Update()
        {
            m_TurnPoint.transform.rotation = Quaternion.Slerp(m_TurnPoint.transform.rotation, 
                                                            Quaternion.LookRotation(-(m_TurnPoint.transform.position - 
                                                            m_TargetPosition).normalized), 1 * Time.deltaTime);


            if (m_RetryTimer <= 0)
            {
                m_TurnPoint.transform.rotation = Quaternion.Euler(m_TurnPoint.transform.rotation.x, m_TurnPoint.transform.rotation.y + 10, m_TurnPoint.transform.rotation.z);
                m_RetryTimer = 10.0f;
            }

            

            if (m_TurnPoint.transform.rotation == Quaternion.LookRotation(-(m_TurnPoint.transform.position - 
                                                                                    m_TargetPosition).normalized))
            {

                m_AI_TurretController.m_GetSetCurrentState = new SelectAmmoAndFire(m_AI_TurretController, m_Target);
            }

            m_RetryTimer -= Time.deltaTime;
            
        }
    }

    public class SelectAmmoAndFire : State
    {
        private AI_TurretController m_AI_TurretController;

        private GameObject m_Bullet;

        private GameObject m_Target;

        private Transform m_FirePlaceLeft;
        private Transform m_FirePlaceRight;

        private Transform m_VerticalTurnPoint;

        private float m_LeftReload = Random.Range(1f, 2f);
        private float m_RightReload = Random.Range(1f, 2f);

        private bool m_ObservedABullet = false;
        public SelectAmmoAndFire(AI_TurretController controller, GameObject target)
        {
            m_AI_TurretController = controller;
            m_Target = target;

            m_FirePlaceLeft = m_AI_TurretController.gameObject.transform.FindChild("turret/HorizontalTurnPoint/VerticalTurnPoint/TotalBarrelLeft/BulletSpawn");
            m_FirePlaceRight = m_AI_TurretController.gameObject.transform.FindChild("turret/HorizontalTurnPoint/VerticalTurnPoint/TotalBarrelRight/BulletSpawn");

            m_VerticalTurnPoint = m_AI_TurretController.gameObject.transform.FindChild("turret/HorizontalTurnPoint/VerticalTurnPoint");

            SelectAmmunitionForTarget();

        }

        private void CreateBulletLeftBarrel()
        {

            //Bullet left barrel.
            GameObject FiredShellLeft = GameObject.Instantiate(m_Bullet, m_FirePlaceLeft.transform.position,
                                                            m_VerticalTurnPoint.transform.rotation) as GameObject;

            //Follow bullet with observer pattern.
            SubscribeToBullet(FiredShellLeft);

            m_LeftReload = Random.Range(3.5f, 6.0f);

            //Get bullet that is needed for target.
            SelectAmmunitionForTarget();

        }

        private void CreateBulletRightBarrel()
        {
            //Bullet right barrel.
            GameObject FiredShellRight = GameObject.Instantiate(m_Bullet, m_FirePlaceRight.transform.position,
                                                m_VerticalTurnPoint.transform.rotation) as GameObject;

            //Follow bullet with observer pattern.
            SubscribeToBullet(FiredShellRight);

            m_RightReload = Random.Range(3.5f, 6.0f);

            //Get bullet that is needed for target.
            SelectAmmunitionForTarget();
        }


        //Checks the script and subscribes to the bullet.
        private void SubscribeToBullet(GameObject bullet)
        {
            if (bullet.GetComponent<HE_BulletBehaviour>() != null)
            {
                bullet.GetComponent<HE_BulletBehaviour>().m_FollowDelagate = BulletDataObserver;
            }

            if (bullet.GetComponent<AP_BulletBehaviour>() != null)
            {
                bullet.GetComponent<AP_BulletBehaviour>().m_FollowDelagate = BulletDataObserver;
            }
            else
            {
                bullet.GetComponent<WB_BulletBehaviour>().m_FollowDelagate = BulletDataObserver;
            }

        }
        private void SelectAmmunitionForTarget()
        {
            switch (m_Target.tag)
            {
                case "EnemyTurret":
                    m_Bullet = m_AI_TurretController.m_Get_AP_Shell;
                    break;
                case "Infantry":
                    m_Bullet = m_AI_TurretController.m_Get_HE_Shell;
                    break;
                case "Wall":
                    m_Bullet = m_AI_TurretController.m_Get_WB_Shell;
                    break;
                default:
                    m_Bullet = m_AI_TurretController.m_Get_HE_Shell;
                    break;
            }
        }

        private void BulletDataObserver(Vector3 impactPosition, GameObject gameObjectHit)
        {
            if (m_Target != null && m_Target.tag == "Wall" || m_Target != null && m_Target.tag == "Untagged" && m_ObservedABullet == false)
            {

                m_ObservedABullet = true;
                m_Target.tag = "Untagged";
                m_AI_TurretController.m_GetSetCurrentState = new FindTarget(m_AI_TurretController);
            }

            //Checking if it missed, if so give the position the bullet hit.
            if (m_Target != null && gameObjectHit != null && gameObjectHit.transform.parent != null && 
                gameObjectHit.transform.name!= m_Target.name)
            {
                m_AI_TurretController.m_GetSetCurrentState = new AimAndCalculateTrajectory(m_AI_TurretController, m_Target, impactPosition);
            }

        }

        public void Update()
        {
            //Left barrel.
            if (m_LeftReload <= 0)
            {
                CreateBulletLeftBarrel();
                m_LeftReload = Random.Range(3.5f, 6.0f);
            }

            //Right barrel.
            if (m_RightReload <= 0)
            {
                CreateBulletRightBarrel();
                m_RightReload = Random.Range(3.5f, 6.0f);
            }

            m_LeftReload -= Time.deltaTime;
            m_RightReload -= Time.deltaTime;
        }
    }

    public class FindTarget : State
    {

        private AI_TurretController m_AI_TurretController;

        private List<GameObject> m_Turrets = new List<GameObject>();
        private List<GameObject> m_Infantry = new List<GameObject>();
        private List<GameObject> m_Walls = new List<GameObject>();

        private GameObject m_Target;

        private bool m_EndOfCheck = false;
        private bool m_HasTarget = false;

        public FindTarget(AI_TurretController controller)
        {
            m_AI_TurretController = controller;

            m_Turrets.AddRange(GameObject.FindGameObjectsWithTag("EnemyTurret"));
            m_Infantry.AddRange(GameObject.FindGameObjectsWithTag("Infantry"));
            m_Walls.AddRange(GameObject.FindGameObjectsWithTag("Wall"));

            SetTargetsByValue();
        }

        private void SetTargetsByValue()
        {
            float TargetValue = 1400.0f;
            float Value = 0;

            if (m_Turrets.Count >= 1)
            {
                for (int i = 0; i < m_Turrets.Count; i++)
                {
                    Value = Vector3.Distance(m_Turrets[i].transform.position, m_AI_TurretController.transform.position);
                    Value -= 1400;
                    if (Value < TargetValue)
                    {
                        m_HasTarget = true;
                        TargetValue = Value;
                        m_Target = m_Turrets[i].transform.gameObject;
                        m_Target.GetComponent<EnemyTurretAI>().m_FollowDelagate += EnemyTargetObserver;
                    }
                }
            }

            if (m_Infantry.Count >= 1)
            {
                for (int i = 0; i < m_Infantry.Count; i++)
                {
                    Value = Vector3.Distance(m_Infantry[i].transform.position, m_AI_TurretController.transform.position);
                    Value -= 10;
                    if (Value < TargetValue)
                    {
                        m_HasTarget = true;
                        TargetValue = Value;
                        m_Target = m_Infantry[i].transform.gameObject;
                    }
                }
            }

            if (m_Walls.Count >= 1)
            {
                for (int i = 0; i < m_Walls.Count; i++)
                {
                    Value = Vector3.Distance(m_Walls[i].transform.position, m_AI_TurretController.transform.position);

                    if (Value < TargetValue)
                    {
                        m_HasTarget = true;
                        TargetValue = Value;
                        m_Target = m_Walls[i].transform.gameObject;
                    }
                }
            }

            m_EndOfCheck = true;

        }

        private void EnemyTargetObserver()
        {
            m_AI_TurretController.m_GetSetCurrentState = new Moving(m_AI_TurretController);

        }

        public void Update()
        {
            if (m_EndOfCheck == true && m_Target != null)
            {
                m_AI_TurretController.m_GetSetCurrentState = new AimAndCalculateTrajectory(m_AI_TurretController, m_Target, Vector3.zero);
            }

            if (m_EndOfCheck == true && m_HasTarget == false)
            {
                //END STATE
                Debug.Log("NO MORE TARGETS");
            }

        }
    }

    public class HidingWhenHit : State
    {
        AI_TurretController m_AI_Controller;

        private Vector3 m_TargetHidingSpot = Vector3.zero;

        private GameObject m_CurrentHidingObject;

        private NavMeshAgent m_NavmeshAgent;

        private List<GameObject> m_HidingSpots = new List<GameObject>();

        private float m_WaitingTime = 5f;

        public HidingWhenHit(AI_TurretController controller)
        {
            m_AI_Controller = controller;

            m_HidingSpots.AddRange(GameObject.FindGameObjectsWithTag("HidingSpot"));

            m_NavmeshAgent = controller.gameObject.GetComponent<NavMeshAgent>();

           
        }

        private void SetClosestHidingSpot()
        {
            //Selecting closest hiding spot.
            for (int i = 0; i < m_HidingSpots.Count; i++)
            {

                if (m_TargetHidingSpot != Vector3.zero &&
                    Vector3.Distance(m_AI_Controller.gameObject.transform.position, m_HidingSpots[i].transform.position) <=
                    Vector3.Distance(m_AI_Controller.gameObject.transform.position, m_TargetHidingSpot))
                {
                    m_TargetHidingSpot = m_HidingSpots[i].transform.position;
                    m_CurrentHidingObject = m_HidingSpots[i];
                }

                if (m_TargetHidingSpot == Vector3.zero)
                {
                    m_TargetHidingSpot = m_HidingSpots[i].transform.position;
                    m_CurrentHidingObject = m_HidingSpots[i];

                }
            }

            m_NavmeshAgent.SetDestination(m_TargetHidingSpot);
        }

        public void Update()
        {
            if (!m_NavmeshAgent.pathPending && m_NavmeshAgent.remainingDistance <= m_NavmeshAgent.stoppingDistance
                && !m_NavmeshAgent.hasPath)
            {
                if (m_CurrentHidingObject != null)
                {
                    GameObject.Destroy(m_CurrentHidingObject);
                }

                m_WaitingTime -= Time.deltaTime;
            }

            if (m_WaitingTime <= 0)
            {
                m_AI_Controller.m_GetSetCurrentState = new Moving(m_AI_Controller);
            }
        }
    }
}