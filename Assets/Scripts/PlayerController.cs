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
    [SerializeField] float attackSpeed;
    [SerializeField] float comboTime;
    [Range(1, 100)] public int HP;
    public bool isDead;

    public event System.Action<AnimationEvent> OnSwing;


    Vector3 playerVelocity;
    Vector3 move = Vector3.zero;
    Animator animator;

    public int HPOrig;
    int prevHP;
    float healthSmoothTime = 0.5f;
    float healthSmoothCount;
    float healthFillAmount;

    // Player States
    bool isAttacking = false;
    public int ComboNumber = 0;
    float comboTimer;


    // Start is called before the first frame update
    void Start() {
        HPOrig = HP;
        animator = GetComponentInChildren<Animator>();
        comboTimer = 0;
    }

    // Update is called once per frame
    void Update() {
        if (!isDead)
            PlayerMovement();

        if (Input.GetKeyDown("mouse 0")) {
            StartCoroutine(Attack());
        }

        comboTimer -= Time.deltaTime;
        if (comboTimer < 0) {
            ComboNumber = 1;
        }
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

        //Adding the move vector to the character controller
        controller.Move(move * playerSpeed * Time.deltaTime);

        controller.Move(playerVelocity * Time.deltaTime);

        if (move != Vector3.zero) {
            animator.SetBool("Moving", true);
            animator.SetFloat("Velocity X", Vector3.Dot(transform.right, move));
            animator.SetFloat("Velocity Z", Vector3.Dot(transform.forward, move));
        } else
            animator.SetBool("Moving", false);

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

    IEnumerator Attack() {
        if (!isAttacking) {
            isAttacking = true;
            animator.SetTrigger("Attack");
            animator.SetInteger("Action", ComboNumber);
            ComboNumber++;
            if (ComboNumber > 3)
                comboTimer = 0;
            else
                comboTimer = comboTime;
            Debug.Log("Howdy");
            yield return new WaitForSeconds(attackSpeed);
            isAttacking = false;
            animator.SetInteger("Action", 0);
        }
    }
}
