using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Xml;
using Microsoft.Win32;
using System.Xml.Linq;
using System.Reflection;
using System.Security.Principal;


namespace Win11Check
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern UInt32
        GetFirmwareEnvironmentVariableA(string lpName, string lpGuid, IntPtr pBuffer, UInt32 nSize);
        const int ERROR_INVALID_FUNCTION = 1;
        FindEspec FE = new FindEspec();
        string TPMAct = null, TPMEna = null, SpecVersion = null;
        string architecture = null, cores = null, clock = null, clockshort = null, name = null, threads = null, nameshort = null;
        string TotalRAM = null, storage = null;
        string drx = null, WDDMv = null, secure = null, checkUEFI = null;
        string[] proname;
        XDocument doc;
        XElement element;
        double sv;

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Shown(object sender, EventArgs e)
        {
          lbV.Text = "V " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
          CPU();
          TPM();
          RAM();
          UEFI();
          Storage();
          DirectX();
          Secureboot();
        }

        public void TPM()
        {
            try
            {
               FE.InsertInfo("root\\CIMV2\\Security\\MicrosoftTpm", "Win32_Tpm");

                foreach (ManagementObject searchTPM in FE.seacherComp.Get())
                {
                    try
                    {

                        if (searchTPM["IsActivated_InitialValue"] != null)
                        {
                            TPMAct = searchTPM["IsActivated_InitialValue"].ToString();

                        }
                        if (searchTPM["IsEnabled_InitialValue"] != null)
                        {
                            TPMEna = searchTPM["IsEnabled_InitialValue"].ToString();

                        }
                        if (searchTPM["SpecVersion"] != null)
                        {
                            SpecVersion = searchTPM["SpecVersion"].ToString();                    
                            if (SpecVersion.Length > 3)
                                SpecVersion = SpecVersion.Substring(0, 3);

                            sv = double.Parse(SpecVersion);

                        }
              
                        if (TPMAct == "True" && TPMEna == "True" && sv > 2.0)
                        {
                            txtTPM.AppendText ("\r\n Version: " + SpecVersion );
                            this.picTPM.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "check.png");
                            picTPM.Refresh();
                        }
                        else if (TPMAct == "True" && TPMEna == "True" && sv < 2.0)
                        {
                            txtTPM.AppendText("\r\n Version incompatible: " + SpecVersion);
                            this.picTPM.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "x_30465_640.png");
                            picTPM.Refresh();
                        }
                        else
                        {
                            txtTPM.AppendText("\r\n TPM not enabled or activated ");
                            this.picTPM.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "x_30465_640.png");
                            picTPM.Refresh();
                        }
                    }
                    catch (Exception)
                    {
                        txtTPM.AppendText("\r\n TPM not found ");
                        this.picTPM.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "x_30465_640.png");
                        picTPM.Refresh();
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Please, run to administrator", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
            }

        }

        public void CPU()
        {
            FE.InsertInfo("", "Win32_Processor");

            foreach (ManagementObject moProcessor in FE.seacherComp.Get())
            {
                if (moProcessor["Architecture"] != null)
                {
                    architecture = moProcessor["Architecture"].ToString();

                    if (int.Parse(architecture) == 0)
                    {
                        architecture = "x32";
                        this.picArc.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "x_30465_640.png");
                        picArc.Refresh();
                    }
                    else if (int.Parse(architecture) == 1)
                    {
                        architecture = "MIPS";
                        this.picArc.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "x_30465_640.png");
                        picArc.Refresh();
                    }
                    else if (int.Parse(architecture) == 2)
                    {
                        architecture = "Alpha";
                        this.picArc.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "x_30465_640.png");
                        picArc.Refresh();
                    }
                    else if (int.Parse(architecture) == 3)
                    {
                        architecture = "PowerPC";
                        this.picArc.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "x_30465_640.png");
                        picArc.Refresh();
                    }
                    else if (int.Parse(architecture) == 5)
                    {
                        architecture = "ARM";
                        this.picArc.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "check.png");
                        picArc.Refresh();
                    }
                    else if (int.Parse(architecture) == 6)
                    {
                        architecture = "ia64";
                        this.picArc.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "check.png");
                        picArc.Refresh();
                    }
                    else
                    {
                        architecture = "x64";
                        this.picArc.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "check.png");
                        picArc.Refresh();
                    }
                }

                if (moProcessor["NumberOfCores"] != null)
                {
                    cores = moProcessor["NumberOfCores"].ToString();

                    if (int.Parse(cores) >= 2)
                    {
                        this.picCores.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "check.png");
                        picCores.Refresh();
                    }
                    else
                    {
                        this.picCores.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "x_30465_640.png");
                        picCores.Refresh();
                    }
                }

                if (moProcessor["NumberOfLogicalProcessors"] != null)
                {
                    threads = moProcessor["NumberOfLogicalProcessors"].ToString();
                }

                if (moProcessor["Name"] != null)
                {
                    name = moProcessor["Name"].ToString();

                    proname = name.Split(' ');

                    if (proname[0] == "Intel(R)")
                    {              
                        doc = XDocument.Load("Resource\\SupportedProcessorsIntel.xml");

                        if (proname[2] == "Silver" || proname[2] == "Gold" || proname[2] == "Platinum" || proname[2] == "Bronze" || proname[2] == "CPU")
                            nameshort = proname[1] + " " + proname[2] + " " + proname[3];

                        else if (proname[1] == "Core(TM)" || proname[1] == "Atom(R)" || proname[1] == "Celeron(R)" || proname[1] == "Xeon(R)")
                            nameshort = proname[1] + " " + proname[2];
                    }

                    if (proname[0] == "AMD")
                    {
                        doc = XDocument.Load("Resource\\SupportedProcessorsAMD.xml");

                        if (proname[3] == "PRO")
                            nameshort = proname[1] + " " + proname[2] + " " + proname[3] + " " + proname[4];

                        else if (proname[1] == "Athlon" || proname[1] == "EPYC")
                            nameshort = proname[1] + " " + proname[2];

                        else if (proname[2] == "Gold" || proname[2] == "Silver" || proname[1] == "Ryzen")
                            nameshort = proname[1] + " " + proname[2] + " " + proname[3];
                        else
                            nameshort = proname[1];
                    }

                    if(proname[0] == "Qualcomm")
                    {
                        doc = XDocument.Load("Resource\\SupportedProcessorsQualcomm.xml");

                        if (proname[1] == "PRO")
                            nameshort = proname[1] + " " + proname[2] + " " + proname[3];
                        else if (proname[1] == "Microsoft")
                            nameshort = proname[1] + " " + proname[2];
                    }

                }

                if (moProcessor["MaxClockSpeed"] != null)
                {
                    clock = moProcessor["MaxClockSpeed"].ToString();
                    clockshort = moProcessor["MaxClockSpeed"].ToString();

                    if (clock.Length > 3)
                        clock = clock.Substring(0, 1);
                        clock += ".";
                        clock += clockshort.Substring(1, 2);

                    if(double.Parse(clock) >= 1.0)
                    {
                        this.picFreq.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "check.png");
                        picFreq.Refresh();
                    }
                    else
                    {
                        this.picFreq.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "x_30465_640.png");
                        picFreq.Refresh();
                    }
                }

            }

            try
            {
                if (proname[0] == "Intel(R)")
                {
                    element = doc.Element("ProcessorIntelList").Descendants("processor").Where(a => a.Value.Equals(nameshort)).First();
                }
                else if (proname[0] == "AMD")
                {
                    element = doc.Element("ProcessorAMDList").Descendants("processor").Where(a => a.Value.Equals(nameshort)).First();
                }
                else if (proname[0] == "Qualcomm")
                {
                    element = doc.Element("ProcessorQualcommList").Descendants("processor").Where(a => a.Value.Equals(nameshort)).First();
                }


                    if (element.Value == nameshort)
                {
                    this.picCPU.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "check.png");
                    picCPU.Refresh();
                }
                else
                {
                    this.picCPU.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "x_30465_640.png");
                    picCPU.Refresh();
                }
            }
            catch
            {
                this.picCPU.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "x_30465_640.png");
                picCPU.Refresh();
            }
     
            txtArc.AppendText("\r\n" + architecture);
            txtFreq.AppendText("\r\n" + clock + " GHz");
            txtCores.AppendText("\r\n Cores: " + cores + "\r\n Threads: " + threads);
            txtName.AppendText(name);
        }

        public void RAM()
        {
            FE.InsertInfo("", "Win32_OperatingSystem");

            foreach (ManagementObject moRAM in FE.seacherComp.Get())
            {
                if (moRAM["TotalVisibleMemorySize"] != null)
                {
                    TotalRAM = moRAM["TotalVisibleMemorySize"].ToString();
                    double gb = float.Parse(TotalRAM) / 1024 / 1024;
                    gb = Math.Round(gb);
                    TotalRAM = gb.ToString();

                    if(gb >= 4)
                    {
                        this.picRAM.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "check.png");
                        picRAM.Refresh();
                    }
                    else
                    {
                        this.picRAM.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "x_30465_640.png");
                        picRAM.Refresh();
                    }
                }
            }
            txtRam.AppendText("\r\n" + TotalRAM + " GB");
        }

        public void UEFI()
        {
                GetFirmwareEnvironmentVariableA("", "{00000000-0000-0000-0000-000000000000}", IntPtr.Zero, 0);

                if (Marshal.GetLastWin32Error() == ERROR_INVALID_FUNCTION)
                {
                    checkUEFI = "Legacy";
                    this.picBOOT.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "x_30465_640.png");
                    picBOOT.Refresh();

                }
                else
                {
                    checkUEFI = "UEFI";
                    this.picBOOT.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "check.png");
                    picBOOT.Refresh();
                }

            txtBoot.AppendText("\r\n" + checkUEFI);
        }

        public void Storage()
        {
            FE.InsertInfo("", "Win32_DiskDrive");

            foreach (ManagementObject moStorage in FE.seacherComp.Get())
            {
                if (moStorage["Size"] != null)
                {
                    storage = moStorage["Size"].ToString();       
                    double gb = float.Parse(storage) / 1024 / 1024 / 1024;
                    storage = gb.ToString();
                    if (storage.Length > 3)
                        storage = storage.Substring(0, 3);

                    if (gb >= 64)
                    {
                        this.picStorage.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "check.png");
                        picStorage.Refresh();
                    }
                    else
                    {
                        this.picStorage.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "x_30465_640.png");
                        picStorage.Refresh();
                    }
                }
            }
            txtStorage.AppendText("\r\n" + storage + " GB");

        }

        public void DirectX()
        {
            Process.Start("dxdiag", "/x dxv.xml");
            while (!File.Exists("dxv.xml"))
                Thread.Sleep(1000);
            XmlDocument doc = new XmlDocument();
            doc.Load("dxv.xml");
            XmlNode dxd = doc.SelectSingleNode("//DxDiag");
            XmlNode dxv = dxd.SelectSingleNode("//DirectXVersion");
            XmlNode wddm = dxd.SelectSingleNode("//DriverModel");

            drx = dxv.InnerText.Split(' ')[1].ToString();
            WDDMv = wddm.InnerText.Split(' ')[1].ToString();

            if (int.Parse(drx) >= 12 && double.Parse(WDDMv) >= 2.0)
            {
                this.picDxv.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "check.png");
                picDxv.Refresh();
            }
            else
            {
                this.picDxv.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "x_30465_640.png");
                picDxv.Refresh();
            }

            txtDirect.AppendText("\r\n DirectX " + drx + " / WDDM " + WDDMv);
        }

        public void Secureboot()
        {
            int rc = 0;
            string key = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecureBoot\State";
            string subkey = @"UEFISecureBootEnabled";
            try
            {
                object value = Registry.GetValue(key, subkey, rc);
                if (value != null)
                    rc = (int)value;
            }
            catch { }
            secure = ($@"{(rc >= 1 ? "Enable" : "Disable")}");

            if(secure == "Enable")
            {
                this.picSecure.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "check.png");
                picSecure.Refresh();
            }
            else
            {
                this.picSecure.Image = Image.FromFile(Application.StartupPath + "\\Resource\\" + "x_30465_640.png");
                picSecure.Refresh();
            }

            txtSecure.AppendText("\r\n" + secure);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnGit_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/NAT4N");
            }
            catch
            {
                MessageBox.Show("Unable to open link that was clicked.");
            }

        }

        private void btnKofi_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://ko-fi.com/nat4n");
            }
            catch
            {
                MessageBox.Show("Unable to open link that was clicked.");
            }
        }


    }
}
