using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Puzzle : MonoBehaviour
{
    [Header("Sockets & Button")]
    public XRSocketInteractor[] sockets;
    public XRBaseInteractable button;

    [Header("Door Settings")]
    public Transform doorTransform;
    public float doorDropDistance = 1.5f;  // How far the door moves down
    public float openSpeed = 2f;
    public AudioSource doorAudioSource; // 🔊 Door movement sound

    [Header("Socket Effects")]
    public ParticleSystem correctPlacementEffect;
    public string[] correctTags; // ✅ Multiple allowed tags

    private bool isButtonUnlocked = false;
    private bool doorOpening = false;
    private Vector3 targetPosition;
    private Vector3 initialPosition;

    private void Start()
    {
        if (button == null || doorTransform == null)
        {
            Debug.LogError("❌ Button or Door Transform not assigned.");
            return;
        }

        initialPosition = doorTransform.localPosition;
        SetButtonInteractable(false);

        foreach (var socket in sockets)
        {
            if (socket != null)
            {
                socket.selectEntered.AddListener(OnItemPlaced); // Handle item placed
                socket.selectExited.AddListener(OnSocketExited); // Handle item removed
            }
        }

        button.selectEntered.AddListener(OnButtonPressed); // Handle button press
    }

    private void OnDestroy()
    {
        foreach (var socket in sockets)
        {
            if (socket != null)
            {
                socket.selectEntered.RemoveListener(OnItemPlaced);
                socket.selectExited.RemoveListener(OnSocketExited);
            }
        }

        if (button != null)
        {
            button.selectEntered.RemoveListener(OnButtonPressed);
        }
    }

    private void Update()
    {
        if (doorOpening)
        {
            doorTransform.localPosition = Vector3.MoveTowards(
                doorTransform.localPosition,
                targetPosition,
                Time.deltaTime * openSpeed
            );

            if (Vector3.Distance(doorTransform.localPosition, targetPosition) < 0.01f)
            {
                doorTransform.localPosition = targetPosition;
                doorOpening = false;
            }
        }
    }

    // Called when an item is placed into the socket (selectEntered)
    private void OnItemPlaced(SelectEnterEventArgs args)
    {
        string placedTag = args.interactableObject.transform.tag;

        if (IsCorrectTag(placedTag))
        {
            Debug.Log($"✅ Correct item placed: {placedTag}");

            if (correctPlacementEffect != null)
            {
                ParticleSystem effect = Instantiate(
                    correctPlacementEffect,
                    args.interactorObject.transform.position,
                    Quaternion.identity
                );
                effect.Play();
                Destroy(effect.gameObject, 2f);
            }
        }

        OnSocketUpdate();
    }

    // Called when an item is removed from the socket (selectExited)
    private void OnSocketExited(SelectExitEventArgs args)
    {
        OnSocketUpdate();
    }

    // Checks if all sockets are filled with items
    private void OnSocketUpdate()
    {
        if (!isButtonUnlocked && AllSocketsFilled())
        {
            isButtonUnlocked = true;
            SetButtonInteractable(true);
            Debug.Log("✔ All sockets filled. Button is now active!");
        }
        else if (isButtonUnlocked && !AllSocketsFilled())
        {
            isButtonUnlocked = false;
            SetButtonInteractable(false);
            Debug.Log("❌ One or more sockets are now empty. Button disabled.");
        }
    }

    // Called when the button is pressed to open the door
    private void OnButtonPressed(SelectEnterEventArgs args)
    {
        if (isButtonUnlocked && !doorOpening)
        {
            Debug.Log("🔓 Button pressed. Lowering door!");
            targetPosition = initialPosition + new Vector3(0, -doorDropDistance, 0);
            doorOpening = true;

            if (doorAudioSource != null)
            {
                doorAudioSource.Play(); // 🔊 Play sound
            }
        }
    }

    // Check if all sockets have objects placed inside
    private bool AllSocketsFilled()
    {
        foreach (var socket in sockets)
        {
            if (socket != null && !socket.hasSelection)
                return false;
        }
        return true;
    }

    // Enables or disables the button interactable state
    private void SetButtonInteractable(bool isEnabled)
    {
        button.interactionLayers = isEnabled
            ? InteractionLayerMask.GetMask("Default")
            : 0;
    }

    // Check if the placed item has a correct tag
    private bool IsCorrectTag(string tagToCheck)
    {
        foreach (string correctTag in correctTags)
        {
            if (tagToCheck == correctTag)
                return true;
        }
        return false;
    }
}