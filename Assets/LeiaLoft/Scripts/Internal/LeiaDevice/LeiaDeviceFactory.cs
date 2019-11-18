/****************************************************************
*
* Copyright 2019 © Leia Inc.  All rights reserved.
*
* NOTICE:  All information contained herein is, and remains
* the property of Leia Inc. and its suppliers, if any.  The
* intellectual and technical concepts contained herein are
* proprietary to Leia Inc. and its suppliers and may be covered
* by U.S. and Foreign Patents, patents in process, and are
* protected by trade secret or copyright law.  Dissemination of
* this information or reproduction of this materials strictly
* forbidden unless prior written permission is obtained from
* Leia Inc.
*
****************************************************************
*/
using UnityEngine;

namespace LeiaLoft
{
    /// <summary>
    /// Returns proper ILeiaDevice implementation based on platform and connectivity
    /// </summary>
    public class LeiaDeviceFactory
    {
		private const string DevicesAlreadyAdded = "Custom leia device already added.";
        private AbstractLeiaDevice _customDevice;

        /// <summary>
        /// Returns true if registration was successful (no name conflict)
        /// </summary>
        public bool RegisterLeiaDevice(AbstractLeiaDevice device)
        {
            if (_customDevice != null)
            {
                this.Warning(DevicesAlreadyAdded);
                return false;
            }
            this.Debug("Registered leia device: " + device.GetType());

            _customDevice = device;
            return true;
        }

        public void UnregisterLeiaDevice()
        {
            this.Debug("UnregisterLeiaDevice");
            _customDevice = null;
        }

        public ILeiaDevice GetDevice(string stubName)
        {
            this.Debug(string.Format("GetDevice( {0})", stubName));
            if (_customDevice != null)
            {
                _customDevice.SetProfileStubName(stubName);
                return _customDevice;
            }

            return new OfflineEmulationLeiaDevice(stubName);
        }
    }
}
