using System;
using System.Drawing;
using System.IO;
using Console = Colorful.Console;
using LunarAccountTool.Helpers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;

namespace LunarAccountTool
{
    internal class Program
    {
        public static readonly string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public static string lunarAcccountsPath = Path.Combine(userFolder, ".lunarclient/settings/game/accounts.json");
        public static readonly string ver = "v1.7";
        public static readonly string ownertag = "Tagline";
        static async Task<bool> CheckForUpdates()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string latestVersion = await client.GetStringAsync("YOUR_VERSION_FILE_URL_HERE");
                    latestVersion = latestVersion.Trim();

                    if (latestVersion != ver)
                    {
                        Console.WriteLine($"A new version ({latestVersion}) is available!");
                        return true;
                    }
                }
            }
            catch { }

            Console.WriteLine("Your software is up to date.");
            return false;
        }

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Checking for updates...");
            bool hasUpdate = await CheckForUpdates();

            if (hasUpdate)
            {
                Console.WriteLine("Do you want to download the update? (y/n)");
                var ans = Console.ReadLine().ToLower();

                if (ans == "y")
                {
                    System.Diagnostics.Process.Start(new ProcessStartInfo
                    {
                        FileName = "YOUR_DOWNLOAD_LINK_HERE",
                        UseShellExecute = true
                    });
                }
            }

            AccountManager.LoadJson();
            bool continueProgram = true;

            while (continueProgram)
            {
                ClearScreen();
                Console.Title = "Lunar Account Tool";

                PrintMenu();

                Console.Write("[", Color.Green);
                Console.Write(DateTime.Now.ToString("HH:mm:ss"), Color.Green);
                Console.Write("] > ", Color.Green);
                Console.Write("Please type your option (1-4) here: ", Color.Green);

                string choice = System.Console.ReadLine()?.Trim();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            CreateAccountPrompt();
                            break;
                        case "2":
                            RemoveAccountsMenu();
                            break;
                        case "3":
                            AccountManager.ViewInstalledAccounts();
                            break;
                        case "4":
                            ConsoleHelpers.PrintLine("INFO", "Exiting the program....", Color.Green);
                            continueProgram = false;
                            break;
                        default:
                            ConsoleHelpers.PrintLine("ERROR", "Invalid option. Please pick an option (1-4).", Color.Green);
                            break;
                    }
                }
                catch (Exception e)
                {
                    ConsoleHelpers.PrintLine("ERROR", "An error occurred: " + e.Message, Color.Green);
                }

                if (continueProgram)
                {
                    ConsoleHelpers.PrintLine("INFO", "Press any key to return to the main menu...", Color.Green);
                    System.Console.ReadKey(true);
                }
            }

            AccountManager.SaveJson();
        }

        private static void PrintMenu()
        {
            string title = @"
 _                              _____ _ _            _    ___  ___                                  
| |                            /  __ \ (_)          | |   |  \/  |                                  
| |    _   _ _ __   __ _ _ __  | /  \/ |_  ___ _ __ | |_  | .  . | __ _ _ __   __ _  __ _  ___ _ __ 
| |   | | | | '_ \ / _` | '__| | |   | | |/ _ \ '_ \| __| | |\/| |/ _` | '_ \ / _` |/ _` |/ _ \ '__|
| |___| |_| | | | | (_| | |    | \__/\ | |  __/ | | | |_  | |  | | (_| | | | | (_| | (_| |  __/ |   
\_____/\__,_|_| |_|\__,_|_|     \____/_|_|\___|_| |_|\__| \_|  |_/\__,_|_| |_|\__,_|\__, |\___|_|   
                                                                                     __/ |          
                                                                                    |___/                                                                           
";
            Console.WriteLine(title, Color.Green);
            Console.WriteLine(new string('=', 100), Color.Green);
            Console.WriteLine();
            Console.Write($"This tool is made by {ownertag}", Color.Green);
            Console.WriteLine($" --- Version: {ver}", Color.Green);
            Console.WriteLine();

            ConsoleHelpers.PrintLine("", "[MAIN MENU]", Color.Green);
            ConsoleHelpers.PrintLine("OPTION", "1. Create Account", Color.Green);
            ConsoleHelpers.PrintLine("OPTION", "2. Remove Accounts", Color.Green);
            ConsoleHelpers.PrintLine("OPTION", "3. View Installed Accounts", Color.Green);
            ConsoleHelpers.PrintLine("OPTION", "4. Exit the program", Color.Green);
            Console.WriteLine();
        }

        public static void RemoveAccountsMenu()
        {
            ClearScreen();
            ConsoleHelpers.PrintLine("", "[REMOVE ACCOUNTS]", Color.Green);
            ConsoleHelpers.PrintLine("OPTION", "1. Remove All Accounts", Color.Green);
            ConsoleHelpers.PrintLine("OPTION", "2. Remove Cracked Accounts (accessToken is not a UUID)", Color.Green);
            ConsoleHelpers.PrintLine("OPTION", "3. Remove Premium Accounts (accessToken is a UUID)", Color.Green);
            ConsoleHelpers.PrintLine("OPTION", "4. Remove ONE Specific Account", Color.Green);
            Console.WriteLine();

            Console.Write("Please type your option (1-4) here: ", Color.Green);

            string choice = System.Console.ReadLine()?.Trim();
            switch (choice)
            {
                case "1":
                    AccountManager.RemoveAllAccounts();
                    break;
                case "2":
                    AccountManager.RemoveCrackedAccounts();
                    break;
                case "3":
                    AccountManager.RemovePremiumAccounts();
                    break;
                case "4":
                    AccountManager.RemoveSingleAccount();
                    break;
                default:
                    ConsoleHelpers.PrintLine("ERROR", "Invalid option. Returning to main menu.", Color.Green);
                    break;
            }
            AccountManager.SaveJson();
        }

        private static void CreateAccountPrompt()
        {
            Console.Write("[INPUT] ", Color.Green);
            Console.Write("Enter your username: ", Color.Green);
            string usernameAdd = System.Console.ReadLine()?.Trim();

            if (!Validate.IsValidMinecraftUsername(usernameAdd))
            {
                ConsoleHelpers.PrintLine("WARNING", "You may experience issues joining servers because of your username being invalid.", Color.Green);
            }

            while (true)
            {
                Console.Write("[INPUT] ", Color.Green);
                Console.Write("Enter a valid UUID: ", Color.Green);
                string customuuidAdd = System.Console.ReadLine()?.Trim();

                if (!Validate.IsValidUUID(customuuidAdd))
                {
                    ConsoleHelpers.PrintLine("WARNING", "The UUID you entered is invalid. Please ensure it follows the correct format.", Color.Green);
                    Console.Write("[?] ", Color.Green);
                    Console.Write("Would you like to try again? (y/n): ", Color.Green);
                    string retry = System.Console.ReadLine()?.Trim().ToLower();
                    if (retry == "n")
                    {
                        ConsoleHelpers.PrintLine("INFO", "Returning to main menu....", Color.Green);
                        return;
                    }
                }
                else
                {
                    AccountManager.CreateAccount(usernameAdd, customuuidAdd);
                    AccountManager.SaveJson();
                    break;
                }
            }
        }

        private static void ClearScreen()
        {
            System.Console.Clear();
        }
    }
}
