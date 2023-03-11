using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioSynhthesis
{
    public class AnswerInfo
    {
        private string audioPath;
        private string answerStr;
        private string jsonPath;

        public string AudioPath
        {
            get
            {
                return audioPath;
            }
            set
            {
                audioPath = value;
            }
        }

        public string AnswerStr
        {
            get
            {
                return answerStr;
            }
            set
            {
                answerStr = value;
            }
        }

        public string JsonPath
        {
            get
            {
                return jsonPath;
            }
            set
            {
                jsonPath = value;
            }
        }

        public AnswerInfo(string audio, string answer, string jPath)
        {
            audioPath = audio;
            answerStr = answer;
            jsonPath = jPath;
        }
    }
}
