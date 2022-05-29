using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;

namespace Haiku.TimerMod
{
    internal class Timer : MonoBehaviour
    {
        public enum TimerState
        {
            Waiting,
            Running,
            Paused,
            Loading,
            Stopped
        }
        private bool initiated = false;
        public static TimerState state;

        public static Stopwatch stopwatch = new Stopwatch();
        public static TimeSpan time { get => stopwatch.Elapsed; }
        public static TimeSpan[] pb = { TimeSpan.Zero, TimeSpan.Zero };


        void Start()
        {
            On.MainMenuManager.SelectSaveFile += gameStarted;
        }

        private void gameStarted(On.MainMenuManager.orig_SelectSaveFile orig, MainMenuManager self, string saveFile)
        {
            orig(self, saveFile);
            if (initiated) return;
            TimerUI.InitTimerUI();
            initiated = true;
        }

        void Update()
        {
            if (!initiated) return;
            if (state == TimerState.Waiting && Input.anyKeyDown && !Settings.ResetTimer.Value.IsPressed())
            {
                if (Timer.time != TimeSpan.Zero) Timer.stopwatch.Restart();
                else Timer.stopwatch.Start();
                Timer.state = Timer.TimerState.Running;
            }
            TimerUI.UpdateTimer();
        }

        public static void CalculateDeltaAndUpdateText()
        {
            string deltaString = "";
            TimeSpan delta = time - pb[0];
            deltaString += delta < TimeSpan.Zero ? "- " : "+ ";
            deltaString += TimerUI.formatTimer(delta.Duration());
            TimerUI.DeltaText.text = deltaString;
            TimerUI.DeltaGameObject.SetActive(true);
        }
    }
}
