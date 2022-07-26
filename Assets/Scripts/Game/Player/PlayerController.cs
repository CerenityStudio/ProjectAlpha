using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    // each player unique id
    [HideInInspector]
    public int id;

    [Header("Info")]
    public float moveSpeed;
    public int gold;
    public int currHP;
    public int maxHP;
    public bool dead;

    [Header("Attack")]
    public float attackRange;
    public float attackDelay;
    public float lastAttackTime;
    public int damage;

    [Header("Components")]
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public Player photonPlayer;
    public Animator weaponAnim;

    // local player
    public static PlayerController me;

    [PunRPC]
    public void Initialized(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;

        GameManager.instance.players[id - 1] = this;

        if (player.IsLocal)
        {
            me = this;
        }
        else
        {
            rb.isKinematic = false;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Move();

        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime > attackDelay)
        {
            Attack();
        }

        float mouseX = (Screen.width / 2) - Input.mousePosition.x;
        if (mouseX < 0)
        {
            weaponAnim.transform.parent.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            weaponAnim.transform.parent.localScale = new Vector3(-1, 1, 1);
        }
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        rb.velocity = new Vector2(x, y) * moveSpeed;
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        Vector3 dir = (Input.mousePosition - Camera.main.ScreenToWorldPoint(transform.position).normalized);

        RaycastHit2D hit = Physics2D.Raycast(transform.position + dir, dir, attackRange);

        if (hit.collider !=null && hit.collider.gameObject.CompareTag("Enemy"))
        {

        }

        weaponAnim.SetTrigger("Attack");
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        currHP -= damage;

        if (currHP <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DamageFlash());

            IEnumerator DamageFlash()
            {
                sr.color = Color.red;
                yield return new WaitForSeconds(0.04f);
                sr.color = Color.white;
            }
        }
    }

    internal void Die()
    {
        dead = true;
        rb.isKinematic = true;

        transform.position = new Vector3(0, 99, 0);

        Vector3 spawnPos = GameManager.instance.spawnPoints[Random.Range(0, GameManager.instance.spawnPoints.Length)].position;

        StartCoroutine(Spawn(spawnPos, GameManager.instance.respawnTime));
    }

    IEnumerator Spawn(Vector3 spawnPos, float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);

        dead = false;
        transform.position = spawnPos;
        currHP = maxHP;
        rb.isKinematic = false;


    }

    [PunRPC]
    internal void Heal (int amountToHeal)
    {
        currHP = Mathf.Clamp(currHP + amountToHeal, 0, maxHP);


    }

    [PunRPC]
    internal void GetGold(int goldToGive)
    {
        gold += goldToGive;


    }
}
