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


    //Skill 1�� serach change possible object
    public LayerMask Obstacles;
    public LayerMask Teleport;
    public float changeRange;
    public bool objectInChangeRange;
    public bool teleportInRange;
    public GameObject[] characterList;

    //skill 1�� ��� �� �ð� üũ
    public float spanTime = 0.0f;
    public float maxTime = 0.7f;
    public bool IsObstacle = false;
    public int index = 0;

    //skill 3��  ���� �߰������� ������ ������ ��ȯ�� �� ���� z���� ������ ������ ��ġ�� ����ؼ� ��ġ�� ������ ����Ű�� ȸ�� �� 
    //������ Z�� ������ ����Ű�� ȸ������ �ʴ��� ȸ���� �ν��ϰ� �׿� �´� ��ġ�� �� ���� ��� �ʿ�
    public GameObject wallPrefab;
    public List<GameObject> wallDestroy = new List<GameObject>();
    //skill 3�� �� ��ġ �� ���ϴ� ��ҿ� ��ġ
    private Vector3 mOffset;
    private float mZCoord;
    public GameObject blockWallPrefab;
    //3�� Ű�� �������� Ȯ��
    public bool isPressThree;

    private void Start()
    {
        CinemachineCore.GetInputAxis = GetAxisCustom;
        
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
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
            if ((Input.GetKey(KeyCode.LeftShift)) || (Input.GetKey(KeyCode.RightShift)))
            {
                animator.SetBool("isRun", true);
                animator.SetBool("isWalk", false);
            }
            if (!(Input.GetKey(KeyCode.LeftShift)) && !(Input.GetKey(KeyCode.RightShift)))
            {
                animator.SetBool("isRun", false);
                animator.SetBool("isWalk", true);
            }
            if (Input.GetMouseButtonDown(0))
            {
                animator.SetBool("isWalk", true);
                animator.SetBool("isThrow", false);
                Invoke("ThrowFalse", 1.5f);
            }
        }
        else
        {
            if(Input.GetMouseButtonDown(0))
            {
                animator.SetBool("isThrow", true);
                Invoke("ThrowFalse", 1.5f);  //1.5�� �ڿ� ThrowFalse �Լ� ����
                animator.SetBool("isWalk", false);
            }
            if(!(Input.GetMouseButtonDown(0)))
            {
                animator.SetBool("isRun", false);
                animator.SetBool("isWalk", false);
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
        if(objectInChangeRange && ((Input.GetKeyDown(KeyCode.Keypad1) || (Input.GetKeyDown(KeyCode.Alpha1)))))
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, changeRange); //�ݶ��̴� ���� - ����� �ݶ��̴� �迭 �� obstacles ���� - obstacle�� ����ؼ� 
           
            for(int i =0; i<colls.Length; i++)
            {
                if(colls[i].tag == "Obstacles")
                {
                    if(colls[i].name.Contains("barrel"))
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
            if(spanTime >= maxTime)
            {
                characterList[index].SetActive(false);
                
                characterList[0].SetActive(true);
                spanTime = 0;
            }
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            isPressThree = true;
            InstallBlock();
            Invoke("IsNotPressThree", 10f);
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
        Vector3 wallPosition = new Vector3(transform.position.x, transform.position.y-2.0f, transform.position.z+3.0f);
        Quaternion wallRotation = Quaternion.identity;
        wallRotation.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y+90.0f, transform.rotation.z);
        blockWallPrefab = Instantiate(wallPrefab, wallPosition, wallRotation);
        wallDestroy.Add(blockWallPrefab);
        Invoke("DestroyBlock", 3f);
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

    public float GetAxisCustom(string axisName) //Ŭ�� ī�޶�
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
        if((other.CompareTag("Teleport")))
        {

            SceneManager.LoadScene(2);
        }
    }
}