using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Multi_IU_Check_Transaction
{
    class LogClass
    {
        public static void WriteLog(string content)
        {
            string LogPath = Application.StartupPath + "\\Log";
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }
            StreamWriter sw = new StreamWriter(Application.StartupPath + "\\Log\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log", true);
            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss  ") + content);
            sw.Flush();
            sw.Close();

        }


    }
}
