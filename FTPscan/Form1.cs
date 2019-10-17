using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
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

        public List<String> dataExternal_1 = new List<String> { };
        public List<String> dataExternal_2 = new List<String> { };
        public List<String> dataExternal_3 = new List<String> { };
        public List<String> dataExternal_4 = new List<String> { };

        public List<String> filesToDownload = new List<String> { };

        public string fileCurrentlyWriting_1 = "";
        public string fileCurrentlyWriting_2 = "";
        public string fileCurrentlyWriting_3 = "";
        public string fileCurrentlyWriting_4 = "";

        public bool usingMaximumFileCount = false;
        public static int maximumFileCount = 0;

        public int currentFilecount_1 = 0;
        public int currentFilecount_2 = 0;
        public int currentFilecount_3 = 0;
        public int currentFilecount_4 = 0;

        public int totalIPs;
        public int completeIps;
        public string[] lines;

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                RNGCryptoServiceProvider rnd2 = new RNGCryptoServiceProvider(); //may be overdoing it a little
                lines = lines.OrderBy(x => GetNextInt32(rnd2)).ToArray();
            }

            if (checkBox2.Checked)
            {
                usingMaximumFileCount = true;
                maximumFileCount = (int)numericUpDown1.Value;
            }

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            checkBox1.Enabled = false;
            checkBox2.Enabled = false;
            checkBox3.Enabled = false;
            numericUpDown1.Enabled = false;
            textBox1.Enabled = false;
            listBox1.Enabled = false;
            panel1.BackColor = Color.Green;

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
                if (thrid == 1)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        currentFilecount_1 = 0;
                        string safeFilename = s.Replace('.', '-') + ".txt";
                        safeFilename = safeFilename.Replace(':', '-');
                        fileCurrentlyWriting_1 = Application.StartupPath + "/" + safeFilename;
                        fileCurrentlyWriting_1 = fileCurrentlyWriting_1.Replace('\\', '/');
                        var file = File.Create(fileCurrentlyWriting_1);
                        file.Dispose();
                        dataExternal_1 = new List<String> { };
                        completeIps++;
                        label2.Text = completeIps + " / " + lines.Length;
                    }));
                }
                if (thrid == 2)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        currentFilecount_2 = 0;
                        string safeFilename = s.Replace('.', '-') + ".txt";
                        safeFilename = safeFilename.Replace(':', '-');
                        fileCurrentlyWriting_2 = Application.StartupPath + "/" + safeFilename;
                        fileCurrentlyWriting_2 = fileCurrentlyWriting_2.Replace('\\', '/');
                        var file = File.Create(fileCurrentlyWriting_2);
                        file.Dispose();
                        dataExternal_2 = new List<String> { };
                        completeIps++;
                        label2.Text = completeIps + " / " + lines.Length;
                    }));
                }
                if (thrid == 3)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        currentFilecount_3 = 0;
                        string safeFilename = s.Replace('.', '-') + ".txt";
                        safeFilename = safeFilename.Replace(':', '-');
                        fileCurrentlyWriting_3 = Application.StartupPath + "/" + safeFilename;
                        fileCurrentlyWriting_3 = fileCurrentlyWriting_3.Replace('\\', '/');
                        var file = File.Create(fileCurrentlyWriting_3);
                        file.Dispose();
                        dataExternal_3 = new List<String> { };
                        completeIps++;
                        label2.Text = completeIps + " / " + lines.Length;
                    }));
                }
                if (thrid == 4)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        currentFilecount_4 = 0;
                        string safeFilename = s.Replace('.', '-') + ".txt";
                        safeFilename = safeFilename.Replace(':', '-');
                        fileCurrentlyWriting_4 = Application.StartupPath + "/" + safeFilename;
                        fileCurrentlyWriting_4 = fileCurrentlyWriting_4.Replace('\\', '/');
                        var file = File.Create(fileCurrentlyWriting_4);
                        file.Dispose();
                        dataExternal_4 = new List<String> { };
                        completeIps++;
                        label2.Text = completeIps + " / " + lines.Length;
                    }));
                }

                string root = "ftp://" + s;

                try
                {
                    //begin recursive dl
                    List<String> data = getFilesInDirectory(root, thrid, s);
                }
                catch
                {

                }

            }
        }

        public List<String> getFilesInDirectory(string dir, int thrid, string rawIP)
        {
            int fileCounter = 0;
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
            bool killswitch = false;

            foreach (string str in dataSpl)
            {
                if (str.Length > 0)
                {
                    string tmp = str.Substring(56);
                    tmp = Regex.Replace(tmp, @"\t|\n|\r", "");
                    string nextURl = dir + "/" + tmp;
                    string processedURL = nextURl.Replace("ftp://", "");

                    //processedURL = processedURL.Replace(" ", "_");

                    //Console.WriteLine(processedURL);

                    if (processedURL.Length > 500)
                    {
                        processedURL = processedURL.Substring(processedURL.Length - 100, 100);
                        processedURL = "..." + processedURL;
                    }               

                    switch (thrid)
                    {
                        case 1:
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                currentFilecount_1++;
                                label1.Text = rawIP.Replace("ftp://", "") + " | " + fileCounter + " files";
                                dataExternal_1.Add(processedURL);
                                File.WriteAllLines(fileCurrentlyWriting_1, dataExternal_1);
                            }));
                            if((currentFilecount_1 > maximumFileCount) && usingMaximumFileCount)
                            {
                                killswitch = true;
                            }                           
                            break;

                        case 2:
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                currentFilecount_2++;
                                label7.Text = rawIP.Replace("ftp://", "") + " | " + fileCounter + " files";
                                dataExternal_2.Add(processedURL);
                                File.WriteAllLines(fileCurrentlyWriting_2, dataExternal_2);
                            }));
                            if ((currentFilecount_2 > maximumFileCount) && usingMaximumFileCount)
                            {
                                killswitch = true;
                            }
                            break;

                        case 3:
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                currentFilecount_3++;
                                label8.Text = rawIP.Replace("ftp://", "") + " | " + fileCounter + " files";
                                dataExternal_3.Add(processedURL);
                                File.WriteAllLines(fileCurrentlyWriting_3, dataExternal_3);
                            }));
                            if ((currentFilecount_3 > maximumFileCount) && usingMaximumFileCount)
                            {
                                killswitch = true;
                            }
                            break;

                        case 4:
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                currentFilecount_4++;
                                label9.Text = rawIP.Replace("ftp://", "") + " | " + fileCounter + " files";
                                dataExternal_4.Add(processedURL);
                                File.WriteAllLines(fileCurrentlyWriting_4, dataExternal_4);
                            }));
                            if ((currentFilecount_4 > maximumFileCount) && usingMaximumFileCount)
                            {
                                killswitch = true;
                            }
                            break;

                        default:
                            break; //should never hit this
                    }

                    if (killswitch)
                    {
                        return new List<string> { };
                    }

                    bool downloadNextFile = false;

                    string file = getFileFromUrl(processedURL);

                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        if (checkBox3.Checked && filesToDownload.Contains(file))
                        {
                            downloadNextFile = true;
                        }
                    }));

                    if (downloadNextFile)
                    {
                        try
                        {
                            WebClient webc = new WebClient();
                            webc.Credentials = new NetworkCredential("anonymous", "anonymous");
                            webc.DownloadFile("ftp://" + processedURL, file);
                        }
                        catch
                        {

                        }
                    }

                    fileCounter++;
                    string sbst = nextURl.Substring(nextURl.Length - 4, 1);
                    if (nextURl.Length < 1000)
                    {
                        if (!isUrlAFile(processedURL))
                        {
                            try
                            {
                                getFilesInDirectory(nextURl, thrid, rawIP);
                            }
                            catch
                            {
                                return new List<string> { };
                            }
                        }
                    }
                }
            }
            return dataFinal;
        }

        public bool isUrlAFile(string url)
        {
            string[] splitBySlash = url.Split('/');
            string finalObj = splitBySlash[splitBySlash.Length - 1];
            string[] splitByDot = finalObj.Split('.');
            string finalPostDor = splitByDot[splitByDot.Length - 1];
            if (finalPostDor.Length > 1 && finalPostDor.Length < 5)
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

        public static int GetNextInt32(RNGCryptoServiceProvider rnd)
        {
            byte[] randomInt = new byte[4];
            rnd.GetBytes(randomInt);
            return Convert.ToInt32(randomInt[0]);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(!(textBox1.Text == ""))
            {
                listBox1.Items.Insert(0, textBox1.Text);
                filesToDownload.Add(textBox1.Text);
                textBox1.Text = "";
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                textBox1.Enabled = true;
                button3.Enabled = true;
                listBox1.Enabled = true;
            }
            else
            {
                textBox1.Enabled = false;
                button3.Enabled = false;
                listBox1.Enabled = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                numericUpDown1.Enabled = true;
            }
            else
            {
                numericUpDown1.Enabled = false;
            }
        }

        public string getFileFromUrl(string url)
        {
            string[] spl = url.Split('/');
            string file = spl[spl.Length - 1];
            return file;
        }
    }
}
