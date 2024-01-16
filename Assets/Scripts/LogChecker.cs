using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogChecker : MonoBehaviour
{
    private const int MaxLogLines = 10;
    private string logText = "";
    private GUIStyle guiStyle = new GUIStyle();
    private bool showLogInGame = false;

    private float lastLogTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        guiStyle.fontSize = 20;
        guiStyle.normal.textColor = Color.white;

        showLogInGame = !Application.isEditor;
    }

    private void OnGUI()
    {
        if (showLogInGame)
        {
            GUI.Label(new Rect(10, 10, Screen.width, Screen.height), logText, guiStyle);
        }
    }

    private void OnEnable()
    {
        // デバッグログを表示するためのイベントハンドラを登録
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        // イベントハンドラを解除
        Application.logMessageReceived -= HandleLog;
    }

    private void Update()
    {
        // ゲーム画面内のログ表示が有効な場合のみ、3秒ごとにログのテキストをクリア
        if (showLogInGame && Time.time - lastLogTime > 3f)
        {
            logText = "";
        }
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Debug.Log()のテキストをlogTextに追加
        logText += logString + "\n";

        // 表示するログの行数がMaxLogLinesを超えたら、古いログを削除
        string[] logLines = logText.Split('\n');
        if (logLines.Length > MaxLogLines)
        {
            logText = string.Join("\n", logLines, logLines.Length - MaxLogLines, MaxLogLines);
        }

        // 最後にログを表示した時刻を更新
        lastLogTime = Time.time;
    }
}