using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // Add this namespace for TextMeshPro

public class RubyController : MonoBehaviour
{
    // ========= MOVEMENT =================
    public float speed = 4;
    public InputAction moveAction;

    // ======== HEALTH ==========
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;
    public Transform respawnPosition;
    public ParticleSystem hitParticle;

    // ======== PROJECTILE ==========
    public GameObject projectilePrefab;
    public InputAction launchAction;

    // ======== AUDIO ==========
    public AudioClip hitSound;
    public AudioClip shootingSound;

    // ======== HEALTH ==========
    public int health
    {
        get { return currentHealth; }
    }

    // ======== DIALOGUE ==========
    public InputAction dialogAction;

    // =========== MOVEMENT ==============
    private Rigidbody2D rigidbody2d;
    private Vector2 currentInput;

    // ======== HEALTH ==========
    private int currentHealth;
    private float invincibleTimer;
    private bool isInvincible;

    // ==== ANIMATION =====
    private Animator animator;
    private Vector2 lookDirection = new Vector2(1, 0);

    // ================= SOUNDS =======================
    private AudioSource audioSource;

    // ======== GAME OVER ==========
    public TextMeshProUGUI gameOverText; // Change this to TextMeshProUGUI
    private bool isGameOver = false;
    public AudioClip gameOverAudio; 

    // ======== SCORE ==========
    public int totalRobotsToFix = 5;  // Set this to the total number of robots
    private int fixedRobots = 0;

    // ======== COLLIDER ==========
    private Collider2D collider2d;

    void Start()
    {
        // =========== MOVEMENT ==============
        rigidbody2d = GetComponent<Rigidbody2D>();
        moveAction.Enable();

        // ======== HEALTH ==========
        invincibleTimer = -1.0f;
        currentHealth = maxHealth;

        // ==== ANIMATION =====
        animator = GetComponent<Animator>();

        // ==== AUDIO =====
        audioSource = GetComponent<AudioSource>();

        // ==== ACTIONS ====
        launchAction.Enable();
        dialogAction.Enable();

        launchAction.performed += LaunchProjectile;

        // ======== GAME OVER ==========
        gameOverText.enabled = false; // Ensure the Game Over text is initially hidden

        // ======== COLLIDER ==========
        collider2d = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
            return;
        }

        // ================= HEALTH ====================
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        // ============== MOVEMENT ======================
        Vector2 move = moveAction.ReadValue<Vector2>();

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        currentInput = move;

        // ============== ANIMATION =======================
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        // ======== DIALOGUE ==========
        if (dialogAction.WasPressedThisFrame())
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, 1 << LayerMask.NameToLayer("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    // Add dialogue interaction logic here
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (isGameOver) return;

        Vector2 position = rigidbody2d.position;

        position = position + currentInput * speed * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    // ===================== HEALTH ==================
    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;

            animator.SetTrigger("Hit");
            audioSource.PlayOneShot(hitSound);

            Instantiate(hitParticle, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        if (currentHealth == 0)
        {
            TriggerGameOver();
        }

        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }

    public void ChangeScore(int amount)
    {
        fixedRobots += amount;

        // Check if all robots are fixed
        if (fixedRobots >= totalRobotsToFix)
        {
            TriggerGameOver();
        }

        // Update UI or other game elements if needed
    }

    void Respawn()
    {
        ChangeHealth(maxHealth);
        transform.position = respawnPosition.position;
    }

    // =============== PROJECTILE ========================
    void LaunchProjectile(InputAction.CallbackContext context)
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
        audioSource.PlayOneShot(shootingSound);
    }

    // =============== SOUND ==========================
    // Allow to play a sound on the player sound source. Used by Collectible
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    // =============== GAME OVER ==========================
    void TriggerGameOver()
    {
        isGameOver = true;
        gameOverText.enabled = true;
        moveAction.Disable();
        launchAction.Disable();
        dialogAction.Disable();

        if (gameOverAudio != null)
    {
        audioSource.PlayOneShot(gameOverAudio);
    }

        // Disable the collider to prevent movement due to collisions
        collider2d.enabled = false;
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Re-enable the collider when restarting the game
        collider2d.enabled = true;
    }
}
