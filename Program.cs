using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Facepunch.UnityBatch
{

    class Program
    {
        static int Main( string[] args )
        {
            var unityVersion = "1.2.3";
            var projectPath = "project/path";

            for (int i = 0; i<args.Length; i++ )
            {
                if ( args[i] == "-unityVersion" ) unityVersion = args[i + 1];
                if ( args[i] == "-projectPath" ) projectPath = args[i + 1];

            }

            return RunUnity( unityVersion, projectPath, string.Join( " ", args ) );
        }

        private static int RunUnity( string unityVersion, string projectPath, string fullOptions )
        {
            var unityPath = $"C:/Program Files/Unity {unityVersion}/Editor/Unity.exe";
            var logPath = System.IO.Path.GetTempFileName();
            var commandLine = $"-silent-crashes -no-dialogs -batchmode -quit {fullOptions} -logFile \"{logPath}\"";

            if ( !System.IO.File.Exists( unityPath ) )
            {
                Console.WriteLine( $"Couldn't find unity version: {unityPath}" );
                return 1;
            }

            var process = new Process();
            process.StartInfo.FileName = unityPath;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.WorkingDirectory = projectPath;
            process.StartInfo.ErrorDialog = false;
            process.StartInfo.Arguments = commandLine;

            Console.WriteLine( process.StartInfo.FileName );
            Console.WriteLine( process.StartInfo.Arguments );
            Console.WriteLine();

            Console.WriteLine( "Running...." );

            process.Start();

            using ( FileStream stream = File.Open( logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) )
            {
                using ( StreamReader reader = new StreamReader( stream ) )
                {
                    while ( !process.HasExited )
                    {
                        PrintFromLog( reader );
                        System.Threading.Thread.Sleep( 500 );
                    }

                    System.Threading.Thread.Sleep( 500 );
                    PrintFromLog( reader );
                }
            }

            //
            // Try to delete the log file
            //
            for (int i=0; i< 5; i++ )
            {
                try
                {
                    System.IO.File.Delete( logPath );
                    break;
                }
                catch ( System.IO.IOException )
                {
                    Console.WriteLine( $"Couldn't delete {logPath}.. trying again.." );
                    System.Threading.Thread.Sleep( 1000 );
                    continue;
                }
            }

            return process.ExitCode;
        }

        private static void PrintFromLog( StreamReader logStream )
        {
            var txt = logStream.ReadToEnd();
            if ( string.IsNullOrEmpty( txt ) ) return;

            Console.Write( txt );
        }
    }
}
