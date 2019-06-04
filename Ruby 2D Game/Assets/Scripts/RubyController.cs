using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

//RAYCAST IS A LASERBEAM AND ANYTHING IT HITS WILL BE STORED
public class RubyController : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip walkClip;
    public AudioClip throwClip;

    public float speed = 5.0f;
    public int maxHealth = 100;
    public float timeInvincible = 2.0f;

    int currentHealth;
    bool isInvincible;
    float invincibleTimer;

    public GameObject projectilePrefab;

    public int health{get{return currentHealth;}}

    Rigidbody2D rigidbody2d;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        currentHealth = 100;
        audioSource = GetComponent<AudioSource>();
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
            //PlaySound(walkClip);
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        Vector2 position = rigidbody2d.position;

        position = position + move * speed * Time.deltaTime;

        rigidbody2d.MovePosition(position);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if(invincibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
            PlaySound(throwClip);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            speed = 10.0f;
            position = position + move * speed * Time.deltaTime;
        }
    }

    //time deltatime - the time it takes to get from one frame to another (subtract one frame to its previous frame)
    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
        }

        //doesnt exceed the maximum and never less than minimum
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}