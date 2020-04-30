using System;
//=============================================================================================================================================
// To give credit where credit is due and to be clear that I copied the base code from a readily available solution for the sake of efficiency.
// This code was copied from: https://bestsoftwareqa.wordpress.com/2018/03/07/mastermind-game-code-example-c/  (It makes no sense to reinvent the wheel)
//
//                                   Mastermind Game Code Example | C# 
//                                   =================================
//
//       Then, the code was heavily modified and improved to suit your needs and my preferences.
//       Original Code:
//              had little or no validation
//              did not match requirements
//              was not as configurable as my modifications made it
//              contained static values for some arrays

//        -- I hope you enjoy the results --  :)
//
//**********************
// Development Process
//**********************
// Analysis: 2 of the requirements were not precise: (I attempted to get clarification on these issues prior to my development process.)
//
//      1. "with each digit between the numbers 1 and 6" - This is a poor requirement.
//		   - Inclusive?  
//
//      Decision: I assumed 1 and 6 were not included and the code reflects that assumption in my max and min variables.
//      ---------
//
//      2. "Nothing should be printed for incorrect digits." - This is a very poor requirement.
//         - "Printed" is a poor choice as the word should be "displayed"
//            A null can not be printed nor can a space, however, a space can be displayed
//         - "Nothing" implies null. This is a poor requirement.
//           The hint string would be unclear to the user thereby causing unnecessary frustration and overall loss of enjoyment.
//
//      Decision: I decided a space was more appropriate, the code reflects that decision.)
//      ---------
//
// Modification Time: ~3 hours (I was not familiar with the game previous to this.)
// Testing Time:      ~1 hour (I had to transfer the code to my dev machine as I did all of my initial editing with Notepad++)
//=============================================================================================================================================

namespace MasterMind
{
    class Program
    {
        #region  Variables

        // low number limit for random number generator
        private const int Min = 2;

        // high number limit for random number generator
        private const int Max = 6;

        // user can play again
        private static bool _playAgain;

        // switch for invalid characters
        private static bool _notValidNumber;

        // numbers outside of range
        private static string _errors = "";

        // length of number string (array sizing)
        private const int CodeSize = 4;

        // number of allowed attempts to crack the code
        private const int AllowedAttempts = 10;

        // number of tried guesses (accumulator)
        private static int _numTry;

        // user guess attempt array
        private static char[] _guess = new char[CodeSize];

        // random number solution (array)
        private static char[] _solution = new char[CodeSize]; // = new char[] { '3', '2', '5', '4' };

        // game board (2 dim array)
        private static readonly string[][] Board = new string[AllowedAttempts][];

        #endregion  Variables

        public static void Main()
        {
            _playAgain = true;
            while (_playAgain)
            {
                _notValidNumber = false;
                _numTry = 0;
                Console.Clear();
                bool shouldGameContinue = true;

                _solution = GenerateRandomCode(Min, Max);
                CreateBoard();

                while (shouldGameContinue)
                {
                    _errors = "";
                    Console.Clear();
                    DrawBoard();
                    Prompt(0);

                    _guess = Console.ReadLine().ToCharArray();

                    if (_guess.Length > 0) // avoid [enter] Only
                    {
                        if (_guess[0] == 'q' || _guess[0] == 'Q') // quit at any turn
                            Environment.Exit(0);

                        string number = string.Join("", _guess);
                        int x = 0;
                        if (!int.TryParse(number, out x)) // not number
                            Prompt(2);
                        else if (_guess.Length > CodeSize) // too long
                            Prompt(3);
                        else if (_guess.Length < CodeSize) // too short
                            Prompt(4);

                        if (!_notValidNumber) // we have a number, are any integers in it outside of our range?
                        {
                            CheckRange(_guess);

                            if (_errors.Length > 0) // outside range
                                Prompt(5);
                            else // finally, a good number for guess...
                                shouldGameContinue = !CheckSolution(_guess) && !HasUserRunOutOfAttempts();
                        }
                    }
                    else
                        Prompt(1); // did not enter any characters, [Enter] key only...

                    _notValidNumber = false;
                }
            }

            Console.ReadLine();
        }

        #region Private Methods - In order of apperance

        private static char[] GenerateRandomCode(int min, int max)
        {
            Random rnd = new Random();

            for (var i = 0; i < CodeSize; i++)
            {
                var chr = rnd.Next(min, max).ToString();
                _solution[i] = chr[0];
            }

            return _solution;
        }

        private static void CreateBoard()
        {
            for (var i = 0; i < AllowedAttempts; i++)
            {
                Board[i] = new string[CodeSize + 1];
                for (var j = 0; j < CodeSize + 1; j++)
                    Board[i][j] = " ";
            }
        }

        private static void DrawBoard()
        {
            for (var i = 0; i < Board.Length; i++) 
                Console.WriteLine("|" + String.Join("|", Board[i]));
        }

        private static void Prompt(int promptSwitch)
        {
            switch (promptSwitch)
            {
                case 1:
                    Console.Clear();
                    Console.WriteLine("You MUST enter valid characters, ONLY [2,3,4,5] are allowed.");
                    Console.WriteLine("Press [Enter] key to continue...");
                    Console.ReadLine();
                    Console.Clear();
                    break;
                case 2:
                    _notValidNumber = true;
                    Console.Clear();
                    Console.WriteLine("You entered invalid characters, ONLY [2,3,4,5] are allowed.");
                    Console.WriteLine("Press [Enter] key to continue...");
                    Console.ReadLine();
                    Console.Clear();
                    break;
                case 3:
                    _notValidNumber = true;
                    Console.Clear();
                    Console.WriteLine("You entered too many characters");
                    Console.WriteLine("Press [Enter] key to continue...");
                    Console.ReadLine();
                    Console.Clear();
                    break;
                case 4:
                    _notValidNumber = true;
                    Console.Clear();
                    Console.WriteLine("You entered too few characters");
                    Console.WriteLine("Press [Enter] key to continue...");
                    Console.ReadLine();
                    Console.Clear();
                    break;
                case 5:
                    Console.Clear();
                    Console.WriteLine("You entered numbers in positions " + _errors +" outside the allowed range.");
                    Console.WriteLine("Press [Enter] key to continue...");
                    Console.ReadLine();
                    Console.Clear();
                    break;
                default:
                    Console.WriteLine("Please Pick 4 Numbers in any order from [2,3,4,5]...");
                    Console.WriteLine(" ---- Enter Q to Quit ----");
                    Console.WriteLine("Enter Guess:");
                    break;
            }
        }

        private static void CheckRange(char[] guess)
        {
            for (int i = 0; i < CodeSize; i++)
                if (int.Parse(guess[i].ToString()) <= Min - 1 || int.Parse(guess[i].ToString()) >= Max)
                    _errors = _errors + (i + 1) + ",";

            if (_errors.Length > 0)
            {
                _errors = _errors.TrimEnd(_errors[_errors.Length - 1]);
                _errors = '[' + _errors + ']';
            }
        }

        private static bool CheckSolution(char[] guess)
        {
            // 1 - Detect a correct solution

            string guessString = string.Join("", guess);
            string solutionString = string.Join("", _solution);

            if (guessString == solutionString)
            {
                Console.WriteLine("Congrats, you guessed the secret code!");
                Console.WriteLine("======================================");
                Console.WriteLine("Press [Enter] key to continue...");
                Console.ReadLine();
                
                bool badChoice = true;
                while (badChoice)
                {
                    Console.Clear();
                    Console.WriteLine("Play MasterMind Again? (Y/N)");
                    char[] resp = Console.ReadLine().ToCharArray();
                    if (resp[0] == 'n' || resp[0] == 'N')
                        Environment.Exit(0);
                    else if (resp[0] == 'y' || resp[0] == 'Y')
                    {
                        badChoice = false;
                        _playAgain = true;
                    }
                }

                return true;
            }

            InsertCode(guess);
            return false;
        }

        private static string GenerateHint(char[] guess)
        {
            // 2 - Generate a hint

            // Clone the solution
            char[] solutionCloned = (char[])_solution.Clone();

            char[] hints = new char[CodeSize];

            for(int i = 0; i < CodeSize;i++)
                 hints[i] =  ' ';

            // Determine correct "number-locations" 
            for (int i = 0; i < solutionCloned.Length; i++)
                if (solutionCloned[i] == guess[i])
                    hints[i] = '+'; //By doing this we don't count again the same number found in the next for loop

            // Determine correct "numbers" 
            string clonedSolutionString = string.Join("", solutionCloned);
            for (int i = 0; i < solutionCloned.Length; i++)
                if (clonedSolutionString.Contains(guess[i])) // number is correct
                    if (hints[i] != '+') // number is not in wrong position
                        hints[i] = '-';
            string rslt = string.Join(",", hints);
    
            // return hint string
            return "[" + rslt + "]";
        }

        private static void InsertCode(char[] guess)
        {
            // 3 - Add guess and hint to the board

            for (int i = 0; i < guess.Length; i++) 
                Board[_numTry][i] = guess[i].ToString();

            Board[_numTry][CodeSize] = GenerateHint(guess).ToString();

            _numTry++;
        }

        private static bool HasUserRunOutOfAttempts()
        {
            if (_numTry < AllowedAttempts)
                return false;

            Console.WriteLine($"You ran out of turns! The solution was: {string.Join("", _solution)}");
            Console.WriteLine("*****************************************************************************************************");
            Console.WriteLine("Press [Enter] key to continue...");
            Console.ReadLine();
            bool badChoice = true;
            while (badChoice)
            {
                Console.Clear();
                Console.WriteLine("Play MasterMind Again? (Y/N)");
                char[] resp = Console.ReadLine().ToCharArray();
                if (resp[0] == 'n' || resp[0] == 'N')
                    Environment.Exit(0);
                else if (resp[0] == 'y' || resp[0] == 'Y')
                {
                    badChoice = false;
                    _playAgain = true;
                }

            }

            return _playAgain;
        }

        #endregion Methods
    }
}

