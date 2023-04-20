using UnityEngine;
using UnityEngine.UI;

namespace DebugStuff
{
    public class ConsoleDisplay : MonoBehaviour
    {
        //#if !UNITY_EDITOR
        static string myLog = "";
        private string output;
        private string stack;

        bool doShow;

        [SerializeField] private Slider slider;

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
                float consoleWidth = Screen.width * 0.45f; // szerokość okna konsoli
                float consoleHeight = Screen.height * (slider.value / 10f); // wysokość okna konsoli
                float consolePosX = (Screen.width - consoleWidth) / 2f; // pozycja X okna konsoli
                float consolePosY = Screen.height - consoleHeight; // pozycja Y okna konsoli

                Rect consoleRect = new Rect(consolePosX, consolePosY, consoleWidth, consoleHeight);



                GUIStyle style = GUI.skin.GetStyle("Box"); // Wybranie stylu okna konsoli
                style.fontSize = (int)(Screen.width * 0.015f); // Wielkosc czcionki
                style.alignment = TextAnchor.UpperLeft; // Wyrownanie tekstu do lewej gornej krawedzi okna konsoli

                myLog = GUI.TextArea(consoleRect, myLog, style);
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