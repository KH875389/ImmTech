using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class KeyLockSystem : MonoBehaviour
{
    public XRSocketInteractor lockSocket;     // The XR Socket where the key goes
    public string requiredKeyTag = "Key";      // Tag for the correct key object

    public Transform door;                    // Full door GameObject (Collider + Mesh)
    public Vector3 openPositionOffset = new Vector3(0, -2, 0); // How far the door moves
    public float openSpeed = 2f;

    public Rigidbody lockRigidbody;           // Rigidbody of the lock to fall
    public float lockFallForce = 1f;

    public AudioSource doorAudioSource;        // AudioSource component for door sound
    public AudioClip doorOpeningSound;         // The sound clip that plays when door moves

    private bool isUnlocked = false;
    private bool doorSoundPlayed = false;
    private Vector3 initialDoorPosition;
    private Vector3 targetDoorPosition;

    private void Start()
    {
        if (lockSocket == null || door == null || lockRigidbody == null)
        {
            Debug.LogError("❌ Missing reference! Check the inspector!");
            return;
        }

        lockSocket.selectEntered.AddListener(OnKeyInserted);

        initialDoorPosition = door.position;
        targetDoorPosition = initialDoorPosition + openPositionOffset;

        // Lock stays frozen until unlocked
        lockRigidbody.isKinematic = true;
    }

    private void OnDestroy()
    {
        lockSocket.selectEntered.RemoveListener(OnKeyInserted);
    }

    private void Update()
    {
        if (isUnlocked)
        {
            // Start playing door sound once when door begins to move
            if (!doorSoundPlayed && doorAudioSource != null && doorOpeningSound != null)
            {
                doorAudioSource.PlayOneShot(doorOpeningSound);
                doorSoundPlayed = true;
            }

            // Move the door down smoothly
            door.position = Vector3.Lerp(door.position, targetDoorPosition, Time.deltaTime * openSpeed);
        }
    }

    private void OnKeyInserted(SelectEnterEventArgs args)
    {
        GameObject insertedObject = args.interactableObject.transform.gameObject;

        if (insertedObject.CompareTag(requiredKeyTag))
        {
            Debug.Log("✅ Correct key inserted. Unlocking door!");
            Unlock();
        }
        else
        {
            Debug.Log("❌ Wrong key inserted!");
        }
    }

    private void Unlock()
    {
        isUnlocked = true;

        // Disable the socket so key stays locked in
        lockSocket.socketActive = false;

        // Make lock fall off
        lockRigidbody.isKinematic = false;
        lockRigidbody.useGravity = true;
        lockRigidbody.AddForce(Vector3.down * lockFallForce, ForceMode.Impulse);
    }
}
