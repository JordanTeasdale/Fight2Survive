using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("-----Components-----")]

    [SerializeField] CharacterController controller;

    [Header("-----Player Attributes-----")]

    [SerializeField] float playerSpeed;
    [SerializeField] float sprintMulti;
    [Range(1, 100)] public int HP;
    public bool isDead;


    Vector3 playerVelocity;
    Vector3 move = Vector3.zero;

    float playerSpeedOriginal;
    public int HPOrig;
    int prevHP;
    float healthSmoothTime = 0.5f;
    float healthSmoothCount;
    float healthFillAmount;

    // Player States
    bool isSprinting = false;
    bool isMeleeing = false;

    public GameObject cameraMain;

    // Start is called before the first frame update
    void Start()
    {
        playerSpeedOriginal = playerSpeed;
        HPOrig = HP;

    }

    // Update is called once per frame
    void Update()
    {
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

            //Getting input from Unity Input Manager
            move = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));

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
