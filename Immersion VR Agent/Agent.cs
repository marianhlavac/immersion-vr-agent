using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Valve.VR;
using System.Diagnostics;

namespace Immersion_VR_Agent {

    enum AgentStatus {
        None, Error, Ready, AppRunning, InLauncher, InTutorial, Quitting
    };

    class Agent {
        private CVRSystem vrSystem;
        private uint currentPid = 0;
        private int launcherPid = 0;
        private string launcherExecutablePath;
        public AgentStatus status;

        public Agent(string executablePath) {
            launcherExecutablePath = executablePath;
        }

        public bool InitializeOpenVR() {
            EVRInitError initError = EVRInitError.None;
            vrSystem = OpenVR.Init(ref initError, EVRApplicationType.VRApplication_Background);

            status = initError == EVRInitError.None ? AgentStatus.Ready : AgentStatus.Error; // TODO: specify, why error happened.

            return initError == EVRInitError.None;
        }

        public bool PollEvents() {
            VREvent_t vrevent = new VREvent_t();

            if (vrSystem.PollNextEvent(ref vrevent, (uint)Marshal.SizeOf(typeof(VREvent_t)))) {
                switch (vrevent.eventType) {
                    case 404: // VREvent_SceneApplicationChanged
                        OnVRApplicationChange(vrevent.data.applicationLaunch.pid);
                        break;
                    case 700: // VREvent_Quit
                        OnVRQuit();
                        break;
                }

                return true;
            }
            return false;
        }

        public void OnVRApplicationChange(uint appPid) {
            Console.WriteLine(appPid);
            if (appPid > 0) {
                currentPid = appPid;
                status = AgentStatus.AppRunning;
            } else if (currentPid != launcherPid) {
                RunImmersionVR();
            }
        }

        public void OnVRQuit() {
            Console.WriteLine("App is quitting.");
            status = AgentStatus.Quitting;
        }

        public string GetRunningAppName() {
            if (status == AgentStatus.AppRunning) {
                return "PID: " + currentPid; // todo: maybe later
            } else {
                return "<none>";
            }
        }

        public int? RunImmersionVR(bool tutorial = false) {
            try {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = launcherExecutablePath;
                startInfo.Arguments = tutorial ? "" : "-launcher";
                Process proc = Process.Start(startInfo);

                status = tutorial ? AgentStatus.InTutorial : AgentStatus.InLauncher;
                launcherPid = proc.Id;

                return proc.Id;
            } catch {
                return null;
            }
        }

        public void ChangeExecutablePath(string executablePath) {
            launcherExecutablePath = executablePath;
        }
    }
}
