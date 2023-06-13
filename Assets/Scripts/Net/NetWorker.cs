using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using TMPro;
using UnityEngine.UI;

class NetWorker : MonoBehaviour
{
    Socket client;
    public TMP_InputField inputField;

    //Revieve Buffer
    List<byte[]> AudioBuffer= new List<byte[]>();
    byte[] readBuff = new byte[4096];
    string recvStr = "";

    List<Socket> checkList = new List<Socket>();

    public AudioSource audioSource;

    private bool Recieving;
    private long fileLength;
    private long recievedLength;
    private void Start()
    {
        //StartSymbol = false; fragmentNum= 0; AudioBuffer.Clear(); totalAudioSize= 0;
        Recieving = false; fileLength = 0; recievedLength = 0;
        Logger.Init();
        Connection();
    }

    private int fragmentSize = 2048;
    //private bool StartSymbol;
    //private int fragmentNum;
    //private int totalAudioSize;

    //void StartRecvAudio(string recvStr)
    //{
    //    string[] strings = recvStr.Split();
    //    if(strings.Length == 3 ) { 
    //        fragmentNum = int.Parse(strings[1]); StartSymbol = true; totalAudioSize = int.Parse(strings[2]);
    //        Logger.Log($"{fragmentNum} {StartSymbol} {totalAudioSize}\n"); 
    //    }
    //}

    //片段长是 1024  Recieving ，fileLength, recievedLength
    void RecvAudio(Socket s)
    {
        // 创建缓冲区用于接收数据
        byte[] buffer = new byte[fragmentSize];
        // 接收数据
        int bytesRead = s.Receive(buffer);
        // 处理接收到的数据
        if (bytesRead > 0)
        {
            Logger.Log($"接收到{bytesRead}个bytes");
            if (Recieving == false)
            {
                Recieving = true;
                //取总长度长度
                fileLength = BitConverter.ToInt64(buffer, 0);
                byte[] fragment = new byte[bytesRead - 8];
                Array.Copy(buffer,8,fragment,0,bytesRead - 8);
                AudioBuffer.Add(fragment);
                recievedLength += bytesRead - 8;
            }
            else
            {
                //按需存储，防止存储片段之间有大量的000000000
                byte[] fragment = new byte[bytesRead];
                Array.Copy(buffer, fragment, bytesRead);
                // 将接收到的片段添加到列表中
                AudioBuffer.Add(fragment);
                recievedLength += bytesRead;
            }
            
            // Logger.Log(Encoding.UTF8.GetString(fragment));
        }
        else
        {
            Logger.Log($"Recv NO.{AudioBuffer.Count + 1} Fragment ERROR");
        }
        //if(AudioBuffer.Count == fragmentNum) { StartSymbol= false; Logger.Log($"StartSymbol change to false "); }

        if(recievedLength >= fileLength)
        {
            SynhthesisAudio();
        }

    }

    //void EndRecvAudio()
    //{
    //    // 合并音频片段
    //    byte[] audioData = new byte[totalAudioSize];
    //    int offset = 0;
    //    foreach (byte[] fragment in AudioBuffer)
    //    {
    //        Array.Copy(fragment, 0, audioData, offset, fragment.Length);
    //        offset += fragment.Length;
    //    }
    //    fragmentNum = 0 ; totalAudioSize = 0;
    //    Logger.Log("Recv Over! Start Synhthesis");
    //    SynhthesisAudio(audioData);
    //}

    /// <summary>
    /// 音频接收 处理 播放 收尾
    /// </summary>
    void EndPlay()
    {
        Recieving = false;
        fileLength = 0;
        recievedLength = 0;
        AudioBuffer.Clear();

}

    private void SynhthesisAudio()
    {
        if(AudioBuffer.Count > 0)
        {
            Debug.Log("Start Synhthesis");
            OnSynhthesisAudio();
            EndPlay();
        }
    }

    private void OnSynhthesisAudio()
    {
        byte[] audioData = new byte[fileLength];
        long offset = 0;
        foreach(byte[] fragment in AudioBuffer)
        {
            Array.Copy(fragment, 0, audioData, offset, fragment.Length);
            offset += fragment.Length;
        }
        // 创建空的 AudioClip
        //AudioClip audioClip = AudioClip.Create("ReceivedAudio", audioData.Length / 2, 1, 44100, false);
        //AudioClip audioClip = AudioClip.Create("ReceivedAudio", audioData.Length / 2, 1, 22050, false);
        AudioClip audioClip = AudioClip.Create("ReceivedAudio", audioData.Length / 4, 1, 22050, false);
        //Logger.Log("收到字节数：" + audioData.Length.ToString());
        // 设置音频数据
        audioClip.SetData(ConvertBytesToFloats(audioData), 0);

        //Logger.Log("Synhthesis OK");
        //Debug.Log("Start Synhthesis");
        // 播放音频
        StartCoroutine(PlayAudio(audioClip));
    }

    private IEnumerator PlayAudio(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
        //Logger.Log($"Start Play NO.{count} Audio");
        float playTime = audioClip.length;
        Logger.Log($"音频的时长是{playTime}s");
        return new WaitForSecondsRealtime(playTime);
    }

    //private float[] ConvertBytesToFloats(byte[] bytes)
    //{
    //    float[] floats = new float[bytes.Length / 2];

    //    for (int i = 0, j = 0; i < bytes.Length; i += 2, j++)
    //    {
    //        short value = (short)(bytes[i] | (bytes[i + 1] << 8));
    //        floats[j] = value / 32768.0f; // 将 16 位有符号整数转换为范围在 -1 到 1 之间的浮点数
    //    }

    //    return floats;
    //}

    private float[] ConvertBytesToFloats(byte[] bytes)
    {
        float[] floats = new float[bytes.Length / 4];

        Buffer.BlockCopy(bytes,0,floats,0,bytes.Length);

        Logger.Log("成功合成floats ：" + floats.Length.ToString());

        return floats;
    }

    private void UpdateNet()
    {
        if (client == null) return;

        checkList.Clear();
        checkList.Add(client);

        Socket.Select(checkList, null, null, 0);

        foreach(Socket s in checkList)
        {
            RecvAudio(s);
            //if (StartSymbol == true)
            //{
            //    RecvAudio(s);
            //}
            //byte[] readBuff = new byte[1024];
            //int count = s.Receive(readBuff);
            ////count > 10 不合理
            //if(count > 10) { Logger.Log($"长度不对劲！！！{count}" ); break; }

            //string recvStr = Encoding.Default.GetString(readBuff, 0, count);
            //if (recvStr.Contains("Start"))
            //{
            //    StartRecvAudio(recvStr);
            //    Logger.Log("Start Start Receive\n");
            //}
            //else if(recvStr.Contains("Over"))
            //{
            //    EndRecvAudio();
            //}
            //Debug.Log(count);
            //Debug.Log(recvStr);
            //Logger.Log(recvStr);
            //inputField.text = recvStr;
        }
    }

    private void OnDestroy()
    {
        OnExitGame();
    }

    private void OnExitGame()
    {
        client.Close();
        Logger.Log("退出！");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void Update()
    {
        UpdateNet();
    }

    public void Connection()
    {
        //Socket
        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Connect
        client.BeginConnect("127.0.0.1", 9888, ConnectCallBack, client);
    }

    public void ConnectCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Logger.Log("Socket Connect Succ");
        }
        catch(SocketException ex)
        {
            Logger.Log("Socket Connect Failed " + ex.ToString());
        }
    }

    public void Send()
    {
        string sendStr = inputField.text;
        Logger.Log("Client wana send " + sendStr + "\n");
        byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
        client.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallBack, client);
    }

    private void SendCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndSend(ar);
            Logger.Log("Socket Send Succ" + count);
        }
        catch(SocketException ex)
        {
            Logger.Log("Socket Send fail " + ex.ToString()); 
        }
    }
}
