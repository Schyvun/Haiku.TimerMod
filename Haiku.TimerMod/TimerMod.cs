using BepInEx;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using MonoMod.RuntimeDetour;
using System.Reflection;

namespace Haiku.TimerMod
{
    [BepInPlugin("haiku.timermod", "Timer Mod", "1.0.0.0")]
    [BepInDependency("haiku.mapi", "1.0.0.1")]
    public sealed class TimerMod : BaseUnityPlugin
    {
        public static GameObject TimerGameObject;
        public static GameObject startObject;
        public static ObjectTrigger startObjectTriggerComponent;
        public static GameObject endObject;
        public static ObjectTrigger endObjectTriggerComponent;
        //private MonoMod.RuntimeDetour.Hook _hook;
        //private static MethodInfo _methodToHook = typeof(SceneManager).GetMethod(nameof(SceneManager.LoadSceneAsync), new[] { typeof(int) });

        //private static MethodInfo _myOwnMethod = typeof(TimerMod).GetMethod(nameof(TimerMod.hookedAsyncSceneLoading));

        void Awake()
        {
            TimerGameObject = new GameObject();
            TimerGameObject.name = "Timer Mod";
            TimerGameObject.transform.SetParent(gameObject.transform);

            Settings.initSettings(Config);

            Timer.state = Timer.TimerState.Stopped;
            gameObject.AddComponent<Timer>();

            SceneManager.sceneLoaded += onSceneLoaded;
        }

        #region DeprecatedLoadRemover
        void Start()
        {
            //if (_methodToHook is null)
            //{
            //    Debug.LogError("methodToHook is null");
            //    return;
            //}
            //if (_myOwnMethod is null)
            //{
            //    Debug.LogError("myownmethod is null");
            //    return;
            //}
            //_hook = new MonoMod.RuntimeDetour.Hook(_methodToHook, _myOwnMethod);
        }
        /* Pause Timer during Async Loads:
           I'm hooking into the Method "LoadSceneAsync(int Scene)" since the game always calls that method, now Unity just forwards that method with a parameter
           LoadSceneMode, so I'm just calling that Method afterwards to have control over the Load Progress, so I basically make LoadSceneAsync(int Scene) not do anything
           and just continue with LoadSceneAsync(int Scene, LoadSceneParameters param). */
        public static AsyncOperation hookedAsyncSceneLoading(Func<int, AsyncOperation> orig, int SceneBuildIndex) {
            print("Before Loading");
            if (startObject != null)
            {
                if (startObjectTriggerComponent.SceneIndex == SceneBuildIndex) startObject.SetActive(true);
                else startObject.SetActive(false);
            }
            if (endObject != null)
            {
                if (endObjectTriggerComponent.SceneIndex == SceneBuildIndex) endObject.SetActive(true);
                else endObject.SetActive(false);
            }
            if (Timer.state == Timer.TimerState.Running)
            {
                Timer.stopwatch.Stop();
                Timer.state = Timer.TimerState.Loading;
            }
            LoadSceneParameters parameters = new LoadSceneParameters(LoadSceneMode.Single);
            GameManager.instance.StartCoroutine(LoadYourAsyncScene(SceneBuildIndex, parameters));
            return null;
        }

        public static IEnumerator LoadYourAsyncScene(int SceneBuildIndex, LoadSceneParameters parameters)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneBuildIndex, parameters);

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            print("After Loading");
            if (Timer.state == Timer.TimerState.Loading)
            {
                Timer.stopwatch.Start();
                Timer.state = Timer.TimerState.Running;
            }
        }
        #endregion
        private void onSceneLoaded(Scene SceneLoaded, LoadSceneMode SceneLoadMode)
        {
            if (startObject != null)
            {
                if (startObjectTriggerComponent.SceneIndex == SceneLoaded.buildIndex) startObject.SetActive(true);
                else startObject.SetActive(false);
            }
            if (endObject != null)
            {
                if (endObjectTriggerComponent.SceneIndex == SceneLoaded.buildIndex) endObject.SetActive(true);
                else endObject.SetActive(false);
            }
            print(SceneLoaded.buildIndex);
        }

        void Update()
        {
            if (Timer.state != Timer.TimerState.Stopped && Timer.state != Timer.TimerState.Loading && SceneManager.sceneCount > 1)
            {
                Timer.stopwatch.Stop();
                Timer.state = Timer.TimerState.Loading;
            }
            else if (Timer.state == Timer.TimerState.Loading && SceneManager.sceneCount == 1)
            {
                Timer.stopwatch.Start();
                Timer.state = Timer.TimerState.Running;
            }


            if (Settings.CreateStartTrigger.Value.IsDown())
            {
                if (startObject != null)
                {
                    Destroy(startObject);
                }
                createObjectTrigger(PlayerScript.instance.gameObject.transform.position, TypeOfTrigger.Start);
                Timer.pb[0] = TimeSpan.Zero;
                Timer.pb[1] = TimeSpan.Zero;
                TimerUI.PBText.text = TimerUI.formatTimer(TimeSpan.Zero);
            }
            if (Settings.CreateStopTrigger.Value.IsDown())
            {
                if (startObject != null)
                {
                    Destroy(endObject);
                }
                Timer.pb[0] = TimeSpan.Zero;
                Timer.pb[1] = TimeSpan.Zero;
                TimerUI.PBText.text = TimerUI.formatTimer(TimeSpan.Zero);
                createObjectTrigger(PlayerScript.instance.gameObject.transform.position, TypeOfTrigger.End);
            }
            if (Settings.ResetTimer.Value.IsDown())
            {
                Timer.stopwatch.Reset();
                if (Timer.state != Timer.TimerState.Waiting)
                {
                    Timer.state = Timer.TimerState.Stopped;
                }
            }
            if (Settings.RemoveLastPB.Value.IsDown())
            {
                Timer.pb[0] = Timer.pb[1];
                TimerUI.PBText.text = "PB " + TimerUI.formatTimer(Timer.pb[0]);
                Timer.CalculateDeltaAndUpdateText();
            }
            if (Settings.PauseTimer.Value.IsDown())
            {
                Timer.stopwatch.Stop();
                Timer.state = Timer.TimerState.Stopped;
            }
            if (Settings.ResumeTimer.Value.IsDown())
            {
                Timer.stopwatch.Start();
                Timer.state = Timer.TimerState.Running;
            }
        }

        public static void createObjectTrigger(Vector2 position, TypeOfTrigger type)
        {
            GameObject gameObjTrigger = new GameObject();
            gameObjTrigger.transform.SetParent(TimerGameObject.transform);
            gameObjTrigger.name = $"{type} ObjectTrigger";
            gameObjTrigger.transform.position = position;

            ObjectTrigger objTrigger = gameObjTrigger.AddComponent<ObjectTrigger>();
            objTrigger.Type = type;
            objTrigger.SceneIndex = SceneManager.GetActiveScene().buildIndex;

            Vector2 size = new Vector2(35, 35);
            gameObjTrigger.AddComponent<SpriteRenderer>().sprite = objTrigger.createSprite(size);
            BoxCollider2D boxColl = gameObjTrigger.AddComponent<BoxCollider2D>();
            boxColl.size = new Vector2(0.5f, 0.5f);
            boxColl.isTrigger = true;

            if (type == TypeOfTrigger.Start)
            {
                startObject = gameObjTrigger;
                startObjectTriggerComponent = objTrigger;
            }
            else
            {
                endObject = gameObjTrigger;
                endObjectTriggerComponent = objTrigger;
            }
        }
    }
}