using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
public class PlayerMovementController : NetworkBehaviour
{
    [SerializeField] public RayInfo rayInfo;
    [SerializeField] public MoveInfo moveInfo;

    public GameObject settingUI;

    public Camera playerCamera;
    public CharacterController controller;
    public float mouseSensitivity = 100;

    
    private Vector3 moveDirection;
    private float xRotation = 0f;         // 카메라의 수직 회전값
    void Start()
    {
       
        // 로컬 플레이어만 카메라 활성화
        if (isLocalPlayer)
        {
            playerCamera.enabled = true;
            playerCamera.gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.Locked;
            settingUI.SetActive(false);
        }
        else
        {
            playerCamera.enabled = false;
            playerCamera.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Menu_Online")	//Constant.SCENE_GAME은 현재 씬 이름에 맞게 바꿀 것
        {
            if (isLocalPlayer)//예전에는 isOwned였으나, 현재는 바뀜)
            {
                UserInterface();
                Move();
                if (!settingUI.activeSelf)
                {
                    CameraRotate();
                }
                   
                if (Input.GetMouseButtonDown(0))
                {
                    RayCast_Camera();
                }
            }
        }
    }
    public void CameraCheck()
    {
        GameObject[] playerCameras = GameObject.FindGameObjectsWithTag("PlayerCam");

        foreach (GameObject cam in playerCameras)
        {
            if(cam != playerCamera)
            {
                cam.GetComponent<Camera>().enabled = false;
                cam.SetActive(false);
            }
            else
            {
                cam.GetComponent<Camera>().enabled = true;
                cam.SetActive(true);
            }
           
        }
    }
    public void UserInterface()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingUI.SetActive(!settingUI.activeSelf);
            if (settingUI.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
    public void SetPosition()
    {
        transform.position = new Vector3(6, 2, -8);//new Vector3(Random.Range(-2, 2), Random.Range(-2, 2));
    }
    public void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * moveX + transform.forward * moveZ)*moveInfo.speed;
        //controller.Move(move * speed * Time.deltaTime);

        if (controller.isGrounded)
        {
            // 지면에 있다면 y축 속도 초기화
            moveInfo.verticalVelocity = -0.5f;

            // 점프 입력 처리
            if (Input.GetButtonDown("Jump")) // 기본적으로 스페이스바
            {
                moveInfo.verticalVelocity = moveInfo.jumpForce;
            }
        }
        else
        {
            // 공중에 있다면 중력 적용
            moveInfo.verticalVelocity -= moveInfo.gravity * Time.deltaTime;
        }

        moveDirection = move;
        moveDirection.y = moveInfo.verticalVelocity;
        controller.Move(moveDirection * Time.deltaTime);
    }
    public void CameraRotate()
    {
        // 마우스 입력 받기
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 카메라의 수직 회전 제한
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // 상하 회전을 -90도에서 90도 사이로 제한

        // 카메라와 플레이어 회전
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);  // 카메라 수직 회전
        this.transform.Rotate(Vector3.up * mouseX);                          // 플레이어 수평 회전
    }
    public RaycastHit RayCast_Camera()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        var cam = playerCamera;
        // 화면 중앙으로부터 레이 생성
        Ray ray = cam.ScreenPointToRay(screenCenter);

        // RaycastHit 구조체를 이용해 충돌 정보 받기
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayInfo.rayDistance, rayInfo.hitLayers))
        {
            if(hit.collider.gameObject.name == "Cube (1)")
            {
                GameObject.Find("MapGenerator").GetComponent<MapGenerate>().MapGenerate_Start();
                NetworkServer.Destroy(hit.collider.gameObject);
            }
           

            // 충돌한 오브젝트의 정보를 출력
            Debug.Log("Hit object: " + hit.collider.gameObject.name);

            return hit;
            // 만약 레이 충돌 지점에 뭔가를 하고 싶다면 여기에서 처리 가능
            // 예: hit.collider.gameObject.SendMessage("OnHitByRay", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            // 충돌한 오브젝트가 없는 경우
            // 필요하다면 이 부분에 로직 추가
            Debug.Log("Hit object: Nothing");
            return hit;
        }
    }
}
[System.Serializable]
public class RayInfo
{
    public float rayDistance;
    public LayerMask hitLayers;
}
[System.Serializable]
public class MoveInfo
{
    public float speed = 5;
    public float jumpForce = 5f;
    public float gravity = 9.81f;
    public float verticalVelocity;
    public LayerMask groundMask;
}