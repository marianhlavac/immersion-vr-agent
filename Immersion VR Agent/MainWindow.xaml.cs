using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Valve.VR;

namespace Immersion_VR_Agent {
    public partial class MainWindow : Window {
        private readonly BackgroundWorker worker = new BackgroundWorker {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };
        private CVRSystem vrSystem;

        public MainWindow() {
            InitializeComponent();

            if (InitializeOpenVR()) {
                statusIcon.Source = Utils.LoadBitmapFromResource("openvr-good@2x.png");
                statusLabel.Content = "Připraveno.";
                statusDescription.Content = "Ve fázi výuky";
            } else {
                statusIcon.Source = Utils.LoadBitmapFromResource("openvr-error@2x.png");
                statusLabel.Content = "Chyba!";
                statusDescription.Content = "Nepodařilo se inicializovat \nOpenVR.";
            }

            StartUpdating();
        }

        public bool InitializeOpenVR() {
            EVRInitError initError = EVRInitError.None;
            vrSystem = OpenVR.Init(ref initError, EVRApplicationType.VRApplication_Background);

            if (initError == EVRInitError.None) {
                worker.DoWork += worker_DoWork;
                worker.ProgressChanged += worker_OnProgressChanged;
                return true;
            }
            else {
                return false;
            }
        }

        public void StartUpdating() {
            worker.RunWorkerAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e) {
            BackgroundWorker worker = (BackgroundWorker)sender;
            while (!worker.CancellationPending) {
                VREvent_t vrevent = new VREvent_t();
                if (vrSystem.PollNextEvent(ref vrevent, (uint)Marshal.SizeOf(typeof(VREvent_t)))) {
                    worker.ReportProgress(0, vrevent);
                }
            }
        }

        private void worker_OnProgressChanged(object sender, ProgressChangedEventArgs e) {
            VREvent_t vrevent = (VREvent_t) e.UserState;
            if (vrevent.eventType != 0) {
                Console.WriteLine("Polled: " + vrevent.eventType.ToString());
            }
        }
    }
}
