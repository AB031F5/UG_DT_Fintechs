using System.Collections;
using UG_DT_InternalAmolCall.Models;

namespace UG_DT_Fintechs.Logic
{
    public class Helpers
    {
        public bool IsValidNumber(string number)
        {
            var isNumeric = long.TryParse(number, out long n);
            if (isNumeric == true)
            {
                return true;
            }
            else { return false; }
        }
        public void writeToFile(ArrayList textContent, string service)
        {
            Directory.CreateDirectory(LogConstants.Errorlogs + $@"\{service}\");
            string fileUrl = LogConstants.Errorlogs+$@"\{service}\" + DateTime.Now.ToString("yyyMMdd") + ".txt";
            try
            {
                FileStream stream = new FileStream(@fileUrl, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(stream);
                if (textContent.Count == 0)
                {
                    sw.Close();
                    createFile(fileUrl);
                }
                else
                {
                    for (int i = 0; i < textContent.Count; i++)
                    {
                        sw.WriteLine(textContent[i].ToString() + " " + DateTime.Now.ToString());
                    }
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                //Do nothing
            }
        }
        public void writeToFile(ArrayList textContent)
        {
            string fileUrl = LogConstants.Errorlogs + DateTime.Now.ToString("yyyMMdd") + ".txt";
            try
            {
                FileStream stream = new FileStream(@fileUrl, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(stream);
                if (textContent.Count == 0)
                {
                    sw.Close();
                    createFile(fileUrl);
                }
                else
                {
                    for (int i = 0; i < textContent.Count; i++)
                    {
                        sw.WriteLine(textContent[i].ToString() + " " + DateTime.Now.ToString());
                    }
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                //Do nothing
            }
        }
        internal void createFile(string fileUrl)
        {
            try
            {
                FileStream fs = new FileStream(@fileUrl, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
