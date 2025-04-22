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
                socket.selectEntered.AddListener((args) => OnSocketUpdate());
                socket.selectExited.AddListener((args) => OnSocketUpdate());
            }
        }

        button.selectEntered.AddListener(OnButtonPressed);
    }

    private void OnDestroy()
    {
        foreach (var socket in sockets)
        {
            if (socket != null)
            {
                socket.selectEntered.AddListener((args) => OnSocketUpdate());
                socket.selectExited.AddListener((args) => OnSocketUpdate());
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

    private void OnButtonPressed(SelectEnterEventArgs args)
    {
        if (isButtonUnlocked && !doorOpening)
        {
            Debug.Log("🔓 Button pressed. Lowering door!");
            targetPosition = initialPosition + new Vector3(0, -doorDropDistance, 0);
            doorOpening = true;
        }
    }

    private bool AllSocketsFilled()
    {
        foreach (var socket in sockets)
        {
            if (socket != null && !socket.hasSelection)
                return false;
        }
        return true;
    }

    private void SetButtonInteractable(bool isEnabled)
    {
        button.interactionLayers = isEnabled
            ? InteractionLayerMask.GetMask("Default")
            : 0;
    }
}