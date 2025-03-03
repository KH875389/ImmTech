using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR; 
public class HMDManager : MonoBehaviour
{
    [SerializeField] GameObject xrPlayer;
    [SerializeField] GameObject fpsPlayer;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Using Device: " + XRSettings.loadedDeviceName);
        if (XRSettings.isDeviceActive)
        {
            fpsPlayer.SetActive(false);
            xrPlayer.SetActive(true);
        }
        else
        {
            xrPlayer.SetActive(false);
            fpsPlayer.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
