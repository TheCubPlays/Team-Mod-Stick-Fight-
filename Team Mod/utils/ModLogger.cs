// This is mainly used for debugging because I don't know where UnityEngine sends logs lol

using System;
using System.IO;
using UnityEngine;

public static class ModLogger
{
    private static string logFilePath = Path.Combine(Application.persistentDataPath, "TMOD_Log.txt");

    public static void Log(string message)
    {
        try
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string fullMessage = $"[{timestamp}] {message}{Environment.NewLine}";

            File.AppendAllText(logFilePath, fullMessage);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to write to log file: " + e); 
        }
    }
}