using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    [SerializeField] LayerMask layersToHit;

    [Header("-----Components-----")]

    [SerializeField] CharacterController controller;
    public GameObject cameraMain;
    [SerializeField] GameObject camRotPoint;

    [Header("-----Player Attributes-----")]

    [SerializeField] float playerSpeed;
    [SerializeField] float sprintMulti;
    [Range(1, 100)] public int HP;
    public bool isDead;


    Vector3 playerVelocity;
    Vector3 move = Vector3.zero;

    Vector3 screenPosition;
    Vector3 worldPosition;
    Vector3 mouseDir;

    float playerSpeedOriginal;
    public int HPOrig;
    int prevHP;
    float healthSmoothTime = 0.5f;
    float healthSmoothCount;
    float healthFillAmount;

    // Player States
    bool isSprinting = false;
    bool isMeleeing = false;


    // Start is called before the first frame update
    void Start() {
        playerSpeedOriginal = playerSpeed;
        HPOrig = HP;

    }

    // Update is called once per frame
    void Update() {
        if (!isDead)
            PlayerMovement();
    }

    void FixedUpdate() {
        healthSmoothCount = System.Math.Min(healthSmoothTime, healthSmoothCount + Time.fixedDeltaTime);
        if (healthFillAmount != HP) {
            healthFillAmount = Mathf.Lerp(prevHP, HP, healthSmoothCount / healthSmoothTime);
            UpdateHP();
        }
    }

    void PlayerMovement() {
        screenPosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hitData, 10000, layersToHit)) {
            worldPosition = hitData.point;
            Debug.Log("Hitting");
        }
        Debug.DrawLine(cameraMain.transform.position, worldPosition);

        mouseDir = worldPosition - transform.position;

        float angle = Vector3.Angle(new Vector3(transform.forward.x, 0, transform.forward.z), new Vector3(mouseDir.x, 0, mouseDir.z));
        if (angle > 2)
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + angle, transform.localEulerAngles.z); 
        //camRotPoint.transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y + angle, transform.rotation.z);

        //Getting input from Unity Input Manager
        move = (new Vector3(1, 0, 0) * Input.GetAxis("Horizontal")) + (new Vector3(0, 0, 1) * Input.GetAxis("Vertical"));

        //Adding the move vector to the character controller
        controller.Move(move * playerSpeed * Time.deltaTime);

        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void Respawn() {
        isDead = false;
        healthSmoothCount = 0;
        controller.enabled = false;
        transform.position = GameManager.instance.RespawnPos.transform.position;
        controller.enabled = true;

    }

    public IEnumerator Death() {
        isDead = true;
        cameraMain.GetComponent<Animator>().enabled = true;
        cameraMain.GetComponent<Animator>().SetTrigger("isDead");
        yield return new WaitForSeconds(1.728f);
        GameManager.instance.CursorLockPause();
        GameManager.instance.playerDeadMenu.SetActive(true);
        GameManager.instance.menuCurrentlyOpen = GameManager.instance.playerDeadMenu;
        GameManager.instance.isPaused = true;
    }

    public void ResetHP() {
        HP = HPOrig;
    }

    public void UpdateHP() {
        //GameManager.instance.playerHPBar.fillAmount = healthFillAmount / HPOrig;
    }
}
