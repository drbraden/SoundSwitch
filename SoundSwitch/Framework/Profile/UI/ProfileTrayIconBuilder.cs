﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SoundSwitch.Common.Framework.Icon;
using SoundSwitch.Framework.Profile.Trigger;
using SoundSwitch.Model;
using SoundSwitch.Properties;

namespace SoundSwitch.Framework.Profile.UI
{
    public class ProfileTrayIconBuilder
    {
        private ProfileManager ProfileManager => AppModel.Instance.ProfileManager;

        private IAudioDeviceLister AudioDeviceLister => AppModel.Instance.ActiveAudioDeviceLister;

        /// <summary>
        /// Get the menu items for profile that needs to be shown in the menu
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ToolStripMenuItem> GetMenuItems()
        {
            return ProfileManager.Profiles
                                 .Where(profile => profile.Triggers.Any(trigger => trigger.Type == TriggerFactory.Enum.TrayMenu))
                                 .Select(BuildMenuItem);
        }

        private ProfileToolStripMenuItem BuildMenuItem(Profile profile)
        {
            Image image = null;
            try
            {
                var appTrigger = profile.Triggers.FirstOrDefault(trigger => trigger.ApplicationPath != null);
                if (appTrigger != null)
                {
                    image = IconExtractor.Extract(appTrigger.ApplicationPath, 0, false).ToBitmap();
                }
            }
            catch (Exception)
            {
                // ignored
            }

            foreach (var wrapper in profile.Devices)
            {
                if (image != null)
                    break;

                try
                {
                    var device = AudioDeviceLister.PlaybackDevices.FirstOrDefault(info => info.Equals(wrapper.DeviceInfo));
                    image = device?.SmallIcon.ToBitmap();
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            image ??= Resources.default_profile_image;

            return new ProfileToolStripMenuItem(profile, image, profileClicked => ProfileManager.SwitchAudio(profileClicked));
        }
    }
}