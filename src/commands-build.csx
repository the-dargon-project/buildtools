#load "commands.csx"
#load "solution.csx"
#load "helpers.csx"

public Project[] Projects(params string[] names) {
   Project[] projects = new Project[names.Length];
   for (int i = 0; i < projects.Length; i++) {
      projects[i] = new Project(names[i]);
   }
   return projects;
}

public class Project {
   public Project(string name) {
      Name = name;
   }

   public string Name { get; private set; }
}

public static class Build {
   private static readonly string[] kDependencyOperations = new[] { "restore" };
   private static readonly string[] kProjectOperations = new[] { "restore", "publish" };

   public static ICommand Projects(Project[] projects, Project[] dependencies = null) {
      return new LambdaCommand("build", (options) => HandleBuild(options, projects, dependencies));
   }

   public static int HandleBuild(Options options, Project[] projects, Project[] dependencies) {
      var solution = options.Solution;
      foreach (var dependency in dependencies ?? Enumerable.Empty<Project>()) {
         Console.WriteLine("Preparing Dependency '" + dependency.Name + "'.");
         var dependencyPath = Path.Combine(solution.Path, "dependencies", dependency.Name);
         RunOperations(Path.Combine(dependencyPath, "src"), kDependencyOperations);
      }
      foreach (var project in projects) { 
         Console.WriteLine("Building Project '" + project.Name + "'.");
         var projectPath = Path.Combine(solution.Path, "dependencies", project.Name);
         RunOperations(Path.Combine(projectPath, "src"), kProjectOperations);
         RunOperations(Path.Combine(projectPath, "test"), kProjectOperations);
      }
      return 0;
   }

   private static void RunOperations(string path, string[] operations) {
      foreach (var directory in Directory.EnumerateDirectories(path)) {
         foreach (var operation in operations) { 
            Util.Exec(DOTNET + " " + operation + " " + Util.Quote(directory));
            Util.Exec(DOTNET + " " + operation + " " + Util.Quote(directory));
         }
      }
   }
}

public static class Test {
   public static ICommand Projects(Project[] projects) {
      return Common.PerProject("test", projects, TestProject);
   }

   public static void TestProject(Solution solution, Project project, Options options) {
      Console.WriteLine("Testing Project '" + project.Name + "'.");
      var projectPath = Path.Combine(solution.Path, "dependencies", project.Name);
      var testPath = Path.Combine(projectPath, "test");
      foreach (var testSubprojectDirectory in Directory.EnumerateDirectories(testPath)) {
         var testSubprojectName = new DirectoryInfo(testSubprojectDirectory).Name;
         var testDllPath = Path.Combine(testSubprojectDirectory, "bin/Debug/net451/win7-x64", testSubprojectName + ".dll");
         if (!File.Exists(testDllPath)) {
            throw new Exception("Expected but did not find: " + testDllPath);
         }
         var xunitConsolePath = Path.Combine(options.GetProperty("BuildToolsDir"), "dependencies/xunit-1.9.2_64/xunit.console.exe");
         Util.Exec(Util.Quote(xunitConsolePath) + " " + Util.Quote(testDllPath));
      }
   }
}

public static class Common {
   public static ICommand PerProject(string name, Project[] projects, Action<Solution, Project, Options> func) {
      return new LambdaCommand(
         name,
         Handler(projects, func)
      );
   }

   public static Func<Options, int> Handler(Project[] projects, Action<Solution, Project, Options> func) {
      return (options) => {
         var solution = options.Solution;
         foreach (var project in projects) {
            func(solution, project, options);
         }
         return 0;
      };
   }
}