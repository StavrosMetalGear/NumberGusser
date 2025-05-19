using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace KinoGame
{
    class Program
    {
        static List<string> gameHistory = new List<string>();
        static int jackpot = 1000;

        static void Main(string[] args)
        {
            // Display Game Information
            GetAppInfo();
            GreetUser();

            // Load History from File
            LoadGameHistory();

            while (true)
            {
                int wallet = 100;

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("1. Play Kino");
                Console.WriteLine("2. View Game History");
                Console.WriteLine("3. View Jackpot");
                Console.WriteLine("4. Clear Game History");
                Console.WriteLine("5. Exit");
                Console.ResetColor();

                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    PlayKino(ref wallet);
                }
                else if (choice == "2")
                {
                    DisplayGameHistory();
                }
                else if (choice == "3")
                {
                    DisplayJackpot();
                }
                else if (choice == "4")
                {
                    ClearGameHistory();
                }
                else if (choice == "5")
                {
                    Console.WriteLine("Thank you for playing! Goodbye!");
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice. Please select 1, 2, 3, 4, or 5.");
                    Console.ResetColor();
                }
            }

            SaveGameHistory();
        }

        static void PlayKino(ref int wallet)
        {
            Console.WriteLine("Enter your bet amount (Current wallet: {0}): ", wallet);
            if (!int.TryParse(Console.ReadLine(), out int bet) || bet <= 0 || bet > wallet)
            {
                Console.WriteLine("Invalid bet amount.");
                return;
            }

            List<int> userPicks = GetUserPicks();
            List<int> kinoNumbers = GenerateKinoNumbers();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The winning numbers are:");
            kinoNumbers.ForEach(num => Console.Write(num + " "));
            Console.WriteLine();
            Console.ResetColor();

            int matches = userPicks.Intersect(kinoNumbers).Count();
            int winnings = 0;

            if (matches == 0)
            {
                jackpot += bet;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"No matches. Your bet of {bet} credits has been added to the jackpot.");
                Console.ResetColor();
            }
            else if (matches == 1)
            {
                winnings = bet / 2;
                wallet += winnings;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"You matched 1 number! You win back half your bet: {winnings} credits.");
                Console.ResetColor();
            }
            else if (matches == 2)
            {
                winnings = bet;
                wallet += winnings;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"You matched 2 numbers! You win back your full bet: {winnings} credits.");
                Console.ResetColor();
            }
            else
            {
                winnings = CalculateWinnings(matches, bet);
                wallet += winnings;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"You matched {matches} numbers!");
                Console.WriteLine($"You won {winnings} credits! Your current wallet: {wallet}");
                Console.ResetColor();
            }

            gameHistory.Add($"Matched: {matches} | Won: {winnings} | Wallet: {wallet}");
        }

        static void DisplayGameHistory()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Game History:");
            if (gameHistory.Count == 0)
            {
                Console.WriteLine("No games played yet.");
            }
            else
            {
                foreach (var entry in gameHistory)
                {
                    Console.WriteLine(entry);
                }
            }
            Console.ResetColor();
        }

        static void DisplayJackpot()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Current Jackpot: {jackpot}");
            Console.ResetColor();
        }

        static void ClearGameHistory()
        {
            gameHistory.Clear();
            File.Delete("GameHistory.txt");
            Console.WriteLine("Game history cleared.");
        }

        static void SaveGameHistory()
        {
            File.WriteAllLines("GameHistory.txt", gameHistory);
        }

        static void LoadGameHistory()
        {
            if (File.Exists("GameHistory.txt"))
            {
                gameHistory.AddRange(File.ReadAllLines("GameHistory.txt"));
            }
        }

        static void GetAppInfo()
        {
            string appName = "Kino Game";
            string appVersion = "1.2.3";
            string appAuthor = "Stavros Zymvragoudakis";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0}: Version {1} by {2}", appName, appVersion, appAuthor);
            Console.ResetColor();
        }

        static void GreetUser()
        {
            Console.WriteLine("What is your name?");
            string inputName = Console.ReadLine();
            Console.WriteLine("Hello {0}, let's play Kino!", inputName);
        }

        static List<int> GetUserPicks()
        {
            List<int> userPicks = new List<int>();
            Console.WriteLine("Enter 6 unique numbers between 1 and 49:");
            while (userPicks.Count < 6)
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out int number) && number >= 1 && number <= 49 && !userPicks.Contains(number))
                {
                    userPicks.Add(number);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid input or duplicate number. Try again:");
                    Console.ResetColor();
                }
            }
            return userPicks;
        }

        static List<int> GenerateKinoNumbers()
        {
            Random random = new Random();
            return Enumerable.Range(1, 49).OrderBy(x => random.Next()).Take(20).ToList();
        }

        static int CalculateWinnings(int matches, int bet)
        {
            switch (matches)
            {
                case 3: return bet * 2;
                case 4: return bet * 5;
                case 5: return bet * 10;
                case 6: return bet * 20 + jackpot;
                default: return 0;
            }
        }
    }
}
