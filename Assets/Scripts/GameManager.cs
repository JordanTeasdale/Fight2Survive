using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour {
    [SerializeField] RectTransform holder;

    public static GameManager instance;

    public GameObject player;
    public PlayerController playerScript;

    public GameObject RespawnPos;
    public GameObject checkpointFeedback;

    public GameObject pauseMenu;
    public GameObject playerDeadMenu;
    public GameObject playerClearedLevel1;
    public GameObject playerClearedLevel2;
    public GameObject playerWinMenu;
    public GameObject previousMenu = null;
    public GameObject menuCurrentlyOpen;
    public GameObject optionsMenu;
    public GameObject playerDamageFlash;
    public GameObject ammoMagGUI;
    public GameObject reticle;
    public GameObject radialMenu;
    public Image playerHPBar;
    public GameObject lowHealthIndicator;
    public GameObject roomClearedFeedback;
    public AudioMixer mainMixer;

    public bool isMainOptionsMenu;
    public bool isPaused = false;
    public bool onPauseMenu = true;
    public bool isConfigOptions = false;
    bool gameOver = false;

    // Start is called before the first frame update
    void Awake() {
        instance = this;
        if (GameObject.FindGameObjectWithTag("Player") != null) {
            player = GameObject.FindGameObjectWithTag("Player");
            playerScript = player.GetComponent<PlayerController>();

            //RespawnPos = GameObject.FindGameObjectWithTag("Respawn Pos");
            playerScript.Respawn();


        } else
            gameObject.SetActive(false);

        //menuCurrentlyOpen = pauseMenu;
    }

    private void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Cancel") && playerScript.HP > 0 && !gameOver) {

            isPaused = !isPaused;

            if (isPaused == false) {
                UnPause();
                return;
            }

            if (isConfigOptions == false) {
                TransitionFromOptionstoPause();
            }

            menuCurrentlyOpen.SetActive(onPauseMenu || isConfigOptions);

            if (onPauseMenu || isConfigOptions)
                CursorLockPause();
            else
                CursorUnlockUnpause();
        }
        if (playerScript.HP <= playerScript.HPOrig * 0.25 && menuCurrentlyOpen != pauseMenu)
            lowHealthIndicator.SetActive(true);
        else if (playerScript.HP > playerScript.HPOrig * 0.25 || menuCurrentlyOpen == pauseMenu)
            lowHealthIndicator.SetActive(false);

        AccessShowcase();
    }

    private void AccessShowcase() {
        if (Input.GetButton("AccessShow")) {
            SceneManager.LoadScene("Showcase Level");
        }
    }

    public void CursorLockPause() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        reticle.SetActive(false);
        Time.timeScale = 0;
        playerScript.enabled = false;
    }
    public void CursorLockSlowed() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        reticle.SetActive(false);
        player.GetComponentInChildren<CameraController>().enabled = false;
        Time.timeScale = 0.25f;
    }
    public void CursorUnlockUnslowed() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        reticle.SetActive(true);
        player.GetComponentInChildren<CameraController>().enabled = true;
        Time.timeScale = 1;
    }
    public void CursorUnlockUnpause() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        reticle.SetActive(true);
        Time.timeScale = 1;
        playerScript.enabled = true;
    }

    public void TransitionFromOptionstoPause() {
        onPauseMenu = true;
        previousMenu = menuCurrentlyOpen;
        menuCurrentlyOpen = pauseMenu;

    }

    public void UnPause() {

        if (previousMenu != null)
            previousMenu.SetActive(false);
        menuCurrentlyOpen.SetActive(false);

        isPaused = false;
        onPauseMenu = false;
        isConfigOptions = false;
        previousMenu = null;
        menuCurrentlyOpen = null;
        CursorUnlockUnpause();
    }

}
