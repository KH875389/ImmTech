using UnityEngine;

public class VisualFeedback : MonoBehaviour
{
    public string correctKeyTag = "Key";
    public Renderer lockRenderer;
    public Material baseMaterial;
    public Material glowMaterial;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(correctKeyTag))
        {
            SetGlow(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(correctKeyTag))
        {
            SetGlow(false);
        }
    }

    private void SetGlow(bool glow)
    {
        if (lockRenderer != null)
        {
            lockRenderer.material = glow ? glowMaterial : baseMaterial;
        }
    }
}
