using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoboSharp;

namespace RoboCopyErrorTesting
{
    public class RoboCopyProcess
    {
        
        static RoboSharpConfiguration Configuration { get; } = new RoboSharpConfiguration();
        static string RoboCopyPath => Configuration.RoboCopyExe;

        public RoboCopyProcess() { }
        public RoboCopyProcess(string Destination) { Dest = Destination; }

        public string Source { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "SourceFiles");
        public string Dest { get; set; } = "C:\\DEST";
        public List<string> LogLines { get; set; }
        public List<string> ErrLines { get; set; }
        public RoboSharpConfiguration Config => Configuration;

        public void RunRoboCopyProcess()
        {
            Console.WriteLine("Setting Up RoboCopy Process");
            LogLines = new List<string>();
            ErrLines = new List<string>();
            using (Process RC = new Process())
            {
                Console.WriteLine($"- RoboCopy Executable Path: {RoboCopyPath}");
                RC.StartInfo.FileName = RoboCopyPath;
                RC.StartInfo.RedirectStandardOutput = true;
                RC.StartInfo.RedirectStandardError = true;
                RC.StartInfo.UseShellExecute = false;
                RC.OutputDataReceived += RC_OutputDataReceived;
                RC.ErrorDataReceived += RC_ErrorDataReceived;
                RC.StartInfo.Arguments = $"{Source} {Dest} /S /V /R:0 /W:3";
                //Start
                Console.WriteLine($"- Starting RoboCopy Process");
                bool started = RC.Start();
                Assert.IsTrue(started);
                RC.BeginOutputReadLine();
                RC.BeginErrorReadLine();
                RC.WaitForExit();
                Console.WriteLine($"- RoboCopy Process has Exited");
                RC.Dispose();
            }
        }

        private void RC_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e?.Data != null)
                ErrLines.Add(e.Data);
        }

        private void RC_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e?.Data != null)
                LogLines.Add(e.Data);
        }

        public void WriteLogLines(Exception e)
        {
            if (e != null)
            {
                Console.WriteLine($"- Assertion Failed: No Error reported in the log.");
            }

            if (ErrLines.Count > 0)
            {
                Console.WriteLine($"- Error Data Received from Process.ErrorDataReceived): \n");
                foreach (string s in LogLines)
                    Console.WriteLine(s);
            }
            else
            {
                Console.WriteLine($"- No Error Data Received from Process (via Process.ErrorDataReceived)");
            }


            Console.WriteLine($"- Writing Log Lines: \n");
            foreach (string s in LogLines)
                Console.WriteLine(s);

            if (e != null) throw e;
        }

        public void AssertErrorGenerated()
        {
            try
            {
                //If any of the log lines contains 'ERROR', then this will pass the test.
                //If no error is reported, Assert will throw.
                Assert.IsTrue(LogLines.Any(str => str.Contains(Config.ErrorToken)));
                WriteLogLines(null);
            }
            catch (Exception e)
            {
                WriteLogLines(e);
            }
        }
    }

}
