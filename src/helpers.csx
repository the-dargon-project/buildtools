using System;
using System.Diagnostics;

const string DOTNET = "dotnet";

public static class Util {
   public static T[] SubArray<T>(T[] arr, int index, int length) {
      T[] result = new T[length];
      Array.Copy(arr, index, result, 0, length);
      return result;
   }

   public static T[] SubArray<T>(T[] arr, int index) {
      return SubArray(arr, index, arr.Length - index);
   }

   public static int Exec(string command) {
      var process = Process.Start(
         new ProcessStartInfo("cmd", "/s /c " + Quote(command)) {
            UseShellExecute = false
         });
      process.WaitForExit();
      return process.ExitCode;
   }

   public static void Exit(int code) {
      Environment.Exit(code);
   }

   public static string Quote(string s) {
      return "\"" + s + "\"";
   }
}