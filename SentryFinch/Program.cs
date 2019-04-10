using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinchAPI;
using HidSharp;

namespace SentryFinch
{
    class Program
    {
        //*******************************
        // App: Sentry Finch
        // Author: Lauren and Zack
        // Date: 4/8/19
        //*******************************


        static void Main(string[] args)
        {
            DisplayWelcomeScreen();
            DisplayMenu();
            DisplayClosingScreen();

        }

        static void DisplayWelcomeScreen()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\tWelcome to our Application");
            Console.WriteLine();

            DisplayContinuePrompt();
        }

        static void DisplayHeader(string headerText)
        {
            //
            // display header
            //
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t" + headerText);
            Console.WriteLine();
        }

        static void DisplayMenu()
        {
            bool exiting = false;
            string menuChoice;
            Finch steve = new Finch();
            steve.connect();
            double lowerTempThreshold = 0;
            double upperLightThreshold = 0;

            while (!exiting)
            {
                DisplayHeader("Main Menu");
                Console.WriteLine("1) Setup");
                Console.WriteLine("2) Activate Sentry Bot");
                Console.WriteLine("E) Exit");
                Console.WriteLine();
                Console.Write("Enter Menu Choice:");
                menuChoice = Console.ReadLine();

                switch (menuChoice)
                {
                    case "1":
                        lowerTempThreshold = SetupTemp(steve);
                        upperLightThreshold = SetupLight(steve);
                        break;
                    case "2":
                        ActivateSentryBot(lowerTempThreshold, upperLightThreshold, steve);
                        break;
                    case "E":
                    case "e":
                        exiting = true;
                        steve.disConnect();
                        break;
                    default:
                        Console.WriteLine("Please enter a proper menu choice.");
                        DisplayContinuePrompt();
                        break;
                }

            }
        }

        static double SetupTemp (Finch steve)
        {
            double temperatureDifference;
            double lowerTempThreshold;
            double ambientTemp;

            DisplayHeader("Setup Sentry Bot");

            Console.Write("Enter desired change in temperature: ");

            while (!double.TryParse(Console.ReadLine(), out temperatureDifference))
            {
                Console.WriteLine("Please enter a valid response.");
            }


            ambientTemp = steve.getTemperature();

            lowerTempThreshold = ambientTemp - temperatureDifference;


            DisplayContinuePrompt();

            return lowerTempThreshold;
        }

        static double SetupLight (Finch steve)
        {
            double lightDifference;
            double upperLightThreshold;
            double ambientLight;

            DisplayHeader("Setup Light level");

            Console.Write("Enter desired change in light: ");
            double.TryParse(Console.ReadLine(), out lightDifference);

            ambientLight = CalculateAmbientLight(steve);

            upperLightThreshold = ambientLight + lightDifference;


            DisplayContinuePrompt();

            return upperLightThreshold;
        }

        static double CalculateAmbientLight(Finch steve)
        {
            double ambientLight;
            int[] ambientLightArray = new int[1];

            ambientLightArray = steve.getLightSensors();
            ambientLight = ambientLightArray.Average();

            return ambientLight;
        }

        static void ActivateSentryBot(double lowerTempThresholdValue, double upperLightThreshold, Finch steve)
        {
            bool currentTemp = false;
            bool currentLight = false;
            
            DisplayHeader("Activate Sentry Bot");

            Console.Clear();
            Console.WriteLine("Monitor Temperature and Light");

            while (!TemperatureBelowThresholdValue(lowerTempThresholdValue, steve) && !LightAboveThresholdValue(upperLightThreshold, steve))
            {
                TemperatureNominalIndicator(steve); 

                currentTemp = TemperatureBelowThresholdValue(lowerTempThresholdValue, steve);
                currentLight = LightAboveThresholdValue(upperLightThreshold, steve);

            }
            steve.noteOn(750);
            steve.setLED(0, 0, 255);
            Console.WriteLine("Press any key to turn off alarm.");
            Console.ReadKey();
            steve.noteOff();
            steve.setLED(0, 0, 0);

            if (currentTemp == true)
            {
                Console.WriteLine();
                Console.WriteLine("Temperature has dropped below the threshold.");
            }
            else if (currentLight == true)
            {
                Console.WriteLine();
                Console.WriteLine("Light went above the threshold.");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("An error has occured.");
            }


            DisplayContinuePrompt();
        }

        static void TemperatureNominalIndicator(Finch steve)
        {
            steve.setLED(0, 255, 0);
            steve.wait(500);
            steve.setLED(0, 0, 0);
        }

        static bool LightAboveThresholdValue(double upperLightThreshold, Finch steve)
        {
            double ambientLight = CalculateAmbientLight(steve);

            if (ambientLight >= upperLightThreshold)
             {
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool TemperatureBelowThresholdValue(double lowerTempThresholdValue, Finch steve)
        {
            if (steve.getTemperature() <= lowerTempThresholdValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static void DisplayContinuePrompt()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }

        static void DisplayClosingScreen()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\tThank you for using our Application");
            Console.WriteLine();

            DisplayContinuePrompt();
        }
    }
}
