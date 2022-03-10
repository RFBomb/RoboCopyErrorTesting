using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoboSharp;
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace RoboCopyErrorTesting
{

    [TestClass]
    public class RoboCopyValidationUnitTests
    {

        [TestMethod]
        public void NoSourceDirectoryTest()
        {
            Console.WriteLine("This test is using a Directory path that does not exist as the source, and as such an Error should always be reported.");
            Console.WriteLine("");
            Console.WriteLine("If this test passes, an error has been reported by RoboCopy.");
            Console.WriteLine("If this test fails, RoboCopy reported no errors into the log.");
            Console.WriteLine("");
            var RC = new RoboCopyProcess();
            RC.Source = "C:\\SourceDirDoesNotExist";
            RC.RunRoboCopyProcess();
            //Have the RoboCopy class evaluate the results and write out the log lines
            RC.AssertErrorGenerated();
        }

        [TestMethod]
        public void FileLockedTest()
        {
            Console.WriteLine("This test locks a file in the destination to force robocopy to report a failure and an error.");
            Console.WriteLine("");
            Console.WriteLine("If this test passes, an error has been reported by RoboCopy.");
            Console.WriteLine("If this test fails, RoboCopy reported no errors into the log.");
            Console.WriteLine("");
            RoboCopyProcess RC = new RoboCopyProcess();
            //Setup Command Options
            DirectoryInfo d = new DirectoryInfo(RC.Dest);
            Directory.CreateDirectory(RC.Dest);
            if (d.Exists)
            {
                Console.WriteLine($"Destination Exists : Deleting it to ensure fresh run.");
                d.Delete(true);
            }

            //Prep the locked file
            string LockedFile = Path.Combine(RC.Dest, "1024_Bytes.txt");
            //FileInfo LFile = new FileInfo(LockedFile);
            //LFile.Create();

            Console.WriteLine($"Running the Test");
            Console.WriteLine($"- Creating Destination Directory");
            d.Create();
            Console.WriteLine($"- Locking File Path: {LockedFile}");
            var FStream = new StreamWriter(LockedFile);
            FStream.WriteLine("This File Is Locked");
            RC.RunRoboCopyProcess();
            //Close stream
            FStream.Close();
            Console.WriteLine("- Unlocking the Locked file path.");
            //Have the RoboCopy class evaluate the results and write out the log lines
            RC.AssertErrorGenerated();
        }
    }

}
