using UnityEngine;

namespace DebugStuff
{
    public class ConsoleDisplay : MonoBehaviour
    {
        //#if !UNITY_EDITOR
        static string myLog = "";
        private string output;
        private string stack;

        bool doShow;

        void Start()
        {
            doShow = false;
        }

        void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            output = logString;
            stack = stackTrace;
            myLog = output + "\n" + myLog;
            if (myLog.Length > 5000)
            {
                myLog = myLog.Substring(0, 4000);
            }
        }

        void OnGUI()
        {
            //if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
            if (!doShow) { return; }
            {
                float consoleWidth = Screen.width * 0.45f; // szerokoœæ okna konsoli
                float consoleHeight = Screen.height * 0.3f; // wysokoœæ okna konsoli
                float consolePosX = (Screen.width - consoleWidth) / 2f; // pozycja X okna konsoli
                float consolePosY = Screen.height - consoleHeight; // pozycja Y okna konsoli

                Rect consoleRect = new Rect(consolePosX, consolePosY, consoleWidth, consoleHeight);

                myLog = GUI.TextArea(consoleRect, myLog);
            }
        }
        //#endif

        public void ShowOrHideConsole()
        {
            if (doShow == true)
            {
                doShow = false;
            }
            else
                doShow = true;
        }
    }
}