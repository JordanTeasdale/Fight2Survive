using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSpawner : MonoBehaviour {
    [Header("-----Environment-----")]
    [SerializeField] GameObject[] Enviroment;

    [Header("-----Enemy Prefabs-----")]
    [SerializeField] GameObject baseSlime;
    [SerializeField] GameObject catSlime;
    [SerializeField] GameObject vikingSlime;

    [Header("----- Base Slime Spawn Settings -----")]
    [SerializeField] Vector2[] baseSlimeLocation;
    [Header("----- Cat Slime Spawn Settings -----")]
    [SerializeField] Vector2[] catSlimeLocation;
    [Header("----- Viking Slime Spawn Settings -----")]
    [SerializeField] Vector2[] vikingSlimeLocation;

    [SerializeField] int currentLevel;
    public int enemiesAlive;

    LevelLoader levelLoader;

    // Start is called before the first frame update
    void Start() {
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        GameManager.instance.currentLevel = this;
        for (int i = 0; i < baseSlimeLocation.Length; i++) {
            Instantiate(baseSlime, new Vector3(baseSlimeLocation[i].x, 0, baseSlimeLocation[i].y), baseSlime.transform.rotation);
            enemiesAlive++;
        }
        for (int i = 0; i < catSlimeLocation.Length; i++) {
            Instantiate(catSlime, new Vector3(catSlimeLocation[i].x, 0, catSlimeLocation[i].y), catSlime.transform.rotation);
            enemiesAlive++;
        }
        for (int i = 0; i < vikingSlimeLocation.Length; i++) {
            Instantiate(vikingSlime, new Vector3(vikingSlimeLocation[i].x, 0, vikingSlimeLocation[i].y), vikingSlime.transform.rotation);
            enemiesAlive++;
        }
    }

    public void EnemyKilled() {
        enemiesAlive--;
        if (enemiesAlive == 0) {
            for (int i = 0; i < Enviroment.Length; i++) {
                Destroy(Enviroment[i]);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            switch (currentLevel) {
                case 1: 
                    SceneManager.LoadScene("Level 1");
                    break;
                case 2:
                    SceneManager.LoadScene("Level 2");
                    break;
                case 3:
                    SceneManager.LoadScene("Level 3");
                    break;
            }
        }
    }
}
