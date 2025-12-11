using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;

namespace LunarAccountTool.Helpers
{
    internal class AccountManager
    {
        private static JObject json;

        // ------------------------------------------
        // CONFIRMATION PROMPT
        // ------------------------------------------
        private static bool ConfirmAction(string message)
        {
            ConsoleHelpers.PrintLine("WARNING", message + " (y/n): ", Color.Yellow);

            Console.Write("[?] ", Color.Yellow);
            string input = System.Console.ReadLine()?.Trim().ToLower();

            return input == "y";
        }

        // ------------------------------------------
        // CREATE ACCOUNT
        // ------------------------------------------
        public static void CreateAccount(string username, string uuid)
        {
            JObject newAccount = new JObject
            {
                ["accessToken"] = uuid,
                ["accessTokenExpiresAt"] = "2050-07-02T10:56:30.717167800Z",
                ["eligibleForMigration"] = false,
                ["hasMultipleProfiles"] = false,
                ["legacy"] = true,
                ["persistent"] = true,
                ["userProperites"] = new JArray(),
                ["localId"] = uuid,
                ["minecraftProfile"] = new JObject
                {
                    ["id"] = uuid,
                    ["name"] = username
                },
                ["remoteId"] = uuid,
                ["type"] = "Xbox",
                ["username"] = username
            };

            JObject accounts = (JObject)json["accounts"];
            accounts[uuid] = newAccount;

            ConsoleHelpers.PrintLine("SUCCESS", "Your account has successfully been created.", Color.FromArgb(144, 238, 144));
        }

        // ------------------------------------------
        // REMOVE ALL ACCOUNTS
        // ------------------------------------------
        public static void RemoveAllAccounts()
        {
            if (!ConfirmAction("Are you sure you want to remove ALL accounts?"))
            {
                ConsoleHelpers.PrintLine("INFO", "Operation cancelled.", Color.Green);
                return;
            }

            json["accounts"] = new JObject();
            ConsoleHelpers.PrintLine("SUCCESS", "All accounts have been successfully removed.", Color.FromArgb(144, 238, 144));
        }

        // ------------------------------------------
        // REMOVE CRACKED ACCOUNTS
        // ------------------------------------------
        public static void RemoveCrackedAccounts()
        {
            if (!ConfirmAction("Are you sure you want to remove ALL CRACKED accounts?"))
            {
                ConsoleHelpers.PrintLine("INFO", "Operation cancelled.", Color.Green);
                return;
            }

            JArray accountsToRemove = new JArray();
            JObject accounts = (JObject)json["accounts"];

            foreach (var account in accounts)
            {
                if (Validate.IsValidUUID((string)account.Value["accessToken"]))
                {
                    accountsToRemove.Add(account.Key);
                }
            }

            foreach (var key in accountsToRemove)
            {
                accounts.Remove(key.ToString());
            }

            ConsoleHelpers.PrintLine("SUCCESS", "Cracked accounts have been successfully removed.", Color.FromArgb(144, 238, 144));
        }

        // ------------------------------------------
        // REMOVE PREMIUM ACCOUNTS
        // ------------------------------------------
        public static void RemovePremiumAccounts()
        {
            if (!ConfirmAction("Are you sure you want to remove ALL PREMIUM accounts?"))
            {
                ConsoleHelpers.PrintLine("INFO", "Operation cancelled.", Color.Green);
                return;
            }

            JArray accountsToRemove = new JArray();
            JObject accounts = (JObject)json["accounts"];

            foreach (var account in accounts)
            {
                if (!Validate.IsValidUUID((string)account.Value["accessToken"]))
                {
                    accountsToRemove.Add(account.Key);
                }
            }

            foreach (var key in accountsToRemove)
            {
                accounts.Remove(key.ToString());
            }

            ConsoleHelpers.PrintLine("SUCCESS", "Premium accounts have been successfully removed.", Color.FromArgb(144, 238, 144));
        }

        // ------------------------------------------
        // REMOVE A SINGLE ACCOUNT
        // ------------------------------------------
        public static void RemoveSingleAccount()
        {
            JObject accounts = (JObject)json["accounts"];

            if (accounts.Count == 0)
            {
                ConsoleHelpers.PrintLine("INFO", "No accounts available to remove.", Color.Green);
                return;
            }

            ConsoleHelpers.PrintLine("", "[REMOVE ONE ACCOUNT]", Color.Green);

            var accountKeys = new List<string>();
            int index = 1;

            foreach (var acc in accounts)
            {
                string username = (string)acc.Value["username"];
                Console.WriteLine($"{index}. {username} | UUID: {acc.Key}", Color.Green);

                accountKeys.Add(acc.Key);
                index++;
            }

            Console.WriteLine();
            Console.Write("Enter the number of the account to remove: ", Color.Green);

            string input = System.Console.ReadLine()?.Trim();
            if (!int.TryParse(input, out int selected) || selected < 1 || selected > accountKeys.Count)
            {
                ConsoleHelpers.PrintLine("ERROR", "Invalid selection.", Color.Green);
                return;
            }

            string uuidToRemove = accountKeys[selected - 1];
            string usernameToRemove = (string)accounts[uuidToRemove]["username"];

            if (!ConfirmAction($"Are you sure you want to remove account '{usernameToRemove}'?"))
            {
                ConsoleHelpers.PrintLine("INFO", "Operation cancelled.", Color.Green);
                return;
            }

            accounts.Remove(uuidToRemove);
            ConsoleHelpers.PrintLine("SUCCESS", "Account removed successfully!", Color.FromArgb(144, 238, 144));
        }

        // ------------------------------------------
        // DISPLAY INSTALLED ACCOUNTS
        // ------------------------------------------
        public static void ViewInstalledAccounts()
        {
            ConsoleHelpers.PrintLine("", "[Installed Accounts]", Color.Magenta);
            JObject accounts = (JObject)json["accounts"];

            foreach (var account in accounts)
            {
                ConsoleHelpers.PrintLine("ACCOUNT", account.Key + ": " + account.Value["username"], Color.FromArgb(144, 238, 144));
            }
        }

        // ------------------------------------------
        // LOAD JSON
        // ------------------------------------------
        public static void LoadJson()
        {
            try
            {
                if (File.Exists(Program.lunarAcccountsPath))
                {
                    json = JObject.Parse(File.ReadAllText(Program.lunarAcccountsPath));
                }
                else
                {
                    json = new JObject { ["accounts"] = new JObject() };
                }
            }
            catch (Exception e)
            {
                ConsoleHelpers.PrintLine("ERROR", "Failed to load accounts file: " + e.Message, Color.Red);
                ConsoleHelpers.PrintLine("NOTICE", "Please check that you have Lunar Client installed.", Color.Cyan);
                ConsoleHelpers.PrintLine("NOTICE", "Exiting in 3 seconds...", Color.Cyan);
                Thread.Sleep(3000);
                Environment.Exit(1);
            }
        }

        // ------------------------------------------
        // SAVE JSON
        // ------------------------------------------
        public static void SaveJson()
        {
            try
            {
                File.WriteAllText(Program.lunarAcccountsPath, json.ToString());
            }
            catch (Exception e)
            {
                ConsoleHelpers.PrintLine("ERROR", "Failed to save accounts file: " + e.Message, Color.Red);
            }
        }

        // ------------------------------------------
        // SHOW ACCOUNTS BEFORE REMOVAL
        // ------------------------------------------
        public static void ShowAccountsBeforeRemoval()
        {
            JObject accounts = (JObject)json["accounts"];

            if (accounts.Count == 0)
            {
                ConsoleHelpers.PrintLine("INFO", "No accounts installed.", Color.Green);
                return;
            }

            ConsoleHelpers.PrintLine("", "Installed Accounts:", Color.Magenta);

            int index = 1;
            foreach (var acc in accounts)
            {
                string username = (string)acc.Value["username"];
                Console.WriteLine($"{index}. {username} | UUID: {acc.Key}", Color.Green);
                index++;
            }
        }
    }
}
