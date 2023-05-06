using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public GameObject player;
    public PlayerController playerScript;


    public GameObject perseveranceMenu;

    public Image playerHPBar;
    public GameObject ScreenFader;
    public LevelSpawner currentLevel;

    // Start is called before the first frame update
    void Awake() {
        instance = this;
        if (GameObject.FindGameObjectWithTag("Player") != null) {
            player = GameObject.FindGameObjectWithTag("Player");
            playerScript = player.GetComponent<PlayerController>();


        } else
            gameObject.SetActive(false);
    }

    private void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

}
