using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Torii.Event;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Torii.Console
{
    public class UIDevConsole : MonoBehaviour
    {
        public const int MAX_INSTANTIATED_OUTPUT_ROWS = 500;
        public UIDevConsoleStyle Style;
        public GameObject ConsoleOutputRowPrefab;
        public ToriiEvent OnConsoleOpen;
        public ToriiEvent OnConsoleClose;

        public GameObject ConsoleOutputRowContainer;
        public InputField CommandInputField;
        public ScrollRect ContentScrollRect;
        public GameObject ErrorDisplayObject;
        public Text ErrorCountText;
        public bool ShowErrorDisplay = false;

        private readonly List<string> _commandHistory = new List<string>();
        private readonly Queue<GameObject> _instantiatedOutputRows = new Queue<GameObject>();
        private int _commandHistoryPos = -1;
        private int _errorCount = 0;

        private bool _visible;

        public void Awake() { Application.logMessageReceived += LogHandler; }

        public void Start()
        {
            CommandInputField.onEndEdit.AddListener(val =>
            {
                // submit command on press enter
                if (Keyboard.current.enterKey.wasPressedThisFrame ||
                    Keyboard.current.numpadEnterKey.wasPressedThisFrame) commandSubmit(val);
            });

            DevConsole.Register(this);

            _visible = false;
            gameObject.SetActive(false);
        }

        public void Update()
        {
            if (EventSystem.current.currentSelectedGameObject != CommandInputField.gameObject)
            {
                EventSystem.current.SetSelectedGameObject(CommandInputField.gameObject);
            }

            // print completions on tab
            if (Keyboard.current.tabKey.wasPressedThisFrame) PrintCompletions(CommandInputField.text);

            // cycle command history with up/down arrows
            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
                CycleCommandHistory(_commandHistoryPos + 1);
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame) CycleCommandHistory(_commandHistoryPos - 1);
        }

        public void OnEnable()
        {
            ErrorDisplayObject.SetActive(ShowErrorDisplay);
        }

        public void OnDestroy() { Application.logMessageReceived -= LogHandler; }

        [Console]
        public void Clear()
        {
            foreach (Transform child in ConsoleOutputRowContainer.transform) Destroy(child.gameObject);
        }

        public void CycleCommandHistory(int idx)
        {
            if (_commandHistory.Count == 0) return;

            if (idx < 0) idx = 0;

            if (idx > _commandHistory.Count - 1)
            {
                CommandInputField.text = "";
                idx = _commandHistory.Count - 1;
            }

            string command = _commandHistory[idx];
            CommandInputField.text = command;
            _commandHistoryPos = idx;
        }

        public void PrintCompletions(string command)
        {
            List<string> completions = DevConsole.Completions(command);
            StringBuilder sb = new StringBuilder("Available commands: ");
            if (completions.Count == 0) sb.Append("None!");

            sb.Append(string.Join(", ", completions));

            Debug.Log(sb.ToString());
        }

        public void ToggleVisible() { SetVisible(!_visible); }

        public void SetVisible(bool visible)
        {
            CommandInputField.text = string.Empty;

            _visible = visible;
            gameObject.SetActive(visible);

            if (visible)
            {
                StartCoroutine(updateScrollRect());
                selectInputField();
                OnConsoleOpen.Raise();
            }
            else
                OnConsoleClose.Raise();
        }

        private void commandSubmit(string command)
        {
            DevConsole.ExecutionResult result = DevConsole.Execute(command);
            result.Log();
            CommandInputField.text = string.Empty;
            selectInputField();
            _commandHistoryPos = _commandHistory.Count - 1;
            _commandHistory.Add(command);
        }

        private void selectInputField()
        {
            EventSystem.current.SetSelectedGameObject(CommandInputField.gameObject);
            CommandInputField.OnPointerClick(new PointerEventData(EventSystem.current));
        }

        public void LogHandler(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                _errorCount++;
                updateErrorCount();
            }

            instantiateOutputRow(logString, type);
            if (gameObject.activeSelf) StartCoroutine(updateScrollRect());
        }

        private void updateErrorCount()
        {
            ErrorCountText.text = _errorCount.ToString();
        }

        private void instantiateOutputRow(string output, LogType type)
        {
            GameObject outputRow = Instantiate(ConsoleOutputRowPrefab, ConsoleOutputRowContainer.transform,
                worldPositionStays: false);
            _instantiatedOutputRows.Enqueue(outputRow);

            if (_instantiatedOutputRows.Count > MAX_INSTANTIATED_OUTPUT_ROWS)
                Destroy(_instantiatedOutputRows.Dequeue());

            UIDevConsoleOutputRow outputRowScript = outputRow.GetComponent<UIDevConsoleOutputRow>();
            outputRowScript.OutputMessage = output;
            switch (type)
            {
                case LogType.Log:
                {
                    outputRowScript.IconColor = Style.LogMessageColor;
                    outputRowScript.IconSprite = Style.LogSprite;
                    outputRowScript.TextColor = Style.LogMessageColor;
                    break;
                }
                case LogType.Warning:
                {
                    outputRowScript.IconColor = Style.WarningMessageColor;
                    outputRowScript.IconSprite = Style.WarningSprite;
                    outputRowScript.TextColor = Style.WarningMessageColor;
                    break;
                }
                case LogType.Error:
                {
                    outputRowScript.IconColor = Style.ErrorMessageColor;
                    outputRowScript.IconSprite = Style.ErrorSprite;
                    outputRowScript.TextColor = Style.ErrorMessageColor;
                    break;
                }
                case LogType.Assert:
                {
                    outputRowScript.IconColor = Style.ErrorMessageColor;
                    outputRowScript.IconSprite = Style.ErrorSprite;
                    outputRowScript.TextColor = Style.ErrorMessageColor;
                    break;
                }
                case LogType.Exception:
                {
                    outputRowScript.IconColor = Style.ExceptionMessageColor;
                    outputRowScript.IconSprite = Style.ExceptionSprite;
                    outputRowScript.TextColor = Style.ExceptionMessageColor;
                    break;
                }
            }
        }

        /// <summary>
        ///     Used because ScrollRect takes a frame to update, so we need to wait for that
        ///     before we set the scrollbar position to the bottom
        /// </summary>
        private IEnumerator updateScrollRect()
        {
            yield return new WaitForEndOfFrame();
            ContentScrollRect.verticalNormalizedPosition = 0;
        }
    }
}
