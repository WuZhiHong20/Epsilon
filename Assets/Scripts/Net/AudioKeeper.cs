using System.Collections.Generic;

static class AudioKeeper
{
    public enum AudioIndex {DAWNGREETING, MORNINGGREETING, NOONGREETING, AFERNOONGREETING, NIGHTGREETING, MIDNIGHTING, DEEPNIGHTGREETING };

    public static Dictionary<AudioIndex, string> AudioLibrary = new Dictionary<AudioIndex, string> 
    {
        {AudioIndex.DAWNGREETING, "Greeting/起得这么早吗？我还好" },
        { AudioIndex.MORNINGGREETING, "Greeting/早上好呀！新的一天，" },
        {AudioIndex.NOONGREETING, "Greeting/中午好呀！吃个午饭好" },
        {AudioIndex.AFERNOONGREETING, "Greeting/下午好呀！来杯下午茶" },
        {AudioIndex.NIGHTGREETING, "Greeting/晚上好呀！一下子就到" },
        {AudioIndex.MIDNIGHTING, "Greeting/已经很晚了哦，要不要" },
        {AudioIndex.DEEPNIGHTGREETING, "Greeting/夜深了哦...你是打" }
    };


}