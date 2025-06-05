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
    private float xRotation = 0f;         // ī�޶��� ���� ȸ����
    void Start()
    {
       
        // ���� �÷��̾ ī�޶� Ȱ��ȭ
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
        if (SceneManager.GetActiveScene().name == "Menu_Online")	//Constant.SCENE_GAME�� ���� �� �̸��� �°� �ٲ� ��
        {
            if (isLocalPlayer)//�������� isOwned������, ����� �ٲ�)
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
            // ���鿡 �ִٸ� y�� �ӵ� �ʱ�ȭ
            moveInfo.verticalVelocity = -0.5f;

            // ���� �Է� ó��
            if (Input.GetButtonDown("Jump")) // �⺻������ �����̽���
            {
                moveInfo.verticalVelocity = moveInfo.jumpForce;
            }
        }
        else
        {
            // ���߿� �ִٸ� �߷� ����
            moveInfo.verticalVelocity -= moveInfo.gravity * Time.deltaTime;
        }

        moveDirection = move;
        moveDirection.y = moveInfo.verticalVelocity;
        controller.Move(moveDirection * Time.deltaTime);
    }
    public void CameraRotate()
    {
        // ���콺 �Է� �ޱ�
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ī�޶��� ���� ȸ�� ����
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // ���� ȸ���� -90������ 90�� ���̷� ����

        // ī�޶�� �÷��̾� ȸ��
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);  // ī�޶� ���� ȸ��
        this.transform.Rotate(Vector3.up * mouseX);                          // �÷��̾� ���� ȸ��
    }
    public RaycastHit RayCast_Camera()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        var cam = playerCamera;
        // ȭ�� �߾����κ��� ���� ����
        Ray ray = cam.ScreenPointToRay(screenCenter);

        // RaycastHit ����ü�� �̿��� �浹 ���� �ޱ�
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayInfo.rayDistance, rayInfo.hitLayers))
        {
            if(hit.collider.gameObject.name == "Cube (1)")
            {
                GameObject.Find("MapGenerator").GetComponent<MapGenerate>().MapGenerate_Start();
                NetworkServer.Destroy(hit.collider.gameObject);
            }
           

            // �浹�� ������Ʈ�� ������ ���
            Debug.Log("Hit object: " + hit.collider.gameObject.name);

            return hit;
            // ���� ���� �浹 ������ ������ �ϰ� �ʹٸ� ���⿡�� ó�� ����
            // ��: hit.collider.gameObject.SendMessage("OnHitByRay", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            // �浹�� ������Ʈ�� ���� ���
            // �ʿ��ϴٸ� �� �κп� ���� �߰�
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