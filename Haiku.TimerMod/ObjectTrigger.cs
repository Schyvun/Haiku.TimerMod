using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Haiku.TimerMod
{
    public enum TypeOfTrigger
    {
        Start,
        End
    }

    public class ObjectTrigger : MonoBehaviour
    {
        public TypeOfTrigger Type;
        public int SceneIndex;

        void OnTriggerEnter2D(Collider2D col)
        {
            // TODO: Entangle this cba rn

            if (col.gameObject.CompareTag("Player"))
            {
                if (Type == TypeOfTrigger.Start)
                {
                    Timer.state = Timer.TimerState.Waiting;
                } else
                {
                    Timer.stopwatch.Stop();
                    if (Timer.state == Timer.TimerState.Running)
                    {
                        if (Timer.pb[0] != TimeSpan.Zero)
                        {
                            Timer.CalculateDeltaAndUpdateText();

                            if (Timer.time < Timer.pb[0])
                            {
                                Timer.pb[1] = Timer.pb[0];
                                Timer.pb[0] = Timer.time;
                            }
                        }
                        else
                        {
                            Timer.pb[0] = Timer.time;
                        }
                        Timer.state = Timer.TimerState.Stopped;
                    }
                }
            }
        }

        public Sprite createSprite(Vector2 size)
        {
            Texture2D tx2d = new Texture2D((int)size.x, (int)size.y);
            Color currColor = Type == TypeOfTrigger.Start ? Color.cyan : Color.red;
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    if (i <= 6 || j <= 6 || i >= size.x - 7 || j >= size.y - 7)
                    {
                        if (currColor == Color.red) tx2d.SetPixel(i, j, Color.black);
                        else tx2d.SetPixel(i, j, Color.white);
                    } else
                    {
                        tx2d.SetPixel(i, j, currColor);
                    }
                }
            }
            tx2d.Apply();
            return Sprite.Create(tx2d, new Rect(0, 0, size.x, size.y), new Vector2(0.5f, 0.5f));
        }

    }
}
