/*
 * Bryan Hayes 
 * CSE 570J
 * 2/13/2020
 * Zipcodes
 * A console project that interacts with the USLocations library.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using USLocation;

namespace Zipcodes
{
    class Zipcodes
    {
        static void Main(string[] args)
        {
            USLocations locs = new USLocations();
            Task locsTask = locs.loadData("zipcodes.tsv");
            string command;
            string[] arguments;
            
            while (true)
            {
                Console.Write("zipcodes>");
                command = Console.ReadLine();
                arguments = command.Split(null);
                locsTask.Wait();

                if (arguments[0] == "distance")
                {
                    double dist = locs.Distance(Convert.ToInt32(arguments[1]), Convert.ToInt32(arguments[2]));
                    Console.WriteLine("The distance between " + arguments[1] + " and " + arguments[2]
                        + " is " + Math.Round((dist * 3960), 2) + " miles (" + Math.Round((dist * 6371), 2) + " km)");
                }
                else if (arguments[0] == "lookup")
                {
                    List<string> ret = locs.Lookup(Convert.ToInt32(arguments[1]));
                    foreach (string s in ret)
                    {
                        Console.WriteLine(s);
                    }
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
