using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class ThirdPersonMovement : MonoBehaviour
{
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

    //skill 1번 사용 후 시간 체크
    public float spanTime = 0.0f;
    public float maxTime = 0.7f;
    public bool IsObstacle = false;
    public int index = 0;

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
            else
            {
                animator.SetBool("isRun", false);
                animator.SetBool("isWalk", true);
            }
        }
        else
        {
            animator.SetBool("isRun", false);
            animator.SetBool("isWalk", false);
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
            Collider[] colls = Physics.OverlapSphere(transform.position, changeRange); //콜라이더 검출 - 검출된 콜라이더 배열 중 obstacles 검출 - obstacle을 사용해서 
           
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

        ////Skill 2
        //if ((Input.GetKeyDown(KeyCode.Keypad2) || (Input.GetKeyDown(KeyCode.Alpha2)){
            
        //}
        //teleportInRange = Physics.CheckSphere(transform.position, changeRange, Teleport);
        //if (teleportInRange)
        //{
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //}
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
        if((other.CompareTag("Teleport")))
        {

            SceneManager.LoadScene(2);
        }
    }
}