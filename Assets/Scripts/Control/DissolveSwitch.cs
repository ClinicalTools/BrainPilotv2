using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class DissolveSwitch : MonoBehaviour
    {
        public GameObject tool;
        // XrayCone object assigned
        public GameObject offBox;
        // XrayHolder object assigned
        public GameObject HandAnchor;
        // ActiveHandAnchor object assigned
        private bool xray;

    // Start is called before the first frame update
        void Start()
        {
            xray = true;
        }

        void Toggle_Xray()
        {
            if (xray == true) {
                xray = false;
                tool.transform.SetParent(offBox.transform, false);
            } else {
                xray = true;
                tool.transform.SetParent(HandAnchor.transform, false);
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