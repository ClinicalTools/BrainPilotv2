using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedDissolve_Example {
    public class DissolveSwitch : MonoBehaviour
    {
        Controller_Mask_Cone maskController;
        bool xray;
        //Renderer m_BrainRenderer;
        //public Material[] materials;
    // Start is called before the first frame update
        void Start()
        {
            xray = true;
            maskController = GetComponent<Controller_Mask_Cone>();

            maskController.spotLight2.gameObject.SetActive(true);
            maskController.spotLight2.gameObject.SetActive(true);
            maskController.spotLight3.gameObject.SetActive(true);
            //maskController.spotLight4.gameObject.SetActive(false);
        }

        void Toggle_Xray()
        {
            if (xray == true) {
                maskController.spotLight2.gameObject.SetActive(false);
                maskController.spotLight3.gameObject.SetActive(false);
                xray = false;
            } else {
                maskController.spotLight2.gameObject.SetActive(true);
                maskController.spotLight3.gameObject.SetActive(true);
                xray = true;
            }

        }

    // Update is called once per frame
        void Update()
        {
            if(OVRInput.GetDown(OVRInput.Button.One)) {
                Toggle_Xray();
            }
        }
}
}