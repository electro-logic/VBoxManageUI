using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace VBoxManageUI
{
    public partial class MainWindow : Window
    {
        public List<VBoxHdd> HDDs { get; set; } = new List<VBoxHdd>();
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            if(!File.Exists(ConfigurationManager.AppSettings["VBoxManagePath"]))
            {
                MessageBox.Show("Please update VBoxManagePath in the file VBoxManageUI.exe.config");
                App.Current.Shutdown();
            }
            ListHdd();
        }
        public void ListHdd()
        {
            var vbox_exe = ConfigurationManager.AppSettings["VBoxManagePath"];
            var vbox_process = Process.Start(new ProcessStartInfo(vbox_exe, "list hdds")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            });
            const string UUID = "UUID";
            const string ParentUUID = "Parent UUID";
            const string State = "State";
            const string Type = "Type";
            const string Location = "Location";
            const string Format = "Storage format";
            const string Capacity = "Capacity";
            const string Encryption = "Encryption";
            HDDs.Clear();
            VBoxHdd hdd = null;
            while (!vbox_process.StandardOutput.EndOfStream)
            {
                string line = vbox_process.StandardOutput.ReadLine();
                if (line.StartsWith(UUID))
                {
                    hdd = new VBoxHdd();
                    hdd.UUID = line.Substring(UUID.Length + 1).Trim();
                }
                if (line.StartsWith(ParentUUID))
                {
                    hdd.ParentUUID = line.Substring(ParentUUID.Length + 1).Trim();
                }
                if (line.StartsWith(State))
                {
                    hdd.State = line.Substring(State.Length + 1).Trim();
                }
                if (line.StartsWith(Type))
                {
                    hdd.Type = line.Substring(Type.Length + 1).Trim();
                }
                if (line.StartsWith(Location))
                {
                    hdd.Location = line.Substring(Location.Length + 1).Trim();
                }
                if (line.StartsWith(Format))
                {
                    hdd.Format = line.Substring(Format.Length + 1).Trim();
                }
                if (line.StartsWith(Capacity))
                {
                    hdd.Capacity = line.Substring(Capacity.Length + 1).Trim();
                }
                if (line.StartsWith(Encryption))
                {
                    hdd.Encryption = line.Substring(Encryption.Length + 1).Trim();
                    HDDs.Add(hdd);
                }
            }
            DataContext = null;
            DataContext = this;
        }
        void btnList_Click(object sender, RoutedEventArgs e)
        {
            ListHdd();
        }
        void dg_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dg.SelectedItem != null)
            {
                btnCompact.IsEnabled = true;
            }
            else
            {
                btnCompact.IsEnabled = false;
            }
        }
        void btnCompact_Click(object sender, RoutedEventArgs e)
        {
            var hdd = dg.SelectedItem as VBoxHdd;
            if (hdd==null)
            {
                return;
            }
            if (hdd.Format != "VDI")
            {
                MessageBox.Show("Compacting is currently only available for VDI images");
                return;
            }
            if (hdd.State.StartsWith("locked"))
            {
                MessageBox.Show("Hard Drive is currently locked");
                return;
            }
            var vbox_exe = ConfigurationManager.AppSettings["VBoxManagePath"];
            var vbox_process = Process.Start(vbox_exe, $"modifyhd {hdd.UUID} --compact");
            vbox_process.WaitForExit();            
            MessageBox.Show($"Done. Exit Code {vbox_process.ExitCode}.");
        }
        void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Please Defragment and Zero (ex. \"sdelete.exe c: -z\") your virtual hard drive before compacting it.");
        }
    }
}