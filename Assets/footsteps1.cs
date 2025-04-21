using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class footsteps: MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] footstepClips;
    public float stepInterval = 0.5f; // Seconds between steps while moving

    private CharacterController controller;
    private float stepTimer;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (audioSource == null)
        {
            Debug.LogWarning("No AudioSource assigned, adding one automatically.");
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        stepTimer = stepInterval;
    }

    void Update()
    {
        if (controller.isGrounded && controller.velocity.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = stepInterval;
        }
    }

    void PlayFootstep()
    {
        if (footstepClips.Length == 0) return;

        int index = Random.Range(0, footstepClips.Length);
        audioSource.PlayOneShot(footstepClips[index]);
    }
}