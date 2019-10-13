using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FTPscan
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public NetworkCredential nc = new NetworkCredential("anonymous", "anonymous");
        public List<String> dataExternal = new List<String> { };
        public string fileCurrentlyWriting = "";
        public int totalIPs;
        public int completeIps;
        public string[] lines;

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] Ofirstsplit = lines.Take(lines.Length / 2).ToArray();
            string[] Osecondsplit = lines.Skip(lines.Length / 2).ToArray();

            string[] firstsplit = Ofirstsplit.Take(Ofirstsplit.Length / 2).ToArray();
            string[] secondsplit = Ofirstsplit.Skip(Ofirstsplit.Length / 2).ToArray();
            string[] thirdsplit = Osecondsplit.Take(Osecondsplit.Length / 2).ToArray();
            string[] forthsplit = Osecondsplit.Skip(Osecondsplit.Length / 2).ToArray();

            Thread a = new Thread(() => doExist(firstsplit, 1));
            a.IsBackground = true;
            a.Start();

            Thread b = new Thread(() => doExist(secondsplit, 2));
            b.IsBackground = true;
            b.Start();

            Thread c = new Thread(() => doExist(thirdsplit, 3));
            c.IsBackground = true;
            c.Start();

            Thread d = new Thread(() => doExist(forthsplit, 4));
            d.IsBackground = true;
            d.Start();
        }

        public void doExist(string[] ips, int thrid)
        {
            foreach (string s in ips)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    string safeFilename = s.Replace('.', '-') + ".txt";
                    safeFilename = safeFilename.Replace(':', '-');
                    fileCurrentlyWriting = Application.StartupPath + "/" + safeFilename;
                    fileCurrentlyWriting = fileCurrentlyWriting.Replace('\\', '/');
                    var file = File.Create(fileCurrentlyWriting);
                    file.Dispose();
                    dataExternal = new List<String> { };
                    completeIps++;
                    label2.Text = completeIps + " / " + lines.Length;
                }));
                string root = "ftp://" + s;
                List<String> data = getFilesInDirectory(root, thrid, s);
            }
        }

        public List<String> getFilesInDirectory(string dir, int thrid, string rawIP)
        {
            int fileCounter = 0;

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(dir);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.UsePassive = false;
                request.Credentials = nc;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string dataRaw = reader.ReadToEnd();
                string[] dataSpl = dataRaw.Split('\n');
                List<string> dataFinal = new List<String> { };
                foreach (string str in dataSpl)
                {
                    if (str.Length > 0)
                    {
                        string tmp = str.Substring(56);
                        tmp = Regex.Replace(tmp, @"\t|\n|\r", "");
                        string nextURl = dir + "/" + tmp;
                        string processedURL = nextURl.Replace("ftp://", "");

                        processedURL = processedURL.Replace(" ", "_");               

                        //Console.WriteLine(processedURL);
                        this.Invoke(new MethodInvoker(delegate ()
                        {
                            dataExternal.Add(processedURL);
                            File.WriteAllLines(fileCurrentlyWriting, dataExternal);
                        }));

                        switch (thrid)
                        {
                            case 1:
                                this.Invoke(new MethodInvoker(delegate ()
                                {
                                    label1.Text = rawIP.Replace("ftp://", "") + " | " + fileCounter + " files";
                                }));
                                break;

                            case 2:
                                this.Invoke(new MethodInvoker(delegate ()
                                {
                                    label7.Text = rawIP.Replace("ftp://", "") + " | " + fileCounter + " files";
                                }));
                                break;

                            case 3:
                                this.Invoke(new MethodInvoker(delegate ()
                                {
                                    label8.Text = rawIP.Replace("ftp://", "") + " | " + fileCounter + " files";
                                }));
                                break;

                            case 4:
                                this.Invoke(new MethodInvoker(delegate ()
                                {
                                    label9.Text = rawIP.Replace("ftp://", "") + " | " + fileCounter + " files";
                                }));
                                break;

                            default:
                                break; //should never hit this
                        }

                        fileCounter++;
                        string sbst = nextURl.Substring(nextURl.Length - 4, 1);
                        if(!isUrlAFile(processedURL))
                        {
                            getFilesInDirectory(nextURl, thrid, rawIP);
                        }
                    }
                }

                return dataFinal;
            }
            catch
            {
                return new List<String> { };
            }
        }

        public bool isUrlAFile(string url)
        {
            string[] splitBySlash = url.Split('/');
            string finalObj = splitBySlash[splitBySlash.Length - 1];
            string[] splitByDot = finalObj.Split('.');
            string finalPostDor = splitByDot[splitByDot.Length - 1];
            if(finalPostDor.Length > 1 && finalPostDor.Length < 5)
            {
                //probably a file
                return true;
            }
            else
            {
                //probably a folder, or a really weird file idk
                return false;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Input File";
                dlg.Filter = "Text Files | *.txt";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    lines = File.ReadAllLines(dlg.FileName);
                    label2.Text = "0 / " + lines.Length;
                }
            }            
        }
    }
}
