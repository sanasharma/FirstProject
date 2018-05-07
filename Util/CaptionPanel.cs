using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Tool
{
    public class CaptionPanel
    {
        private const string m_strPath = "CP5200.dll";

        private int m_nTimeout = 0;
        private int m_nIPPort = 0;
        private uint m_dwIPAddr = 0;
        private uint m_dwIDCode = 0;
        private string m_IP = "";
        private long[] m_baudtbl = new long[6] { 115200, 57600, 38400, 19200, 9600, 4800 };

        [DllImport(m_strPath, CharSet = CharSet.Auto)]
        private static extern int CP5200_Net_Init(uint dwIP, int nIPPort, uint dwIDCode, int nTimeOut);

        [DllImport(m_strPath, CharSet = CharSet.Auto)]
        public static extern int CP5200_Net_SendText(int nCardID, int nWndNo, IntPtr pText, int crColor, int nFontSize, int nSpeed, int nEffect, int nStayTime, int nAlignment);

        [DllImport(m_strPath, CharSet = CharSet.Auto)]
        public static extern int CP5200_Net_SendInstantMessage1(byte nCardID, byte byPlayTimes, int x, int y, int cx, int cy, int nFontSize, byte byColorAlign, int nEffect, byte nSpeed, byte byStayTime, byte[] pText);

        public CaptionPanel(string IP)
        {
            m_IP = IP;
        }

        private uint GetIP(string strIp)
        {
            System.Net.IPAddress ipaddress = System.Net.IPAddress.Parse(strIp);
            uint lIp = (uint)ipaddress.Address;

            lIp = ((lIp & 0xFF000000) >> 24) + ((lIp & 0x00FF0000) >> 8) + ((lIp & 0x0000FF00) << 8) + ((lIp & 0x000000FF) << 24);
            return (lIp);
        }

        private int InitComm()
        {
            int nRet = 0;
            m_dwIPAddr = GetIP(m_IP);
            if (0 != m_dwIPAddr)
            {
                m_nTimeout = 60;
                m_nIPPort = 5200;
                m_dwIDCode = GetIP("255.255.255.255");
                if (0 != m_dwIDCode)
                {
                    CP5200_Net_Init(m_dwIPAddr, m_nIPPort, m_dwIDCode, m_nTimeout);
                    nRet = 1;
                }
            }

            return nRet;
        }

        public void SendText(object Text)
        {
            try
            {
                object[] objs = Text as object[];
                string strText = objs[0].ToString();
                byte bLoop = byte.Parse(objs[1].ToString());

                byte nSpeed = 10;
                int nEffect = 11; //11向左滾動

                if (1 == InitComm())
                {
                    byte[] ctxt = System.Text.Encoding.Default.GetBytes(strText);
                    CP5200_Net_SendInstantMessage1(1, bLoop, 0, 0, 0, 0, 16, 0x77, nEffect, nSpeed, 0, ctxt);
                }
            }
            catch
            {

            }
        }
    }
}
