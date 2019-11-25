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

namespace LeiaLoft
{
    /// <summary>
    /// Default LeiaDevice that makes no real connection to any device
    /// </summary>
    class OfflineEmulationLeiaDevice : AbstractLeiaDevice
    {
        public OfflineEmulationLeiaDevice(string stubName)
        {
            this.Debug("ctor()");
            _profileStubName = stubName;
        }

		public override void SetBacklightMode(int modeId)
		{
			// this method was left blank intentionally
		}

		public override void SetBacklightMode(int modeId, int delay)
		{
			// this method was left blank intentionally
		}

		public override void RequestBacklightMode(int modeId)
		{
			// this method was left blank intentionally
		}

		public override void RequestBacklightMode(int modeId, int delay)
		{
			// this method was left blank intentionally
		}

		public override int GetBacklightMode()
		{
			return 3;
		}


		public override DisplayConfig GetDisplayConfig()
		{
			var displayConfig = base.GetDisplayConfig();

			return displayConfig;
		}
    }
}