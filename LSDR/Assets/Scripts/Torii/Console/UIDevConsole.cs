using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Torii.Event;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Torii.Console
{
    public class UIDevConsole : MonoBehaviour
    {
        public UIDevConsoleStyle Style;
        public GameObject ConsoleOutputRowPrefab;
        public ToriiEvent OnConsoleOpen;
        public ToriiEvent OnConsoleClose;
        
        public GameObject ConsoleOutputRowContainer;
        public InputField CommandInputField;
        public ScrollRect ContentScrollRect;

        private bool _visible;
        private readonly List<string> _commandHistory = new List<string>();
        private int _commandHistoryPos = -1;

        public void Start()
        {
            _visible = gameObject.activeSelf;
            
            CommandInputField.onEndEdit.AddListener(val =>
            {
                // submit command on press enter
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    commandSubmit(val);
                }
            });

            DevConsole.Register(this);
        }

        public void Update()
        {
            if (EventSystem.current.currentSelectedGameObject != CommandInputField.gameObject) return;
            
            // print completions on tab
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                PrintCompletions(CommandInputField.text);
            }
                
            // cycle command history with up/down arrows
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                CycleCommandHistory(_commandHistoryPos + 1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                CycleCommandHistory(_commandHistoryPos - 1);
            }
        }

        public void OnDestroy() { Application.logMessageReceived -= LogHandler; }
        
        [Console]
        public void Clear()
        {
            foreach (Transform child in ConsoleOutputRowContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }

        public void CycleCommandHistory(int idx)
        {
            if (_commandHistory.Count == 0) return;
            
            if (idx < 0)
            {
                idx = 0;
            }

            if (idx > _commandHistory.Count - 1)
            {
                CommandInputField.text = "";
                idx = _commandHistory.Count - 1;
            }
            
            var command = _commandHistory[idx];
            CommandInputField.text = command;
            _commandHistoryPos = idx;
        }

        public void PrintCompletions(string command)
        {
            var completions = DevConsole.Completions(command);
            StringBuilder sb = new StringBuilder("Available commands: ");
            if (completions.Count == 0)
            {
                sb.Append("None!");
            }

            sb.Append(String.Join(", ", completions));

            Debug.Log(sb.ToString());
        }

        public void ToggleVisible()
        {
            SetVisible(!_visible);
        }

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
            {
                OnConsoleClose.Raise();
            }
        }

        private void commandSubmit(string command)
        {
            var result = DevConsole.Execute(command);
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
            instantiateOutputRow(logString, type);
            if (gameObject.activeSelf) StartCoroutine(updateScrollRect());
        }
        
        private void instantiateOutputRow(string output, LogType type)
        {
            GameObject outputRow = Instantiate(ConsoleOutputRowPrefab, ConsoleOutputRowContainer.transform, false);

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
        /// Used because ScrollRect takes a frame to update, so we need to wait for that
        /// before we set the scrollbar position to the bottom
        /// </summary>
        private IEnumerator updateScrollRect()
        {
            yield return new WaitForEndOfFrame();
            ContentScrollRect.verticalNormalizedPosition = 0;
        }
    }

    [CreateAssetMenu(menuName="Torii/DevConsoleStyle")]
    public class UIDevConsoleStyle : ScriptableObject
    {
        public Color LogMessageColor = Color.white;
        public Color WarningMessageColor = Color.yellow;
        public Color ErrorMessageColor = Color.red;
        public Color ExceptionMessageColor = Color.red;

        public Sprite LogSprite;
        public Sprite WarningSprite;
        public Sprite ErrorSprite;
        public Sprite ExceptionSprite;
    }
}
