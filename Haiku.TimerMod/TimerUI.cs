using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Modding;
using UnityEngine.UI;

namespace Haiku.TimerMod
{
    internal class TimerUI
    {
        private static Text timerText;
        public static GameObject DeltaGameObject;
        public static Text DeltaText;
        public static Text PBText;
        public static void InitTimerUI()
        {
            GameObject timerCanvas = CanvasUtil.CreateCanvas(1);
            timerCanvas.name = "Timer Canvas";
            timerCanvas.transform.SetParent(TimerMod.TimerGameObject.transform);
            timerText = CanvasUtil.CreateTextPanel(timerCanvas, "0:00.000", 12, TextAnchor.MiddleCenter,
                new CanvasUtil.RectData(new Vector2(0, 0), new Vector2(-25, -100), new Vector2(0, 0), new Vector2(2, 1)), CanvasUtil.GameFont).GetComponent<Text>();
            timerText.raycastTarget = false;

            PBText = CanvasUtil.CreateTextPanel(timerCanvas, "PB 0:00.000", 10, TextAnchor.MiddleCenter,
                new CanvasUtil.RectData(new Vector2(0, 0), new Vector2(-27, -90), new Vector2(0, 0), new Vector2(2, 1)), CanvasUtil.GameFont).GetComponent<Text>();
            PBText.raycastTarget = false;

            DeltaGameObject = CanvasUtil.CreateTextPanel(timerCanvas, "0:00.000", 8, TextAnchor.MiddleCenter,
                new CanvasUtil.RectData(new Vector2(0,0), new Vector2(-22, -80), new Vector2(0, 0), new Vector2(2, 1)), CanvasUtil.GameFont);
            DeltaText = DeltaGameObject.GetComponent<Text>();
            DeltaText.raycastTarget = false;
            DeltaGameObject.SetActive(false);
        }

        public static string formatTimer(TimeSpan time)
        {
            return string.Format("{0}:{1:D2}.{2:D3}",
                Math.Floor(time.TotalMinutes),
                time.Seconds,
                time.Milliseconds);
        }

        public static void UpdateTimer()
        {
            if (Timer.pb[0] != TimeSpan.Zero)
            {
                PBText.text = "PB " + formatTimer(Timer.pb[0]);
            } else
            {
                DeltaGameObject.SetActive(false);
            }
            timerText.text = formatTimer(Timer.time);
        }
    }
}
