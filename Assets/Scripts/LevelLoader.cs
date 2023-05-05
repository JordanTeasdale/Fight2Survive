using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] GameObject crossfade;
    public float transitionRate;

    public Perseverance perseveranceScript;

    float deathTimer;
    private void Start() {
        Time.timeScale = 1;
        perseveranceScript = GameManager.instance.perseveranceMenu.GetComponent<Perseverance>();
    }

    private void FixedUpdate() {
        if (!GameManager.instance.playerScript.isDead) {
            deathTimer = 1.728f;
            GameManager.instance.perseveranceMenu.SetActive(false);
            if (crossfade.GetComponent<CanvasGroup>().alpha > 0) {
                crossfade.GetComponent<CanvasGroup>().alpha -= transitionRate;
            }
        } else {
            if (crossfade.GetComponent<CanvasGroup>().alpha < 1 && deathTimer < 0) {
                crossfade.GetComponent<CanvasGroup>().alpha += transitionRate;
            }
            else if (crossfade.GetComponent<CanvasGroup>().alpha >= 1) {
                GameManager.instance.perseveranceMenu.SetActive(true);
                perseveranceScript.ResetBar();
            }else {
                deathTimer -= Time.deltaTime;
            }
        }
    }

}
