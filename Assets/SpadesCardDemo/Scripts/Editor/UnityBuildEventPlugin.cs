using UnityEngine;
using System.IO.Ports;
using System.Threading;
using UnityEditor;
using UnityEditor.Callbacks;

[ExecuteInEditMode]
public class UnityBuildEventPlugin {
    private const string BUILD_COMPLETE = "b";
    private const string COMPILE_COMPLETE = "c";

    private static SerialPort stream = new SerialPort("/dev/cu.usbmodemFA121", 9600);

    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
        Write(BUILD_COMPLETE);

    }

    [DidReloadScripts]
    private static void OnScriptsReloaded() {
        Write(COMPILE_COMPLETE);
    }

    private static void Write(string message) {
        if (stream.IsOpen) {
            stream.Write(message);
            stream.BaseStream.Flush();
        }
        else {
            new Thread(() => {
                stream.Open();
                Thread.Sleep(2000);
                stream.Write(message);
                stream.BaseStream.Flush();
            }).Start();
        }
    }
}
