using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class ThirdPersonMovement : MonoBehaviour
{
    //public Camera CM;
    public CharacterController controller;
    public Animator animator;

    public Transform cam;
    public float speed;
    public float turnSmoothTime;
    public float turnSmoothVelocity;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float gravity;
    public float groundDistance;
    private Vector3 velocity;
    private bool isGrounded;

    //Skill 1번 serach change possible object
    public LayerMask Obstacles;
    public LayerMask Teleport;
    public float changeRange;
    public bool objectInChangeRange;
    public bool teleportInRange;
    public GameObject[] characterList;

    //스킬사용 이후 쿨타임
    public GameObject skillUse;
    public GameObject skillUse3;

    public GameObject skillUse2;

    //skill 1번 사용 후 시간 체크
    public float spanTime = 0.0f;
    public float maxTime = 0.7f;
    public bool IsObstacle = false;
    public int index = 0;

    //skill 3번  지금 추가적으로 베르가 방향을 전환할 때 벽이 z축이 고정되 동일한 위치에 계속해서 배치됨 베르가 방향키로 회전 시 
    //베르의 Z축 방향이 방향키로 회전하지 않더라도 회전을 인식하고 그에 맞는 위치에 벽 생성 기능 필요
    public GameObject wallPrefab;
    public List<GameObject> wallDestroy = new List<GameObject>();
    //skill 3번 벽 설치 후 원하는 장소에 배치
    private Vector3 mOffset;
    private float mZCoord;
    public GameObject blockWallPrefab;
    //3번 키를 눌렀는지 확인
    public bool isPressThree;

    //키가 없을 때 문 앞에 콜라이더와 충돌했을 때 플레이어에게 키가 필요하다는 문구 출력을 위해 keyHolder에 containskey함수 참조
    public GameObject CanOpenDoor;

   
    private void Start()
    {
        CinemachineCore.GetInputAxis = GetAxisCustom;
    }
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }


        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if ((Input.GetKey(KeyCode.W)) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)
            || (Input.GetKey(KeyCode.UpArrow)) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            animator.SetBool("isWalk", true);
            animator.SetBool("isRun", false);
            speed = 4;
            if ((Input.GetKey(KeyCode.LeftShift)) || (Input.GetKey(KeyCode.RightShift)))
            {
                animator.SetBool("isRun", true);
                animator.SetBool("isWalk", false);
                speed = 10;
            }
            if (!(Input.GetKey(KeyCode.LeftShift)) && !(Input.GetKey(KeyCode.RightShift)))
            {
                animator.SetBool("isRun", false);
                animator.SetBool("isWalk", true);
                speed = 4;
            }
            if (Input.GetMouseButtonDown(0))
            {
                animator.SetBool("isWalk", false);
                animator.SetBool("isThrow", true);
                speed = 4;
                //던지고 일정 시간 뒤에 isThrow false
                Invoke("ThrowFalse", 2f);
            }
            //변신 시 자식 레이어까지 전부 Obstacles로 변경 적은 플레이어를 장해물로 판단하고 다시 순찰
            gameObject.layer = 11;
            foreach (Transform child in transform)
            {
                child.gameObject.layer = 11;
            }    
        }
        else
        {
            /////////////////////////////////////////////////////////////////////////////////////6/20 이 구문에 !(Input.GetKeyDown(KeyCode.Alpha3) && !(Input.GetKeyDown(KeyCode.Keypad3) 
            ////////////////////////////////////////////////////////////////////////////////////추가 이유는 왼쪽 버튼을 누르고 쿨타임 조건 만족 시에 던지는 모션을 주니까 3번 스킬 사용 후 벽 이동 시에도 던지는 모션 실행
            if (Input.GetMouseButtonDown(0) && skillUse2.GetComponent<CoolDown>().cooldownTimer == 0 && !(isPressThree))
            {
                animator.SetBool("isThrow", true);
                Invoke("ThrowFalse", 2f);  //1.5초 뒤에 ThrowFalse 함수 실행
                animator.SetBool("isWalk", false);
            }
            if (!(Input.GetMouseButtonDown(0)))
            {
                animator.SetBool("isRun", false);
                animator.SetBool("isWalk", false);
            }

            if (IsObstacle)
            {
                gameObject.layer = 13;
                foreach (Transform child in transform)
                {
                    child.gameObject.layer = 13;
                }
            }

            if (!IsObstacle)
            {
                gameObject.layer = 11;
                foreach (Transform child in transform)
                {
                    child.gameObject.layer = 11;
                }
            }
        }

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        //check object
        objectInChangeRange = Physics.CheckSphere(transform.position, changeRange, Obstacles);

        //Skill 1
        if (skillUse.GetComponent<CoolDown>().cooldownTimer == 0.0f && objectInChangeRange && ((Input.GetKeyDown(KeyCode.Keypad1) || (Input.GetKeyDown(KeyCode.Alpha1)))))
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, changeRange); //콜라이더 검출 - 검출된 콜라이더 배열 중 obstacles 검출 - obstacle을 사용해서 

            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].tag == "Obstacles")
                {
                    //게임 스킬 사용 시 쿨타임 카운트 다운
                    skillUse.GetComponent<CoolDown>().UseSkill();

                  
                   
                    if (colls[i].name.Contains("barrel"))
                    {
                        characterList[0].SetActive(false);
                        characterList[1].SetActive(true);
                        index = 1;
                    }

                    if (colls[i].name.Contains("Car1"))
                    {
                        characterList[0].SetActive(false);
                        characterList[2].SetActive(true);
                        index = 2;
                    }

                    if (colls[i].name.Contains("Broom"))
                    {
                        characterList[0].SetActive(false);
                        characterList[3].SetActive(true);
                        index = 3;
                    }

                    if (colls[i].name.Contains("Wand5"))
                    {
                        characterList[0].SetActive(false);
                        characterList[4].SetActive(true);
                        index = 4;
                    }

                    IsObstacle = true;
                    break;
                }
            }
        }

        if (IsObstacle)
        {
            spanTime += Time.deltaTime;

            if (spanTime >= maxTime)
            {
                characterList[index].SetActive(false);

                characterList[0].SetActive(true);
                spanTime = 0;

                IsObstacle = false;
            }
        }

      

        if ((skillUse3.GetComponent<CoolDown>().cooldownTimer == 0.0f) && (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)))
        {
            //게임 스킬 사용 시 쿨타임 카운트 다운
            skillUse3.GetComponent<CoolDown>().UseSkill();

            animator.SetBool("isThrow", false);
            isPressThree = true;
            InstallBlock();
            Invoke("IsNotPressThree", 5f);
        }
        //if (isPressThree && Input.GetMouseButton(0))
        //{
        //    OnMouseDown();
        //    OnMouseDrag();
        //}  
    }

    public void IsNotPressThree()
    {
        isPressThree = false;
    }

    void InstallBlock()
    {
        Debug.Log(gameObject.transform.rotation.eulerAngles.y);
        Vector3 wallPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Quaternion wallRotation = Quaternion.identity;
        if (gameObject.transform.rotation.eulerAngles.y < 45.0f || gameObject.transform.rotation.eulerAngles.y > 315.0f)
        {
            wallPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + 5.0f);
            if (wallPrefab.name.Contains("another"))
            {
                wallPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + 2.0f);

            }
        }

        if (gameObject.transform.rotation.eulerAngles.y > 45.0f && gameObject.transform.rotation.eulerAngles.y < 135.0f)
        {
            wallPosition = new Vector3(gameObject.transform.position.x + 5.0f, gameObject.transform.position.y, gameObject.transform.position.z);
            wallRotation = Quaternion.Euler(0, 90, 0);
            if (wallPrefab.name.Contains("another"))
            {
                wallPosition = new Vector3(gameObject.transform.position.x + 2.0f, gameObject.transform.position.y, gameObject.transform.position.z);

            }
        }

        if (gameObject.transform.rotation.eulerAngles.y > 135.0f && gameObject.transform.rotation.eulerAngles.y < 225.0f)
        {
            wallPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z - 5.0f);
            if (wallPrefab.name.Contains("another"))
            {
                wallPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z - 2.0f);

            }
        }

        if (gameObject.transform.rotation.eulerAngles.y < 315.0f && gameObject.transform.rotation.eulerAngles.y > 225.0f)
        {
            wallPosition = new Vector3(gameObject.transform.position.x - 5.0f, gameObject.transform.position.y, gameObject.transform.position.z);
            wallRotation = Quaternion.Euler(0, 90, 0);
            if(wallPrefab.name.Contains("another"))
            {
                Debug.Log("어나더?");
                wallPosition = new Vector3(gameObject.transform.position.x - 2.0f, gameObject.transform.position.y, gameObject.transform.position.z);

            }
        }
        blockWallPrefab = Instantiate(wallPrefab, wallPosition, wallRotation);
        wallDestroy.Add(blockWallPrefab);
        Invoke("DestroyBlock", 5f);
    }

    public void DestroyBlock()
    {
        Destroy(wallDestroy[0]);
        wallDestroy.RemoveAt(0);
    }

    //private void OnMouseDown()
    //{
    //    mZCoord = CM.WorldToScreenPoint(blockWallPrefab.transform.position).z;
    //    mOffset = blockWallPrefab.transform.position - GetMouseWorldPos();
    //}

    //private Vector3 GetMouseWorldPos()
    //{
    //    Vector3 mousePoint = Input.mousePosition;

    //    mousePoint.z = mZCoord;

    //    return CM.ScreenToWorldPoint(mousePoint);
    //}

    //private void OnMouseDrag()
    //{
    //    blockWallPrefab.transform.position = GetMouseWorldPos() + mOffset;
    //}

    public void ThrowFalse()
    {
        animator.SetBool("isThrow", false);
    }

    public float GetAxisCustom(string axisName) //클릭 카메라
    {
        if (axisName == "Mouse X")
        {
            if (Input.GetMouseButton(1))
            {
                float v = UnityEngine.Input.GetAxis("Mouse X");
                return -v;
            }

            else
            {
                return 0;
            }
        }

        else if (axisName == "Mouse Y")
        {
            if (Input.GetMouseButton(1))
            {
                float v = UnityEngine.Input.GetAxis("Mouse Y");
                return -v;
            }

            else
            {
                return 0;
            }
        }
        return UnityEngine.Input.GetAxis(axisName);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, changeRange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Teleport"))
        {
            SceneManager.LoadScene(2);
        }
    }
}