using UnityEngine;

public class CatAnimationController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("Cat-1-Idle_0"); // Ensure it matches the name of your idle animation clip
    }
}
