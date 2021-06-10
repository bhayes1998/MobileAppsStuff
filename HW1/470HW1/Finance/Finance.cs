/*
 * Bryan Hayes 
 * CSE 570J
 * 2/13/2020
 * Finance
 * A console project that interacts with the USLocations library.
 */


using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using USLocation;

namespace Finance
{
    class Finance
    {
        static void Main(string[] args)
        {
            string filename = "zipcodes.tsv";
            USLocations locs = new USLocations();
            Task locsTask = locs.loadData(filename);
            string command;
            string[] arguments;

            while (true)
            {
                Console.Write("finance>");
                command = Console.ReadLine();
                arguments = command.Split(null);
                locsTask.Wait();

                if (arguments[0] == "taxes")
                {
                    double avgTaxAmount = locs.avgTax(arguments[1]);
                    Console.WriteLine("Average tax return: " + Math.Round(avgTaxAmount, 2));
                }
                else if (arguments[0] == "exit")
                {
                    Console.WriteLine("Goodbye!");
                    break;
                }
                else
                    Console.WriteLine("Invalid command");
            }

        }
    }
}
