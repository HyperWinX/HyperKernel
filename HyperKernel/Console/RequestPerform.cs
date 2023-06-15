using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HyperKernel.Console
{
    public static class RequestPerform
    {
        public static void PerformRequest(string[] splittedQuery)
        {
            switch (splittedQuery[0])
            {
                case "help":
                    HelpMenu(splittedQuery);
                    break;
                case "echo":
                    System.Console.WriteLine(Echo(splittedQuery));
                    break;
                case "poweroff":
                    System.Console.WriteLine("Shutting down...");
                    Thread.Sleep(1000);
                    Cosmos.System.Power.Shutdown();
                    break;
                case "reboot":
                    System.Console.WriteLine("Restarting...");
                    Thread.Sleep(1000);
                    Cosmos.System.Power.Reboot();
                    break;
                case "list":
                    List(splittedQuery);
                    break;
                case "createpartition":
                    Create(splittedQuery);
                    break;
                case "deletepartition":
                    Delete(splittedQuery);
                    break;
                case "clear":
                    System.Console.Clear();
                    break;
                case "mountall":
                    MountAll(splittedQuery);
                    break;
                case "mount":
                    Mount(splittedQuery);
                    break;
            }
        }
        private static void HelpMenu(string[] splittedQuery)
        {
            if (splittedQuery.Length < 2)
            {
                System.Console.WriteLine("\nhelp - outputs current menu");
                System.Console.WriteLine("echo <text> - outputs specified text to console");
                System.Console.WriteLine("poweroff - shuts down computer");
                System.Console.WriteLine("reboot - restarts computer");
                System.Console.WriteLine("list <subcommand> - outputs list of objects. Enter \"help list\" to see more info");
                System.Console.WriteLine("createpartition <drive_number> <partition_size> - creates partition on target drive with size in MB");
                System.Console.WriteLine("deletepartition <drive_number> <partition_number> - removes target partition");
                System.Console.WriteLine("clear - clears screen\n");
                return;
            }
            else if (splittedQuery[1] == "list")
            {
                System.Console.WriteLine("list drives - list of drives");
                System.Console.WriteLine("list partitions <drive_number> - list of partitions on the drive");
                return;
            }
            else
            {
                Log.Error("Unknown module");
            }
        }
        private static string Echo(string[] splittedQuery)
        {
            string result = "";
            for (int i = 1; i < splittedQuery.Length; i++)
            {
                result = string.Concat(result, splittedQuery[i], " ");
            }
            return result;
        }
        private static void List(string[] splittedQuery)
        {
            switch (splittedQuery[1])
            {
                case "drives":
                    List<Cosmos.System.FileSystem.Disk> drives = Kernel.fs.GetDisks();
                    int i = 1;
                    foreach (var drive in drives)
                    {
                        System.Console.WriteLine($"\nDrive number: {i}");
                        System.Console.WriteLine($"Is MBR: {drive.IsMBR}");
                        System.Console.WriteLine($"Size: {drive.Size / 1024 / 1024}MB\n");
                        i++;
                    }
                    drives = null;
                    i = 0x00;
                    break;
                case "partitions":
                    if (splittedQuery.Length < 2)
                    {
                        Log.Error("Please pass drive number!");
                        return;
                    }
                    if (!int.TryParse(splittedQuery[2], out _))
                    {
                        Log.Error("Converting failed: not a number");
                        return;
                    }
                    if (int.Parse(splittedQuery[2]) > Kernel.fs.Disks.Count)
                    {
                        Log.Error("Cannot find drive with such number.");
                        return;
                    }
                    var disk = Kernel.fs.Disks[int.Parse(splittedQuery[2]) - 1];
                    if (disk.Partitions.Count == 0)
                    {
                        Log.Warning("There is no partitions on the drive!");
                        disk = null;
                        return;
                    }
                    for (int i1 = 0; i1 < disk.Partitions.Count; i1++)
                    {
                        System.Console.WriteLine($"\nPartition #{i1 + 1}");
                        System.Console.WriteLine($"Sector size: {disk.Partitions[i1].Host.BlockSize} bytes");
                        System.Console.WriteLine($"Sector count: {disk.Partitions[i1].Host.BlockCount}");
                        System.Console.WriteLine($"Partition size: {disk.Partitions[i1].Host.BlockCount * disk.Partitions[i1].Host.BlockSize / 1024uL / 1024uL} MB\n");
                    }
                    disk = null;
                    break;
            }
        }
        private static void Create(string[] splittedQuery)
        {
            if (splittedQuery.Length < 3)
            {
                Log.Error("Incorrect arguments. Please see help.");
                return;
            }
            int arg1, arg2;
            if (!Tools.Tools.TryParse(splittedQuery[1], out arg1) || !Tools.Tools.TryParse(splittedQuery[2], out arg2))
            {
                Log.Error("Incorrect arguments. Please see help.");
                return;
            }
            if (arg1 > Kernel.fs.Disks.Count)
            {
                Log.Error("Cannot find drive with specified number!");
                return;
            }
            if (arg2 / 1024 / 1024 >= Kernel.fs.Disks[arg1].Size / 1024 / 1024)
            {
                Log.Error("Cannot create partition with size bigger or equal to drive size!");
                return;
            }
            try
            {
                Kernel.fs.Disks[arg1].CreatePartition(arg2);
            }
            catch (Exception ex)
            {
                Log.Error("Error occured: " + ex.Message);
            }
        }
        private static void Delete(string[] splittedQuery)
        {
            if (splittedQuery.Length < 3)
            {
                Log.Error("Incorrect arguments. Please see help.");
                return;
            }
            int arg1, arg2;
            if (!Tools.Tools.TryParse(splittedQuery[1], out arg1) || !Tools.Tools.TryParse(splittedQuery[2], out arg2))
            {
                Log.Error("Incorrect arguments. Please see help.");
                return;
            }
            if (arg1 > Kernel.fs.Disks.Count)
            {
                Log.Error("Cannot find drive with specified number!");
                return;
            }
            if (arg2 > Kernel.fs.Disks[arg1].Partitions.Count)
            {
                Log.Error("Cannot find partition with specified number!");
                return;
            }
            try
            {
                Kernel.fs.Disks[arg1].DeletePartition(arg2 - 1);
            }
            catch (Exception ex)
            {
                Log.Error("Error occurred: " + ex.Message);
            }
        }
        private static void MountAll(string[] splittedQuery)
        {
            if (splittedQuery.Length < 2)
            {
                Log.Error("Incorrect aruguments. Please see help.");
                return;
            }
            int arg1;
            if (!Tools.Tools.TryParse(splittedQuery[1], out arg1))
            {
                Log.Error("Incorrect arguments. Please see help.");
                return;
            }
            if (arg1 > Kernel.fs.Disks.Count)
            {
                Log.Error("Cannot find drive with specified number!");
                return;
            }
            try
            {
                Kernel.fs.Disks[arg1].Mount();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }
        private static void Mount(string[] splittedQuery)
        {
            if (splittedQuery.Length < 3)
            {
                Log.Error("Incorrect arguments. Please see help.");
                return;
            }
            int arg1, arg2;
            if (!Tools.Tools.TryParse(splittedQuery[1], out arg1) || !Tools.Tools.TryParse(splittedQuery[2], out arg2))
            {
                Log.Error("Incorrect arguments. Please see help.");
                return;
            }
            if (arg1 > Kernel.fs.Disks.Count)
            {
                Log.Error("Cannot find drive with specified number!");
                return;
            }
            if (arg2 > Kernel.fs.Disks[arg1].Partitions.Count)
            {
                Log.Error("Cannot find partition with specified number!");
                return;
            }
            try
            {
                Kernel.fs.Disks[arg1].MountPartition(arg2);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }
    }
}
