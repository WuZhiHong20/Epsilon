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
    private static NetWorker _instance;

    public static NetWorker Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new NetWorker();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
    Socket client;

    private TMP_InputField _inputField;

    private TMP_InputField inputField
    {
        get
        {
            if (_inputField == null)
            {
                GameObject btm_Send = GameObject.Find("chatWindow");
                _inputField = btm_Send.GetComponentInChildren<TMP_InputField>();
            }
            return _inputField;
        }
    }

    //Revieve Buffer
    List<byte[]> AudioBuffer= new List<byte[]>();
    byte[] readBuff = new byte[4096];
    // string recvStr = "";

    List<Socket> checkList = new List<Socket>();

    private bool Recieving;
    private long fileLength;
    private long recievedLength;

    private bool CanExit;

    public Epsilon epsilon;

    private void Start()
    {
        //StartSymbol = false; fragmentNum= 0; AudioBuffer.Clear(); totalAudioSize= 0;
        Recieving = false; fileLength = 0; recievedLength = 0; CanExit = false;
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

    /// <summary>
    /// 片段长是 1024  Recieving ，fileLength, recievedLength
    /// 对AudioBuffer的处理应该要加一个长度维护， 一个语音文件占了几个元素，传入合成函数中
    /// </summary>
    /// <param name="s"></param>
    void RecvAudio(Socket s)
    {
        // 创建缓冲区用于接收数据
        byte[] buffer = new byte[fragmentSize];
        // 接收数据
        int bytesRead = s.Receive(buffer);
        // 处理接收到的数据
        if (bytesRead > 0)
        {
            // Logger.Log($"接收到{bytesRead}个bytes");
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
            if (recievedLength == fileLength)
            {
                SynhthesisAudio(AudioBuffer.Count);
                EndRecieve();
            }
            else
            {
                //如果是连续发送的包，两个语音文件发生了粘包，那么就要把最后一个拿出来重新放
                int OverLength = (int)(recievedLength - fileLength); //多发送的，下一个文件的长度
                int LeftLength = bytesRead - OverLength; //上一个文件剩下的
                Logger.Log("发生了粘包，上一个文件还剩 " + LeftLength.ToString() + " bytes 下一个文件发送了 " + OverLength.ToString() + "bytes");
                AudioBuffer.RemoveAt(AudioBuffer.Count - 1);
                byte[] fragment = new byte[LeftLength];
                Array.Copy(buffer, 0, fragment, 0, LeftLength);
                AudioBuffer.Add(fragment);
                SynhthesisAudio(AudioBuffer.Count);

                //获取下一个文件的首部信息，，，这里默认了首部信息不会被拆包，，，但是其实有可能会发生拆包 ，，，
                if(OverLength > 8)
                {
                    fileLength = BitConverter.ToInt64(buffer, LeftLength);
                    int StartPos = LeftLength + 8;
                    OverLength -= 8;
                    byte[] nextFile = new byte[OverLength];
                    Array.Copy(buffer, StartPos, nextFile, 0, OverLength);
                    AudioBuffer.Add(nextFile);
                    recievedLength = OverLength;
                }
                else
                {
                    Logger.Log("ERROR ERROR 发生了拆包 发生了拆包 首部被拆了");
                }
                
            }
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
    void EndRecieve()
    {
        Recieving = false;
        fileLength = 0;
        recievedLength = 0;
    }

    private void SynhthesisAudio(int fragmentSize)
    {
        if(fragmentSize > 0)
        {
            Debug.Log("Start Synhthesis");
            OnSynhthesisAudio(fragmentSize);
        }
    }
    /// <summary>
    /// 传入一个语音文件被拆成了几个 字节数组， 重新合成
    /// </summary>
    /// <param name="fragmentSize"></param>
    private void OnSynhthesisAudio(int fragmentSize)
    {
        byte[] audioData = new byte[fileLength];
        long offset = 0;
        for(int i = 0; i < fragmentSize; ++i)
        {
            Array.Copy(AudioBuffer[0], 0, audioData, offset, AudioBuffer[0].Length);
            offset += AudioBuffer[0].Length;
            AudioBuffer.RemoveAt(0);
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
        epsilon.Speaking(audioClip);
    }

    //private IEnumerator PlayAudio(AudioClip audioClip)
    //{ 

    //    //Logger.Log($"Start Play NO.{count} Audio");
    //    float playTime = audioClip.length;
    //    Logger.Log($"音频的时长是{playTime}s");
    //    return new WaitForSecondsRealtime(playTime);
    //}

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
        if (client == null || CanExit == true) return;

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

    public void OnExitGame()
    {
        string sendInfo = SendInfo.Commands[2] + SendInfo.mode;
        Logger.Log("Client wana send " + sendInfo + "\n");
        byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendInfo);
        client.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallBack, client);
        CanExit = true;
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

    /// <summary>
    /// 和Send btm UI进行了绑定
    /// 能接受的格式是 : string message = "Repeat t 你好，我是牧濑红莉牺，是你的助手。很高兴见到你，从今往后，请多多关照。 87";
    /// </summary>
    public void Send()
    {
        string sendStr = inputField.text;
        if(sendStr.Length == 0)
        {
            inputField.text = "不要发送空内容哦（发送新内容前先清楚本内容...）";
        }
        else
        {
            string sendInfo = SendInfo.Commands[0] + SendInfo.mode + sendStr + " " + SendInfo.Speaker.ToString();
            Logger.Log("Client wana send " + sendInfo + "\n");
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendInfo);
            client.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallBack, client);
        }
    }

    private void SendCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndSend(ar);
            Logger.Log("Socket Send Succ" + count);
            if(CanExit == true)
            {
                client.Close();
                Logger.Log("---------------Exit-------------");
#if UNITY_EDITOR
                Debug.Log("退出编辑器模式");
                UnityEditor.EditorApplication.isPlaying = false;
                Debug.Log("停止。。。失败？");
#else
                Application.Quit();
#endif
            }
        }
        catch(SocketException ex)
        {
            Logger.Log("Socket Send fail " + ex.ToString()); 
        }
    }
}
