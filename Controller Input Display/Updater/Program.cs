using System;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Updater
{
    #region Serialisation classes
    class Release
    {
        public string tag_name { get; set; }
        public string body { get; set; }
        public List<Asset> assets { get; set; }
    }
    class Asset
    {
        public string name { get; set; }
        public string content_type { get; set; }
        public string browser_download_url { get; set; }
    }

    public class UpdaterJob
    {
        public string[] filesToAdd { get; set; }
        public string[] filesToRemove { get; set; }
    }
    #endregion

    class Program
    {
        public static string programDir = AppDomain.CurrentDomain.BaseDirectory;
        public static bool uUpdated = false;
        public static Version Current = null;

        public static Process ParentProcess = null;
        static void Main(string[] args)
        {
            Console.WriteLine("Controller Input Display - Updater 1.0");
            Console.WriteLine();

            foreach (string arg in args)
            {
                if (arg.StartsWith("-v:\'"))
                {
                    Current = Version.Parse(arg.Split('\'')[1]);
                }
                
                if (arg.StartsWith("-pid:"))
                {
                    string PIDstring = arg.Substring(arg.IndexOf(':') + 1);
                    ParentProcess = Process.GetProcessById(int.Parse(PIDstring));
                }

                if (arg == "-resume")
                {
                    try
                    {
                        string json = File.ReadAllText("remUpdates.json");
                        List<DirectoryInfo> remainingReleases = JsonConvert.DeserializeObject<List<DirectoryInfo>>(json);
                        uUpdated = true;
                        AnalyseRelease(ref remainingReleases);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Something went wrong " + e.Message);
                    }
                }
            }
            if(Current == null)
            {
                Exit("No version given to start the updater");
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

            var task = CheckUpdates();
            task.Wait();

            string output = task.Result;
            if (output == "HttpRequestException")
            {
                Exit("Web Request failed, check if your internet is working\nUpdate checking failed");
            }
            if (output.StartsWith("error code "))
            {
                Exit("API returned " + output + "\nUpdate checking failed");
            }

            //parse the github api response
            Release[] releases = null;
            try
            {
                releases = JsonConvert.DeserializeObject<Release[]>(output);
            }
            catch
            {
                Exit("Invalid API response\nUpdate checking failed");
            }

            //only take the newest and compatible updates
            List<Release> releasesToDownload = new List<Release>();
            for (int i = 0; i < releases.Length; i++)
            {
                Version releaseVersion = ParseTagName(releases[i].tag_name);
                if(Current.CompareTo(releaseVersion) < 0)
                {
                    if (releases[i].assets[0].content_type == "application/x-zip-compressed")
                    {
                        releasesToDownload.Add(releases[i]);
                    }
                }
            }
            
            if (releasesToDownload.Count != 0)
            {
                releasesToDownload.Reverse(); //start from oldest to newest
                foreach (Release update in releasesToDownload)
                {
                    Console.WriteLine(update.tag_name);
                    Console.WriteLine(update.body);
                }
                Console.WriteLine();
                Console.WriteLine();

                Console.WriteLine("There are new updates available");
                Console.WriteLine("Would you like to download the updates? Answer by typing Y or N");

                ConsoleKey key;
                do
                {
                    key = Console.ReadKey().Key;
                    Console.WriteLine();
                    if(key == ConsoleKey.Y || key == ConsoleKey.N)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Enter either Y or N");
                    }
                }
                while (true);

                if (key == ConsoleKey.N)
                {
                    Exit("Updating cancelled");
                }


                Console.Clear();
                try { ParentProcess.Kill(); } catch(Exception e) { Console.WriteLine(e.Message); }

                Console.WriteLine("Backuping old version");

                if (Directory.Exists("old"))
                {
                    Directory.Delete("old", true);
                }
                CopyDirectory(programDir, "old");

                List<DirectoryInfo> downloadedReleases = DownloadUpdates(ref releasesToDownload);

                AnalyseRelease(ref downloadedReleases);                
            }
            Exit("No new updates");
        }

        private static void AnalyseRelease(ref List<DirectoryInfo> releases)
        {
            List<DirectoryInfo> remainingUpdates = new List<DirectoryInfo>(releases);
            for (int i = 0; i < releases.Count; i++)
            {
                Console.WriteLine($"Installing extracted update {i + 1} of {releases.Count}");
                string updateDir = releases[i].FullName;
                using (StreamReader reader = File.OpenText(updateDir + "\\updaterjob.json"))
                using (JsonTextReader treader = new JsonTextReader(reader))
                {
                    JObject jsonObj = (JObject)JToken.ReadFrom(treader);

                    if (jsonObj["uUpdater"].ToObject<bool>() && uUpdated == false)
                    {
                        Console.WriteLine("Update contains an update for the updater");
                        Console.WriteLine(jsonObj["uUpdaterScriptPath"]);

                        File.WriteAllText("remUpdates.json", JsonConvert.SerializeObject(remainingUpdates));

                        Process uupdate = new Process()
                        {
                            StartInfo = new ProcessStartInfo()
                            {
                                FileName = updateDir + "\\" + jsonObj["uUpdaterScriptPath"],
                                WorkingDirectory = programDir,
                                Arguments = Process.GetCurrentProcess().Id.ToString() + " " + releases[i].Name + "\\",
                                Verb = "runas"
                            }
                        };
                        uupdate.Start();
                        Console.WriteLine("Started updater script");
                        Console.ReadKey();
                    }

                    //jsonObj["updaterJobVersion"]; //UpdaterJob file version, useful for later when there's gonna be updates to the UpdaterJob itself
                    UpdaterJob updaterJob = jsonObj["UpdaterJob"].ToObject<UpdaterJob>();

                    InstallUpdate(ref updaterJob, updateDir);
                    Directory.Delete(updateDir, true);
                }
                Console.WriteLine($"Installed update {i + 1} of {releases.Count}{"\n"}");
                remainingUpdates.RemoveAt(i);
                uUpdated = false;
            }

            Exit("Updates have finished installing");
        }
        
        public static void InstallUpdate(ref UpdaterJob job, string updateDir)
        {
            foreach (string filePath in job.filesToAdd)
            {
                try
                {
                    File.Copy(updateDir + "\\" + filePath, filePath, true);
                }
                catch (DirectoryNotFoundException)
                {
                    Directory.CreateDirectory(filePath.Substring(0, filePath.LastIndexOf("\\")));
                    File.Copy(updateDir + "\\" + filePath, filePath, true);
                }
                catch { Console.WriteLine("Failed to copy the file" + filePath); }
            }

            foreach (string filePath in job.filesToRemove)
            {
                try
                {
                    File.Delete(updateDir + "\\" + filePath);
                }
                catch { Console.WriteLine("Failed to delete the file" + filePath); }
            }
        }

        public static void Exit(string message = "")
        {
            Console.WriteLine(message);

            int ExitDelay = 10 * 1000;
            Console.WriteLine("Exiting in {0}s", ExitDelay / 1000);
            Thread.Sleep(ExitDelay);
            Environment.Exit(0);
        }
        static Version ParseTagName(string valueToParse)
        {
            try
            {
                return Version.Parse(valueToParse.Remove(0, 1));
            }
            catch
            {
                return new Version("1.0");
            }
        }

        static List<DirectoryInfo> DownloadUpdates(ref List<Release> releases)
        {
            WebClient web = new WebClient();
            List<DirectoryInfo> downloadedReleases = new List<DirectoryInfo>();
            for (int i = 0; i < releases.Count; i++)
            {
                string updateName = "update" + i;
                try
                {
                    Console.WriteLine($"Downloading update {i + 1} of {releases.Count}");
                    web.DownloadFile(releases[i].assets[0].browser_download_url, updateName + ".zip");
                    Console.WriteLine("Download complete");
                }
                catch
                {
                    Console.WriteLine("Couldn't download the update");
                    continue;
                }

                try
                {
                    if (Directory.Exists(updateName))
                    {
                        Directory.Delete(updateName, true);
                    }

                    ZipFile.ExtractToDirectory(updateName + ".zip", updateName);
                    Console.WriteLine("Extraction of the update complete");

                    downloadedReleases.Add(new DirectoryInfo(updateName));
                    File.Delete(updateName + ".zip");
                }
                catch
                {
                    Console.WriteLine($"Unable to extract update {i + 1} of {releases.Count}");
                    continue;
                }
            }

            return downloadedReleases;
        }

        static async Task<string> CheckUpdates()
        {
            HttpResponseMessage response;
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Controller-Input-Display-Updater/1.0");
                response = await client.GetAsync("https://api.github.com/repos/R3FR4G/Controller-Input-Display/releases");
            }
            catch (HttpRequestException)
            {
                return "HttpRequestException";
            }
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return "error code " + ((int)response.StatusCode).ToString() + " (" + response.StatusCode + ")";
            }

            HttpContent responseContent = response.Content;
            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                return await reader.ReadToEndAsync();
            }
        }

        //https://docs.microsoft.com/fr-fr/dotnet/standard/io/how-to-copy-directories
        private static void CopyDirectory(string sourceDirName, string destDirName, bool onlyCopyNew = false)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();

            //original way of getting which files were supposed to be copied
            if (onlyCopyNew)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    string tempPath = Path.Combine(destDirName, files[i].Name);
                    if (File.Exists(tempPath))
                    {
                        DateTime destFileLastWriteTime = File.GetLastWriteTime(tempPath);
                        if (files[i].LastWriteTime >= destFileLastWriteTime)
                        {
                            files[i].CopyTo(tempPath, true);
                        }
                    }
                    else
                    {
                        files[i].CopyTo(tempPath, true);
                    }
                }
            }
            else
            {
                foreach (FileInfo file in files)
                {
                    string tempPath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(tempPath, true);
                }
            }

            // If copying subdirectories, copy them and their contents to new location.
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                CopyDirectory(subdir.FullName, tempPath, onlyCopyNew);
            }
        }
    }
}