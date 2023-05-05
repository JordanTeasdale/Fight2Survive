using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Perseverance : MonoBehaviour
{
    [SerializeField] Image PerseveranceBar;
    [SerializeField] GameObject aKey;
    [SerializeField] GameObject dKey;

    static int deathCount = 0;
    float decayRate;
    float regenRate = 0.02f;
    bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PerseveranceBar.fillAmount -= Time.deltaTime * 0.05f;

        if (PerseveranceBar.fillAmount >= 0.98) {
            GameManager.instance.playerScript.Revive();
            PerseveranceBar.fillAmount = 0;
            isActive = false;
        }
    }

    public void ResetBar() {
        if (!isActive) {
            isActive = true;
            PerseveranceBar.fillAmount = 0.5f;
            regenRate = 0.02f;
            if (deathCount > 0)
                regenRate /= deathCount;
            StartCoroutine(PerservA());
        }
    }

    IEnumerator PerservA() {
        aKey.SetActive(true);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.A) || !GameManager.instance.playerScript.isDead);
        aKey.SetActive(false);
        if (GameManager.instance.playerScript.isDead) {
            PerseveranceBar.fillAmount += regenRate;
            StartCoroutine(PerservD());
        }
    }

    IEnumerator PerservD() {
        dKey.SetActive(true);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.D) || !GameManager.instance.playerScript.isDead);
        dKey.SetActive(false);
        if (GameManager.instance.playerScript.isDead) {
            PerseveranceBar.fillAmount += regenRate;
            StartCoroutine(PerservA());
        }
    }
}
