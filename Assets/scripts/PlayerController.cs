using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform viewPoint;
    public float mouseSensitivity = 1f;
    private float verticalRotationStore;
    private Vector2 mouseInput;

    public float moveSpeed=5f;
    private Vector3 moveDirection,movement;
    private float activeMoveSpeed;
    private float runSpeed = 8f;
    public float jumpForce = 12f, gravitymod = 2.5f;

    private Camera cam;

    public Transform groundCheck;
    private bool isGrounded;
    public LayerMask groundLayers;

    public CharacterController characterController;

    public GameObject bulletImpact;
    //public float timeBtnShots=0.1f;  //bcz i will use the ones in my gun script
    private float shotsCounter;
    public float muzzleDisplayTime;
    private float muzzleCounter;

    public float maxHeatValue = 10f,/*heatPerShot=1f,*/coolRate=4f,overHeatcoolRate=5f;
    private float heatCounter;
    private bool overHeated;

    public Guns[] gunsInGame;
    private int selectedGun;

  
    // Start is called before the first frame update
    void Start()
    {
     
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main;

        UIController.instance.weapontempSlider.maxValue = maxHeatValue;
        SwitchGun();

        Transform newTransform = SpawnManager.instance.getSpawnPoint();
        transform.position = newTransform.position;
        transform.rotation = newTransform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        View();
        PlayerMovement();
    }
    void LateUpdate()
    {
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
    }

    void View()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
        verticalRotationStore += mouseInput.y;
        verticalRotationStore = Mathf.Clamp(verticalRotationStore, -60f, 60f);
        viewPoint.rotation = Quaternion.Euler(verticalRotationStore + mouseInput.y, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
    }
    void PlayerMovement()
    {
        //movement and Jumping
        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"),0f,Input.GetAxisRaw("Vertical"));
        if (Input.GetKey(KeyCode.LeftShift))
        {
            activeMoveSpeed = runSpeed;
        }
        else
        {
            activeMoveSpeed = moveSpeed;
        }

        float yVel=movement.y;
        movement = ((transform.forward * moveDirection.z) + (transform.right * moveDirection.x)).normalized * activeMoveSpeed;
        movement.y=yVel;

        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, 0.25f, groundLayers);

        if (Input.GetButtonDown("Jump")  && isGrounded)
        {
            movement.y = jumpForce;
        }

        movement.y += Physics.gravity.y * Time.deltaTime * gravitymod;

        if (characterController.isGrounded)
        {
            movement.y = 0f;
        }

        characterController.Move(movement * Time.deltaTime);

        //before we start shooting,deactivate the muzzle flush
        //first check if it is active in our scene
        if (gunsInGame[selectedGun].muzzleFlash.activeInHierarchy)
        {
            muzzleCounter -= Time.deltaTime;
        }
        gunsInGame[selectedGun].muzzleFlash.SetActive(false);


        //we need to shoot if our gun is not overheating to avoid these noobs from shooting 21/7
        if (!overHeated)
        {
            //in order to shoot
            if (Input.GetMouseButton(0))
            {
                Shoot();
            }

            //if we hold the button down w shoot Automatically
            if (Input.GetMouseButton(0) && gunsInGame[selectedGun].isAutomatic)
            {
                shotsCounter -= Time.deltaTime;

                if (shotsCounter <= 0)
                {
                    Shoot();
                }
            }

            heatCounter -= coolRate * Time.deltaTime;
        }

        else
        {
            //we need the heat counter to decrement to 0 or <0 to allow us to shoot again
            heatCounter -= overHeatcoolRate * Time.deltaTime;

            //if it manages to reach 0 or <0
            if (heatCounter <= 0)
            {
                overHeated = false;
                UIController.instance.overHeatedMessage.gameObject.SetActive(false);
            }
        }

        if (heatCounter<0)
        {
            //incase it went below zero it should reset to 0
            heatCounter = 0;
        }

        //linking our slider to all these variables in our script
        UIController.instance.weapontempSlider.value = heatCounter;

        //if we scroll our mouse wheel forward
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            selectedGun++;

            //suppose we have 3 guns and we are on gun3(index of the last gun in our array) and we scroll our mouse wheel that gets to 4
            //since 4 doesnot exist in our array that will register as an error
            if (selectedGun>=gunsInGame.Length)
            {
                //this allows us to wrap around
                selectedGun = 0;
            }
            SwitchGun();
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            selectedGun--;
            if (selectedGun<0)
            {
                selectedGun = gunsInGame.Length - 1;
            }
            SwitchGun();
        }


        //to avoid players failing to use the cursor after they are done playimg or want to use the menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if(Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f,0.5f,0f));
        ray.origin = cam.transform.position;

        if (Physics.Raycast(ray,out RaycastHit hit))
        {
            
            GameObject bulletImpactObject=Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));
            Destroy(bulletImpactObject,10f);
        }

        shotsCounter = gunsInGame[selectedGun].timeBtnShots;
        heatCounter += gunsInGame[selectedGun].heatPerShot;

        if (heatCounter >= maxHeatValue)
        {
            heatCounter = maxHeatValue;
            overHeated = true;

            UIController.instance.overHeatedMessage.gameObject.SetActive(true);
        }

        //activating the muzzle flash
        gunsInGame[selectedGun].muzzleFlash.SetActive(true);
        muzzleCounter = muzzleDisplayTime;

    }

    void SwitchGun()
    {
        /*to switch btn gus:-
        1.Deactivate all guns;
        2.In all guns show the selected gun only
        */

        //step 1.
        foreach(Guns guns in gunsInGame)
        {
            guns.gameObject.SetActive(false);
        }
        //step 2.
        gunsInGame[selectedGun].gameObject.SetActive(true);

        //bug :fixing on muzzle flush when switching btn guns
        gunsInGame[selectedGun].muzzleFlash.SetActive(false);

    }
}
