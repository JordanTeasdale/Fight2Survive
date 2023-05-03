using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamageable
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer rend;
    [SerializeField] Animator anim;

    [Header("----- Enemy Stats -----")]
    [Range(0, 500)] public int HP;
    [Range(0, 10)][SerializeField] int playerFaceSpeed;
    [Range(1, 20)][SerializeField] float speedChase;

    [Header("----- Weapons Stats -----")]
    [Range(0.1f, 5)][SerializeField] float attackRate;
    [Range(1, 10)][SerializeField] int damage;

    Vector3 playerDir;
    bool isAttacking;
    private int HPOrig;

    float stoppingDistanceOrig;

    Vector3 raycastPos;

    // Start is called before the first frame update
    void Start()
    {
        stoppingDistanceOrig = agent.stoppingDistance;
        HPOrig = HP;
    }

    // Update is called once per frame
    void Update()
    {
        raycastPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        if (agent.isActiveAndEnabled /*&& !anim.GetBool("Dead")*/) {
            
            ApproachPlayer();

            playerDir = GameManager.instance.player.transform.position - raycastPos;
        }
    }

    void ApproachPlayer() {

                agent.SetDestination(GameManager.instance.player.transform.position);
                agent.stoppingDistance = stoppingDistanceOrig;
                agent.speed = speedChase;
                FacePlayer();

                if (agent.remainingDistance <= agent.stoppingDistance) {
                    StartCoroutine(Attack());
                }
    }

    void FacePlayer() {
        if (agent.remainingDistance <= agent.stoppingDistance) {
            playerDir.y = 0;
            Quaternion rotation = Quaternion.LookRotation(playerDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * playerFaceSpeed);
        }
    }

    IEnumerator Attack() {


        yield return null;
    }

    public void TakeDamage(int _damage) {
        if (!anim.GetBool("Dead") && agent.isActiveAndEnabled) {
            HP -= damage;

            if (HP > 0) {
                //anim.SetTrigger("Damage");
                StartCoroutine(FlashColor());
            } else if (HP <= 0) {
                Die();
            }
        }
    }

    IEnumerator FlashColor() {
        rend.material.color = Color.red;
        agent.speed = 0;
        yield return new WaitForSeconds(0.5f);
        agent.speed = speedChase;
        if (HP > 0)
            agent.SetDestination(GameManager.instance.player.transform.position);
        agent.stoppingDistance = 0;
        rend.material.color = Color.white;
    }

    public void Die() {
        //GameManager.instance.currentRoom.GetComponent<LevelSpawner>().EnemyKilled();
        //anim.SetBool(("Dead"), true);
        agent.enabled = false;
        GetComponent<EnemyAI>().enabled = false;

        foreach (Collider col in GetComponents<Collider>())
            col.enabled = false;

        foreach (Collider child in GetComponentsInChildren<Collider>())
            child.enabled = false;

        //GetComponent<Animator>().enabled = false;
    }
}
