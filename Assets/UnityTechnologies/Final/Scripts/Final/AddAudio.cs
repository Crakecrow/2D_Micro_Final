using UnityEngine;

public class AddAudio : MonoBehaviour
{
    public AudioClip addAudio; // Public class to place audio in the inspector.
    private AudioSource audioSource;

    private void Start()
    {
        
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = addAudio; 
    }

// -----------------------Trigger Event Code----------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !audioSource.isPlaying)
        {
            Debug.Log("Player entered the trigger zone.");
            audioSource.Play(); 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && audioSource.isPlaying)
        {
            Debug.Log("Player exited the trigger zone.");
            audioSource.Stop();
        }
    }
}