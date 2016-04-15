#load "helpers.csx"
#load "solution.csx"

using System;
using System.Collections.Generic;
using System.Linq;

public class Options {
   public string Command { get; set; }
   public string[] RemainingArgs { get; set; }
   public HashSet<string> Flags { get; set; }
   public Dictionary<string, string> Properties { get; set; } 
   public Options Parent { get; set; }
   public Solution Solution { get; set; }
   public string GetProperty(string name, string fallback = null) {
      string result;
      if (Properties.TryGetValue(name, out result)) {
         return result;
      } else {
         return fallback;
      }
   }
   public string GetPropertyOrThrow(string name) {
      string result;
      if (Properties.TryGetValue(name, out result)) {
         return result;
      } else {
         throw new Exception("Command-line property required: '" + name +"'.");
      }
   }
   public Options WithSolution(Solution solution) {
      this.Solution = solution;
      return this;
   }

   public static Options From(IReadOnlyList<string> args) {
      return From(new Options {
         Flags = new HashSet<string>(),
         Properties = new Dictionary<string, string>(),
         RemainingArgs = args.ToArray()
      });
   }

   public static Options From(Options previous) {
      var result = new Options {
         Flags = new HashSet<string>(previous.Flags),
         Properties = new Dictionary<string, string>(previous.Properties),
         Parent = previous
      };
      var argsIndex = 0;
      for (; argsIndex < previous.RemainingArgs.Length; argsIndex++) {
         var arg = previous.RemainingArgs[argsIndex];
         if (arg[0] != '-') break;
         if (arg[1] != '-') {
            result.Flags.Add(arg.Substring(1));
         } else {
            var delimiter = arg.IndexOf("=");
            result.Properties[arg.Substring(2, delimiter - 2)] = arg.Substring(delimiter + 1);
         }
      }
      if (argsIndex < previous.RemainingArgs.Length) {
         result.Command = previous.RemainingArgs[argsIndex];
         result.RemainingArgs = Util.SubArray(previous.RemainingArgs, argsIndex + 1);
      } else {
         result.RemainingArgs = Util.SubArray(previous.RemainingArgs, argsIndex);
      }
      return result;
   }
}