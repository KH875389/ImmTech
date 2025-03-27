using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VRSocketRotationDoor : MonoBehaviour
{
    [System.Serializable]
    public class SocketRequirement
    {
        public XRSocketInteractor socket; // Reference to the socket
        public string RedGem; // Tag of the required object
    }

    [Header("Socket Settings")]
    public List<SocketRequirement> List; // List of sockets and their required objects

    [Header("Door Settings")]
    public GameObject door; // Door GameObject to rotate
    public Vector3 openRotation; // Target rotation when the door opens
    public float doorOpenSpeed = 2f; // How fast the door rotates
    private bool isDoorOpen = false; // Prevents multiple openings

    void Update()
    {
        // Check if all conditions are met and door is not open yet
        if (!isDoorOpen && AllRequirementsMet())
        {
            OpenDoor();
        }
    }

    // Check if all sockets have the correct objects
    private bool AllRequirementsMet()
    {
        foreach (var requirement in List)
        {
            if (!IsCorrectObjectInSocket(requirement))
            {
                return false;
            }
        }
        return true;
    }

    // Check if the correct object is placed in the socket
    private bool IsCorrectObjectInSocket(SocketRequirement requirement)
    {
        IXRSelectInteractable interactable = requirement.socket.GetOldestInteractableSelected();
        if (interactable != null)
        {
            GameObject placedObject = interactable.transform.gameObject;
            return placedObject.CompareTag(requirement.RedGem);
        }
        return false;
    }

    // Open the door by rotating it
    private void OpenDoor()
    {
        isDoorOpen = true; // Mark the door as open
        StartCoroutine(RotateDoorRoutine());
    }

    // Coroutine to smoothly rotate the door
    private System.Collections.IEnumerator RotateDoorRoutine()
    {
        Quaternion startRotation = door.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(openRotation); // Target rotation
        float elapsedTime = 0f;

        // Gradually rotate the door over time
        while (elapsedTime < 1f)
        {
            door.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime);
            elapsedTime += Time.deltaTime * doorOpenSpeed;
            yield return null;
        }

        // Ensure the final rotation locks into place
        door.transform.rotation = targetRotation;
    }
}