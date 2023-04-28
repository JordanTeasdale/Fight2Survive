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
    Animator animator;

    Vector3 screenPosition;
    Vector3 worldPosition;
    Vector3 mouseDir;

    float playerSpeedOriginal;
    public int HPOrig;
    int prevHP;
    float healthSmoothTime = 0.5f;
    float healthSmoothCount;
    float healthFillAmount;

    float prevangle;

    // Player States
    bool isSprinting = false;
    bool isMeleeing = false;


    // Start is called before the first frame update
    void Start() {
        playerSpeedOriginal = playerSpeed;
        HPOrig = HP;
        animator = GetComponentInChildren<Animator>();
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
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
        Vector3 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
        float playerAngle = transform.eulerAngles.y;

        //Getting input from Unity Input Manager
        move = (new Vector3(1, 0, 0) * Input.GetAxis("Horizontal")) + (new Vector3(0, 0, 1) * Input.GetAxis("Vertical"));
        if (move != Vector3.zero)
            animator.SetBool("Moving", true);
        else
            animator.SetBool("Moving", true);

        //Adding the move vector to the character controller
        controller.Move(move * playerSpeed * Time.deltaTime);

        controller.Move(playerVelocity * Time.deltaTime);

        camRotPoint.transform.position = transform.position;
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
