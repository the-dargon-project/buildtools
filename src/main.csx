#load "options.csx"

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

public static class Program {
   public static int Main(string[] args) {
      var dir = new DirectoryInfo(Environment.CurrentDirectory);
      while (dir != null) {
         var startScript = Path.Combine(dir.FullName, "build_entry.csx");
         if (File.Exists(startScript)) {
            return ExecuteBuildConfigScript(startScript, args);
         } else {
            dir = dir.Parent;
         }
      }
      Console.Error.WriteLine("Could not locate build_entry.csx.");
      return 1;
   }

   private static int ExecuteBuildConfigScript(string path, string[] args) {
      //Workaround: Scriptcs creates scriptcs_nuget.config
      var nonprojectDirectory = new FileInfo(path).Directory.EnumerateDirectories().FirstOrDefault();
      if (nonprojectDirectory != null) {
         Environment.CurrentDirectory = nonprojectDirectory.FullName;
      }
      var process = Process.Start(
         new ProcessStartInfo(
            "scriptcs",
            WrapWithQuotes(path) + " -- " +
            "--SolutionDir=\"" + new FileInfo(path).Directory.FullName + "\"" + " " +
            String.Join(" ", args.Select(WrapWithQuotes))
         ) {
            UseShellExecute = false,
         });
      process.WaitForExit();
      return process.ExitCode;
   }

   private static string WrapWithQuotes(string s) {
      return "\"" + s + "\"";
   }
}

Environment.Exit(Program.Main(Env.ScriptArgs.ToArray()));