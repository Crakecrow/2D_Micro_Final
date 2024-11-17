using System;
using UnityEngine;

/// <summary>
/// This class handles Enemy behavior. It makes them walk back & forth as long as they aren't fixed, and then just idle
/// without being able to interact with the player anymore once fixed.
/// </summary>
public class Enemy : MonoBehaviour
{
    // ====== ENEMY MOVEMENT ========
    public float speed;
    public float timeToChange;
    public bool horizontal;

    public GameObject smokeParticleEffect;
    public ParticleSystem fixedParticleEffect;

    public AudioClip hitSound;
    public AudioClip fixedSound;

    private Rigidbody2D rigidbody2d;
    private float remainingTimeToChange;
    private Vector2 direction = Vector2.right;
    private bool repaired = false;

    // ===== ANIMATION ========
    private Animator animator;

    // ================= SOUNDS =======================
    private AudioSource audioSource;

    // ===== RUBY CONTROLLER =====
    private RubyController rubyController;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        remainingTimeToChange = timeToChange;
        direction = horizontal ? Vector2.right : Vector2.down;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Find the RubyController
        GameObject rubyControllerObject = GameObject.FindWithTag("Player");
        if (rubyControllerObject != null)
        {
            rubyController = rubyControllerObject.GetComponent<RubyController>();
            if (rubyController == null)
            {
                Debug.LogError("RubyController component not found on the player object.");
            }
        }
        else
        {
            Debug.LogError("Player object not found in the scene.");
        }
    }

    void Update()
    {
        if (repaired)
            return;

        remainingTimeToChange -= Time.deltaTime;

        if (remainingTimeToChange <= 0)
        {
            remainingTimeToChange += timeToChange;
            direction *= -1;
        }

        animator.SetFloat("ForwardX", direction.x);
        animator.SetFloat("ForwardY", direction.y);
    }

    void FixedUpdate()
    {
        if (!repaired)
        {
            rigidbody2d.MovePosition(rigidbody2d.position + direction * speed * Time.deltaTime);
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (repaired)
            return;

        RubyController controller = other.collider.GetComponent<RubyController>();

        if (controller != null)
        {
            controller.ChangeHealth(-1);
        }
    }

    public void Fix()
    {
        animator.SetTrigger("Fixed");
        repaired = true;

        smokeParticleEffect.SetActive(false);
        Instantiate(fixedParticleEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);

        // We don't want that enemy to react to the player or bullet anymore, remove its Rigidbody from the simulation
        rigidbody2d.simulated = false;

        audioSource.Stop();
        audioSource.PlayOneShot(hitSound);
        audioSource.PlayOneShot(fixedSound);

        // Update the score in RubyController
        if (rubyController != null)
        {
            rubyController.ChangeScore(1);
        }
    }
}
