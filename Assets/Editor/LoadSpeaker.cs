using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FileReaderEditor : EditorWindow
{
    [MenuItem("Window/File Reader")]
    static void Init()
    {
        FileReaderEditor window = (FileReaderEditor)EditorWindow.GetWindow(typeof(FileReaderEditor));
        window.Show();
    }

    private string filePath;

    [MenuItem("Window/File Reader Clear")]
    static void Clear()
    {
        GameObject ContentObj = GameObject.Find("Content");
        for(int i = ContentObj.transform.childCount-1; i>0; --i)
        {
            Transform speakerObj = ContentObj.transform.GetChild(i);
            Undo.DestroyObjectImmediate(speakerObj.gameObject);
            //Debug.Log("Destory " + i.ToString());
        }
    }

    void OnGUI()
    {
        GUILayout.Label("File Reader", EditorStyles.boldLabel);

        GUILayout.Space(20);

        GUILayout.Label("Enter File Path:");
        filePath = GUILayout.TextField(filePath);

        GUILayout.Space(10);

        if (GUILayout.Button("Load File"))
        {
            LoadFile();
        }
    }

    void LoadFile()
    {
        Debug.Log("Successful");
        // 读取文件内容
        string fileContent = System.IO.File.ReadAllText(filePath);
        string[] speaker_id = fileContent.Split('\n');
        Debug.Log(speaker_id.Length);
        GameObject Content = GameObject.Find("Content");
        float Height = 80.0f * speaker_id.Length;
        Vector2 size = new Vector2(0f, Height);
        RectTransform rectTransform = Content.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size;

        Vector2 textSize = new Vector2(304f, 80f);

        if (Content != null)
        {
            for (int i = 0; i < speaker_id.Length; ++i)
            {
                string context = "btm_" + speaker_id[i];
                GameObject btmObj = new GameObject(context);
                Button btm = btmObj.AddComponent<Button>();
                btmObj.AddComponent<ChangeSpeaker>();

                CanvasRenderer canvasRendererbtmObj = btmObj.AddComponent<CanvasRenderer>();
                //canvasRendererbtmObj.cullTransparentMesh = true;
                Image img = btmObj.AddComponent<Image>();
                btm.targetGraphic = img;

                btmObj.transform.SetParent(Content.transform);

                GameObject textObj = new GameObject("Text");
                Text text = textObj.AddComponent<Text>();
                text.text = speaker_id[i].Split(' ')[0];
                textObj.transform.SetParent(btmObj.transform);

                text.color = Color.grey;
                text.alignment = TextAnchor.MiddleCenter;
                text.fontSize = 40;
                text.rectTransform.sizeDelta = textSize;
            }
        }
    }
}
