using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

public static class Logger
{
    private static string date = "";

    private static readonly string suffix = ".txt";

    private static readonly string prefix = Application.persistentDataPath + "/LOG/";

    //private static readonly string answerPrefix = "助手:";

    public static void Init()
    {
        date = DateTime.Now.ToString().Substring(0, 10);
        date = date.Replace("/", "_");
        date = date.Replace(" ", "");
        string directoryPath = Path.Combine(Application.persistentDataPath, "LOG");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    private static string GetFilePath()
    {
        return prefix + date + suffix;
    }

    private static void WriteFile(string path, string content)
    {
        StreamWriter writer;

        if (File.Exists(path))
        {
            writer = File.AppendText(path);
        }
        else
        {
            writer = new StreamWriter(path);
        }

        writer.WriteLine(content);
        writer.Close();
    }

    public static void Log(string content)
    {
        WriteFile(GetFilePath(), content + "\n");
    }

    //public static void AnswerSaveToLocal(string answer)
    //{
    //    answerBuffer += answer + "\n";
    //}

    //public static void EndAnswer()
    //{
    //    answerBuffer += "\n";
    //    WriteFile(GetFilePath(), answerBuffer);
    //    answerBuffer = "";
    //}
}