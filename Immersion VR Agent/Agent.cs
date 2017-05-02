using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Valve.VR;

namespace Immersion_VR_Agent {

    enum AgentStatus {
        None, Error, Ready, AppRunning, InLauncher, InTutorial, Quitting
    };

    class Agent {
        private CVRSystem vrSystem;
        private uint currentPid = 0;
        public AgentStatus status;

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
            if (appPid > 0) {
                Console.WriteLine("Some app is running now.");
                currentPid = appPid;
                status = AgentStatus.AppRunning;
            } else {
                Console.WriteLine("The app has been closed. Now it's time to run our launcher.");
                status = AgentStatus.Ready;
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
    }
}
