using UnityEngine;

public class KillRat : MonoBehaviour
{
    // Define a list of tags for items that should be destroyed
    public string[] allowedTags;

    // Reference to the AudioSource component
    public AudioSource destructionSound;  // Drag the AudioSource component in the Inspector

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object has a tag that should be destroyed
        foreach (string tag in allowedTags)
        {
            if (other.CompareTag(tag))  // Check if the object has one of the allowed tags
            {
                // Play the destruction sound if an AudioSource is assigned
                if (destructionSound != null)
                {
                    destructionSound.Play();
                }

                // Destroy the object
                Destroy(other.gameObject);
                Debug.Log($"Item with tag {tag} destroyed in the kill box.");

                return; // Exit after destroying the first matching object
            }
        }
    }
}
