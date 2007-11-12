using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Db4oAdmin;
 
namespace Db4oTools.MSBuild
{
    public class Db4oAssemblyEnhancer : Task
    {
        private ITaskItem[] assemblies;

        [Required]
        public ITaskItem[] Assemblies
        {
            get { return assemblies; }
            set { assemblies = value; }
        }

        private ITaskItem projectDir;

        public ITaskItem ProjectDir
        {
            get { return projectDir; }
            set { projectDir = value; }
        }

        public override bool Execute()
        {
            foreach (ITaskItem assembly in assemblies)
            {
                string assemblyFile = projectDir + assembly.ItemSpec;
                Log.LogWarning(string.Format("Enhancing assembly: {0}", assemblyFile));
                int ret = Enhance(assemblyFile);
                if (ret != 0)
                {
                    string errorMsg = string.Format("Fail to enhance assembly: {0} with return value {1}", assemblyFile, ret);
                    Log.LogError(errorMsg);
                    return false;
                }
                string message = string.Format("Assembly {0} is enhanced successfully.", assemblyFile);
                Log.LogWarning(message);
            }
            return true;
        }

        private int Enhance(string assembly)
        {
            string[] options = new string[] { "-ta", assembly };
            int ret = Program.Main(options);
            return ret;
        }
    }
}
