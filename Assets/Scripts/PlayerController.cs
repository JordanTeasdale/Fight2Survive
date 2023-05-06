using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IDamageable {
    [SerializeField] LayerMask layersToHit;

    [Header("-----Components-----")]
    [SerializeField] CharacterController controller;
    public GameObject cameraMain;
    [SerializeField] GameObject camRotPoint;
    [SerializeField] BoxCollider attackBox;

    [Header("-----Player Attributes-----")]
    [SerializeField] float playerSpeed;
    [SerializeField] float sprintMulti;
    [SerializeField] float attackSpeed;
    [SerializeField] float comboTime;
    [SerializeField] float invincibilityTimer;
    [SerializeField] int baseDamage;
    [SerializeField] int finalDamage;
    [SerializeField] float baseStun;
    [SerializeField] float finalStun;
    [Range(1, 100)] public int HP;
    public bool isDead = false;
    public bool fullyRevived = true;

    public event System.Action<AnimationEvent> OnSwing;


    Vector3 playerVelocity;
    Vector3 move = Vector3.zero;
    Animator animator;
    public SkinnedMeshRenderer rend;

    public int HPOrig;
    int prevHP;
    float healthSmoothTime = 0.5f;
    float healthSmoothCount;
    float healthFillAmount;
    float damageTimer;

    // Player States
    bool isAttacking = false;
    public int ComboNumber = 1;
    float comboTimer;


    // Start is called before the first frame update
    void Start() {
        HPOrig = HP;
        animator = GetComponentInChildren<Animator>();
        attackBox = GetComponentInChildren<BoxCollider>();
        rend = transform.GetChild(0).GetComponentInChildren<SkinnedMeshRenderer>();
        comboTimer = 0;
    }

    // Update is called once per frame
    void Update() {
        if (damageTimer > 0)
            damageTimer -= Time.deltaTime;

        if (!isDead && fullyRevived) {
            PlayerMovement();

            if (Input.GetKeyDown("mouse 0")) {
                StartCoroutine(Attack());
            }
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

        playerVelocity.y -= Time.deltaTime;
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        /*isDead = false;
        healthSmoothCount = 0;
        transform.position = GameManager.instance.RespawnPos.transform.position;
        controller.enabled = true;*/

    }

    public void TakeDamage(int _damage, float _stun) {
        if (damageTimer <= 0) {
            damageTimer = invincibilityTimer;

            if (_damage > HP)
                _damage = HP;

            if (healthFillAmount == HP) {
                healthSmoothCount = 0;
                prevHP = HP;
            }
            HP -= _damage;

            if (_damage > 0) {
                UpdateHP();
                StartCoroutine(DamageFlash());
                if (HP <= 0) {
                    // Kill the player
                    fullyRevived = false;
                    animator.SetTrigger("Dead");
                    StartCoroutine(FirstDeath());
                }
            }
        }
    }

    IEnumerator DamageFlash() {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        rend.material.color = Color.white;
    }

    public IEnumerator FirstDeath() {
        isDead = true;
        yield return new WaitForSeconds(1.728f);
        //GameManager.instance.CursorLockPause();
        //GameManager.instance.isPaused = true;
    }

    public void ResetHP() {
        HP = HPOrig;
    }

    public void UpdateHP() {
        GameManager.instance.playerHPBar.fillAmount = healthFillAmount / HPOrig;
    }

    IEnumerator Attack() {
        if (!isAttacking) {
            isAttacking = true;
            animator.SetTrigger("Attack");
            animator.SetInteger("Action", ComboNumber);
            attackBox.enabled = true;
            yield return new WaitForSeconds(0.5f);
            attackBox.enabled = false;
            yield return new WaitForSeconds(attackSpeed - 0.1f);
            isAttacking = false;
            animator.SetInteger("Action", 0);
            if (ComboNumber >= 3)
                comboTimer = 0;
            else
                comboTimer = comboTime;
            ComboNumber++;
        }
    }

    public void Revive() {
        damageTimer = 5;
        isDead = false;
        HP = HPOrig / 2;
        UpdateHP();
        animator.SetTrigger("Revived");
        Time.timeScale = 1;
        fullyRevived = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<IDamageable>(out IDamageable obj) && other.CompareTag("Enemy") && other.isTrigger == false) {
            if (ComboNumber > 2)
                obj.TakeDamage(finalDamage, finalStun);
            else
                obj.TakeDamage(baseDamage, baseStun);
        }
    }
}
