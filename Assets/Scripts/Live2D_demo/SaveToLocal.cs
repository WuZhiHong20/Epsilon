using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

public static class SaveToLocal
{
    private static readonly string suffix = ".docx";

    private static readonly string prefix = Application.persistentDataPath + "/ChatRecord/";

    //private static readonly string answerPrefix = "助手:";

    private static string answerBuffer = "";

    public static void Init()
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "ChatRecord");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    private static string GetFilePath()
    {
        TimeDetector timeDetector = TimeDetector.GetInstance();
        string fileName = timeDetector.GetDateTime();

        //Debug.Log(fileName);

        return prefix + fileName + suffix;
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

    public static void InputSaveToLocal(string content)
    {
        if (content.Substring(0, 2) != "我:")
        {
            content = "我:" + content;
        }
        WriteFile(GetFilePath(), content);
    }

    public static void AnswerSaveToLocal(string answer)
    {
        answerBuffer += answer + "\n";
    }

    public static void EndAnswer()
    {
        answerBuffer += "\n";
        WriteFile(GetFilePath(), answerBuffer);
        answerBuffer = "";
    }
}