using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Bremsengine
{
    #region Dialogue Editor Custom Inspector
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine.InputSystem;

    [CustomEditor(typeof(TestDialogue))]
    public partial class DialogueInspectorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Start Dialogue") && Application.isPlaying && target is Dialogue d and not null)
            {
                d.StartDialogue();
            }
        }
    }
#endif
    #endregion
    #region Dialogue Internal Coroutines
    public partial class Dialogue
    {
        Dictionary<string, Coroutine> activeRoutines = new();
        public bool TryGetSubroutine(string key, out Coroutine c)
        {
            c = null;
            if (activeRoutines.ContainsKey(key))
            {
                c = activeRoutines[key];
                return true;
            }
            return false;
        }
        public void TryEndSubroutine(string key)
        {
            if (TryGetSubroutine(key, out Coroutine c))
            {
                if (c != null)
                    StopCoroutine(c);
            }
        }
        public void StartSubroutine(string key, IEnumerator coroutine)
        {
            TryEndSubroutine(key);
            activeRoutines[key] = StartCoroutine(coroutine);
        }
    }
    #endregion
    #region Dialogue Event Busses
    public partial class Dialogue
    {
        public void TriggerEvent(string key)
        {
            DialogueEventBus.TriggerEvent(key);
        }
    }
    public static class DialogueEventBus
    {
        static Dictionary<string, System.Action> EventBusCache;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ReInitializeEventBus()
        {
            EventBusCache = new Dictionary<string, System.Action>();
        }
        public static void BindEvent(string eventID, System.Action action)
        {
            if (!EventBusCache.ContainsKey(eventID) || EventBusCache[eventID] == null)
            {
                EventBusCache[eventID] = action;
                return;
            }
            EventBusCache[eventID] += action;
        }
        public static void ReleaseEvent(string eventID, System.Action action)
        {
            if (EventBusCache.ContainsKey(eventID) && EventBusCache[eventID] != null)
            {
                EventBusCache[eventID] -= action;
            }
        }
        public static void TriggerEvent(string eventID)
        {
            if (EventBusCache.ContainsKey(eventID) && EventBusCache[eventID] != null)
            {
                EventBusCache[eventID]?.Invoke();
            }
        }
    }
    #endregion
    #region Dialogue Buttons
    public abstract partial class Dialogue
    {
        #region Dialogue Button Entries Class
        [System.Serializable]
        public class DialogueButton
        {
            int continueProgress = -999;
            #region Button Actions
            public DialogueButton SetText(string s)
            {
                ButtonText.text = s;
                return this;
            }
            public DialogueButton SetVisible(bool state)
            {
                IsVisible = state;
                ButtonReference.gameObject.SetActive(state);
                return this;
            }

            public DialogueButton SetProgressWhenPressed(int progress = -999)
            {
                continueProgress = progress;
                return this;
            }
            public DialogueButton ContinueWhenPressed()
            {
                continueProgress = Progress + 2;
                return this;
            }
            private void PressContinueButton()
            {
                Debug.Log(continueProgress);
                OnContinuePressed = null;
                if (continueProgress != -999)
                {
                    OnContinuePressed += Dialogue.SetProgress;
                }
                OnContinuePressed?.Invoke(continueProgress);
            }
            public DialogueButton SetForceEndWhenPressed()
            {
                void ButtonEndDialogue()
                {
                    if (Dialogue.ActiveDialogue == null)
                        return;
                    Dialogue.ActiveDialogue.ForceEndDialogue();
                }
                OnPressedAction += ButtonEndDialogue;
                return this;
            }
            private TMP_Text FindAndCacheTextComponent(Button b)
            {
                if (b == null)
                {
                    Debug.Log("Bad Button Reference");
                    return null;
                }
                if (ButtonReference.gameObject.GetComponentInChildren<TMP_Text>() is TMP_Text t and not null)
                {
                    storedButtonText = t;
                    return t;
                }
                Debug.Log("Found Nothing, maybe bad");
                return null;
            }
            public DialogueButton PressButton()
            {
                OnPressedAction?.Invoke();
                PressContinueButton();
                return this;
            }
            #endregion
            public int ButtonIndex = 0;
            bool IsVisible;
            public Button ButtonReference;
            TMP_Text storedButtonText;
            public System.Action OnPressedAction;
            public System.Action<int> OnContinuePressed;
            public bool ContinueDialogueWhenPressed { get; private set; }
            public TMP_Text ButtonText => storedButtonText == null ? FindAndCacheTextComponent(ButtonReference) : storedButtonText;
        }
        #endregion
        #region Cache Reset
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        //Unity Engine Runs this on Run Game.
        //It's manual resetting of static objects for a setting i use that turns off static resetting for faster loads.
        private static void ResetCache()
        {
            buttonCache = null;
        }
        #endregion
        static Dictionary<int, DialogueButton> buttonCache;
        private static void Initialize(List<DialogueButton> b)
        {
            if (buttonCache == null)
            {
                buttonCache = new Dictionary<int, DialogueButton>();
            }
            buttonCache.Clear();
            foreach (var item in b)
            {
                buttonCache.Add(item.ButtonIndex, item);
            }
        }
        private static bool TryGetCachedButton(int buttonIndex, out DialogueButton b)
        {
            Initialize(DialogueRunner.GetButtons());
            if (buttonCache != null && buttonCache.ContainsKey(buttonIndex))
            {
                b = buttonCache[buttonIndex];
                return true;
            }
            Debug.LogWarning("Bad Button id");
            b = null;
            return false;
        }
        public static void ClearButtonContents()
        {
            Initialize(DialogueRunner.GetButtons());
            DialogueButton iteration;
            foreach (var item in buttonCache)
            {
                if (item.Value == null)
                {
                    Debug.LogWarning("Bad");
                    continue;
                }
                iteration = item.Value;
                iteration.OnPressedAction = null;
                iteration.OnContinuePressed = null;
                iteration.SetVisible(false);
            }
        }
        public static DialogueButton SetButton(int buttonIndex, string buttonText, Action a = null)
        {
            if (TryGetCachedButton(buttonIndex, out DialogueButton b))
            {
                b.SetVisible(true);
                b.SetText(buttonText);
                b.SetProgressWhenPressed(-999 /* -999 means no continue when pressed*/);
                if (a != null)
                {
                    b.OnPressedAction = a;
                }
                return b;
            }
            return null;
        }
        public static void PressButton(int buttonIndex)
        {
            if (TryGetCachedButton(buttonIndex, out DialogueButton b))
            {
                b.PressButton();
            }
        }
    }
    #endregion
    #region Dialogue Text

    [System.Serializable]
    public class DialogueText
    {
        static TMP_Text textRenderer;
        public delegate void TextEvent(DialogueText t);
        public static TextEvent OnDisplayText;
        public static TextEvent OnFinishDisplayText;
        public string text { get; private set; }
        public static void SetDialogueTextRenderer(TMP_Text t)
        {
            textRenderer = t;
        }
        public void DisplayText(string s)
        {
            text = s;
            ReDrawText();
            OnDisplayText?.Invoke(this);
            OnFinishDisplayText?.Invoke(this); //For now it will just display everything instantly so onfinished will go here
        }
        private void ReDrawText()
        {
            textRenderer.text = text;
        }
        public void AddText(string s)
        {
            text += s;
            ReDrawText();
        }
        public void AddText(char c)
        {
            text += c;
            ReDrawText();
        }
    }
    public abstract partial class Dialogue
    {
        protected static DialogueText activeText;
        protected DialogueText text => activeText;
        public static void BindDialogueText(DialogueText d)
        {
            activeText = d;
        }
        protected void DrawDialogue(string text)
        {
            DialogueRunner.BoxVisibility(true);
            ClearButtonContents();
            activeText.DisplayText(text);
        }
        protected void UnDrawDialogue()
        {
            DialogueRunner.BoxVisibility(false);
        }
    }
    #endregion
    #region Continue Shortcut Input
    public partial class Dialogue
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Reinitialize()
        {

        }
    }
    #endregion
    #region Dialogue Shortcuts
    public abstract partial class Dialogue
    {
        protected void ContinueButton(int index) => SetButton(index, "Continue").SetProgressWhenPressed(Progress + 2);
        protected int NextContinueProgress => Progress + 2;
        protected void ActionButton(int index, Action<bool> buttonAction, bool state)
        {
            buttonAction?.Invoke(state);
        }
        protected static int Progress = 0;
        public static void SetProgress(int p) => Progress = p;
        public WaitUntil WaitForProgressAbove(int progress) => new WaitUntil(() => Progress >= progress);
        protected void CharacterSelect(int index) => DialogueRunner.SetCharacterFocus(index);
    }
    #endregion
    #region Character Sprites
    public partial class Dialogue
    {
        [SerializeField] List<Sprite> characterSprites = new List<Sprite>();
    }
    #endregion
    public abstract partial class Dialogue : MonoBehaviour
    {
        public static Dialogue activeDialogueCollection { get; protected set; }
        protected static DialogueRunner runnerInstance;
        protected abstract IEnumerator DialogueContents(int progress = 0);
        public delegate void DialogueEvent(Dialogue dialogue);
        public static DialogueEvent TriggerContinue;
        static Coroutine activeDialogueRoutine;
        static Dialogue ActiveDialogue;
        public static bool IsDialogueRunning => ActiveDialogue != null;
        protected abstract void WhenStartDialogue(int progress);
        protected abstract void WhenEndDialogue(int dialogueEnding);
        public static void BindRunner(DialogueRunner runner)
        {
            runnerInstance = runner;
        }
        [ContextMenu("Start Dialogue")]
        public void StartDialogue(int progress = 0)
        {
            Progress = progress;
            if (activeDialogueRoutine != null && runnerInstance == null)
            {
                Debug.Log("Really bad");
            }
            if (activeDialogueRoutine != null && runnerInstance != null)
            {
                runnerInstance.StopCoroutine(activeDialogueRoutine);
            }
            ActiveDialogue = this;
            DialogueRunner.SetDialogueVisibility(true);
            int spriteIndex = 0;
            foreach (var item in DialogueRunner.AllCharacterSprites)
            {
                item.sprite = null;
            }
            foreach (var item in characterSprites)
            {
                DialogueRunner.SetCharacterSprite(spriteIndex, item);
                spriteIndex++;
            }
            activeDialogueRoutine = runnerInstance.StartCoroutine(DialogueContents(progress));
            WhenStartDialogue(progress);
        }
        public void ForceEndDialogue(int ending = 0)
        {
            WhenEndDialogue(ending);
            if (activeDialogueRoutine != null)
            {
                runnerInstance.StopCoroutine(activeDialogueRoutine);
                activeDialogueRoutine = null;
            }
            DialogueRunner.SetDialogueVisibility(false);
            ActiveDialogue = null;
        }
    }
}
