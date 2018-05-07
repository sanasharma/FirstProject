using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Speech.Synthesis;

namespace Tool
{
    public class Speech
    {
        public static void SpeakAsync(object Text)
        {
            try
            {
                // 建立 SpeechSynthesizer
                SpeechSynthesizer synth = new SpeechSynthesizer();
                synth.SetOutputToDefaultAudioDevice();

                // 設定音量跟速率
                synth.Volume = 100;
                synth.Rate = -2;

                // 發音產生
                string txt = Convert.ToString(Text);
                synth.SpeakAsync(txt);
            }
            catch
            {

            }
        }
    }
}