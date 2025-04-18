using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class puzzle : MonoBehaviour
{
    public XRSocketInteractor[] sockets;
    public XRBaseInteractable button;
    public Transform doorTransform; // The door GameObject's transform
    public Vector3 openRotation = new Vector3(0, 90, 0); // How the door should rotate
    public float openSpeed = 2f;

    private bool isButtonUnlocked = false;
    private bool doorOpening = false;
    private Quaternion targetRotation;

    private void Start()
    {
        if (button == null || doorTransform == null)
        {
            Debug.LogError("Button or Door Transform not assigned.");
            return;
        }

        SetButtonInteractable(false);

        foreach (var socket in sockets)
        {
            if (socket != null)
            {
                socket.selectEntered.AddListener((args) => OnSocketUpdate());
                socket.selectExited.AddListener((args) => OnSocketUpdate());
            }
        }

        button.selectEntered.AddListener((args) => OnButtonPressed());
    }

    private void OnDestroy()
    {
        foreach (var socket in sockets)
        {
            if (socket != null)
            {
                socket.selectEntered.RemoveAllListeners();
                socket.selectExited.RemoveAllListeners();
            }
        }

        button.selectEntered.RemoveAllListeners();
    }

    private void Update()
    {
        if (doorOpening)
        {
            doorTransform.rotation = Quaternion.Slerp(doorTransform.rotation, targetRotation, Time.deltaTime * openSpeed);

            // Optional: stop when close to target
            if (Quaternion.Angle(doorTransform.rotation, targetRotation) < 0.5f)
            {
                doorTransform.rotation = targetRotation;
                doorOpening = false;
            }
        }
    }

    private void OnSocketUpdate()
    {
        if (!isButtonUnlocked && AllSocketsFilled())
        {
            SetButtonInteractable(true);
            isButtonUnlocked = true;
            Debug.Log("✔ All sockets filled. Button is now active!");
        }
    }

    private void OnButtonPressed()
    {
        if (isButtonUnlocked && !doorOpening)
        {
            Debug.Log("🔓 Button pressed. Opening door!");
            targetRotation = Quaternion.Euler(doorTransform.localEulerAngles + openRotation);
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