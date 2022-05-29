using BepInEx;
using BepInEx.Configuration;
using Modding;
using System.Collections.Generic;
using UnityEngine;

namespace Haiku.TimerMod
{
    internal class Settings
    {
        public static ConfigEntry<KeyboardShortcut> CreateStartTrigger;
        public static ConfigEntry<KeyboardShortcut> CreateStopTrigger;

        public static ConfigEntry<KeyboardShortcut> ResetTimer;
        public static ConfigEntry<KeyboardShortcut> PauseTimer;
        public static ConfigEntry<KeyboardShortcut> ResumeTimer;

        public static ConfigEntry<KeyboardShortcut> RemoveLastPB;

        public static void initSettings(ConfigFile config)
        {
            CreateStartTrigger = config.Bind("Trigger", "Create Start Trigger", new KeyboardShortcut(KeyCode.Keypad1), ConfigManagerUtil.setPosition(2));
            CreateStopTrigger = config.Bind("Trigger", "Create Stop Trigger", new KeyboardShortcut(KeyCode.Keypad2), ConfigManagerUtil.setPosition(1));

            ResetTimer = config.Bind("Timer", "Reset Timer", new KeyboardShortcut(KeyCode.Keypad3), ConfigManagerUtil.setPosition(4));
            PauseTimer = config.Bind("Timer", "Pause Timer", new KeyboardShortcut(KeyCode.Keypad4), ConfigManagerUtil.setPosition(3));
            ResumeTimer = config.Bind("Timer", "Resume Timer", new KeyboardShortcut(KeyCode.Keypad5), ConfigManagerUtil.setPosition(2));

            RemoveLastPB = config.Bind("Timer", "Remove last PB", new KeyboardShortcut(KeyCode.Keypad6), ConfigManagerUtil.setPosition(1));



            //ConfigManagerUtil.createButton(config, YourPluginClass.doCoolThings, "CategoryName", "CoolName", "Does Cool things");
            //ConfigManagerUtil.createWebsiteButton(config, "https://github.com/YourRepo/YourPlugin");
        }
    }
}
