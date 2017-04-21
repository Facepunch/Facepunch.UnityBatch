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

            string fullLogFile = "";

            //
            // Try to delete the log file, while handling access errors
            //
            for (int i=0; i<10; i++ )
            {
                try
                {
                    fullLogFile = System.IO.File.ReadAllText( logPath );
                    System.IO.File.Delete( logPath );
                }
                catch ( System.IO.IOException )
                {
                    Console.WriteLine( $"Couldn't delete {logPath}.. trying again.." );
                    System.Threading.Thread.Sleep( 5000 );
                    continue;
                }
            }

            if ( process.ExitCode != 0 )
            {
                Console.WriteLine( $"Failed: Unity exit code was {process.ExitCode}" );

                if ( fullLogFile .Contains( "Batchmode quit successfully invoked - shutting down!" ) || fullLogFile.Contains( "Exiting batchmode successfully now!" ) )
                {
                    Console.WriteLine( $"Fail seems to have happened when closing unity, rather than during the build. Counting as a success." );
                    return 0;
                }
            }
            else
            {
                Console.WriteLine( $"Success: Unity exit code was {process.ExitCode}" );
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
