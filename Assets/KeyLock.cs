using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class KeyLock : MonoBehaviour
{
    public XRSocketInteractor keySocket; // Assign in Inspector
    public GameObject door; // The door to open
    public Vector3 openRotation = new Vector3(0, 90, 0); // Door open rotation
    public float openSpeed = 2f;

    private bool isUnlocked = false;
    private Quaternion targetRotation;
    private bool doorOpening = false;

    void Start()
    {
        if (keySocket == null || door == null)
        {
            Debug.LogError("Key Socket or Door not assigned.");
            return;
        }

        keySocket.selectEntered.AddListener(OnKeyInserted);
    }

    void Update()
    {
        if (doorOpening)
        {
            door.transform.rotation = Quaternion.Slerp(door.transform.rotation, targetRotation, Time.deltaTime * openSpeed);

            if (Quaternion.Angle(door.transform.rotation, targetRotation) < 0.5f)
            {
                door.transform.rotation = targetRotation;
                doorOpening = false;
            }
        }
    }

    void OnKeyInserted(SelectEnterEventArgs args)
    {
        if (isUnlocked) return;

        // Check if the inserted object has the correct tag
        if (args.interactableObject.transform.CompareTag("Key"))
        {
            Debug.Log("Correct key inserted. Unlocking...");
            isUnlocked = true;

            // Trigger door opening
            targetRotation = Quaternion.Euler(door.transform.localEulerAngles + openRotation);
            doorOpening = true;

            // Optionally, disable the socket to prevent further interactions
            keySocket.socketActive = false;
        }
        else
        {
            Debug.Log("Incorrect object inserted.");
        }
    }
}
