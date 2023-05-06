using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamageable
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer rend;
    [SerializeField] Animator anim;
    [SerializeField] Rigidbody rb;
    [SerializeField] BoxCollider attackBox;

    [Header("----- Enemy Stats -----")]
    [Range(0, 500)] public int HP;
    [Range(0, 10)][SerializeField] int playerFaceSpeed;
    [Range(1, 20)][SerializeField] float speedChase;
    [SerializeField] float invincibilityTimer;

    [Header("----- Weapons Stats -----")]
    [Range(0.1f, 5)][SerializeField] float attackRate;
    [Range(1, 10)][SerializeField] int damage;

    Vector3 playerDir;
    bool isAttacking;
    private int HPOrig;

    float stoppingDistanceOrig;
    float damageTimer;
    public float stunTimer;

    Vector3 raycastPos;

    // Start is called before the first frame update
    void Start()
    {
        //anim = GetComponentInChildren<Animator>();
        rend = GetComponentInChildren<SkinnedMeshRenderer>();
        stoppingDistanceOrig = agent.stoppingDistance;
        HPOrig = HP;
    }

    // Update is called once per frame
    void Update()
    {
        if (damageTimer > 0)
            damageTimer -= Time.deltaTime;

        if (stunTimer > 0) {
            stunTimer -= Time.deltaTime;
            agent.speed = 0;
        } else {
            agent.speed = speedChase;
            raycastPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            if (agent.isActiveAndEnabled /*&& !anim.GetBool("Dead")*/) {

                ApproachPlayer();

                playerDir = GameManager.instance.player.transform.position - raycastPos;
            }
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
        if (!isAttacking) {
            isAttacking = true;
            agent.isStopped = true;
            anim.SetTrigger("Attack");
            attackBox.enabled = true;
            yield return new WaitForSeconds(0.1f);
            attackBox.enabled = false;
            yield return new WaitForSeconds(1f);

            isAttacking = false;
        }
        agent.isStopped = false;
    }

    public void TakeDamage(int _damage, float _stun) {
        stunTimer += _stun;
        if (damageTimer <= 0) {
            damageTimer = invincibilityTimer;
            if (!anim.GetBool("Dead")) {
                HP -= _damage;

                if (HP > 0) {
                    anim.SetInteger("DamageType", 1);
                    anim.SetTrigger("Damage");
                    StartCoroutine(FlashColor());
                } else if (HP <= 0) {
                    StartCoroutine(Die());
                }
            }
        }
    }

    IEnumerator FlashColor() {
        rend.material.color = Color.red;
        agent.speed = 0;
        yield return new WaitForSeconds(0.5f);
        agent.speed = speedChase;
        agent.stoppingDistance = 0;
        rend.material.color = Color.white;
    }

    public IEnumerator Die() {
        //GameManager.instance.currentRoom.GetComponent<LevelSpawner>().EnemyKilled();
        anim.SetBool(("Dead"), true);
        agent.enabled = false;
        GetComponent<EnemyAI>().enabled = false;

        foreach (Collider col in GetComponents<Collider>())
            col.enabled = false;

        foreach (Collider child in GetComponentsInChildren<Collider>())
            child.enabled = false;

        GameManager.instance.currentLevel.EnemyKilled();

        yield return new WaitForSeconds(0.5f);
        rb.useGravity = true;
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<IDamageable>(out IDamageable obj) && other.CompareTag("Player") && other.isTrigger == false) {
            obj.TakeDamage(damage, 0);
        }
    }
}
