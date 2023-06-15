using HyperKernel.Console;
using Sys = Cosmos.System;

namespace HyperKernel
{
    public class Kernel : Sys.Kernel
    {
        public static Cosmos.System.FileSystem.CosmosVFS fs;
        protected override void BeforeRun()
        {
            fs = InitializeVFS();
        }

        protected override void Run()
        {
            ConsoleWorker.Work();
        }
    }
}
