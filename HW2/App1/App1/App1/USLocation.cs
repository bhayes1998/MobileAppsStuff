/*
 * Bryan Hayes 
 * CSE 570J
 * 2/27/2020
 * USLocations 
 * A class that provides information on US states.    
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

namespace USLocations
{


    public class USLocation
    {

        private Dictionary<string, List<string>> totalData = new Dictionary<string, List<string>>();

        public USLocation()
        {

        }

        // Handles loading in the data from the zipcodes.tsv file 
        public void loadData(string fileName)
        {

            // Normally I would use a simple StreamReader, but that was giving me problems, so this was the only way 
            // I could figure out how to load the data file in  
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(App1.MainPage)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("App1." + fileName);
            string text = "";
            var reader = new System.IO.StreamReader(stream);

            string splitter = "\t";
            reader.ReadLine();

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] temp = line.Split(splitter.ToCharArray());
                string taxReturns = temp[16];
                if (taxReturns == "")
                    continue;
                List<string> insert = new List<string>();

                // Selects only the data that we need, and gets rid of the rest 
                if (!totalData.ContainsKey(temp[1]))
                { 
                    string info = temp[3] + " " + temp[4] + " " + temp[16] + " " + temp[18];
                    insert.Add(info);
                    totalData.Add(temp[1], insert);
                }
            }
        }

        // Calculates every zipcode that has an average tax return within $100 of the given amount 
        public List<string> avgTaxWithin(double taxAmount)
        {
            List<string> taxesWithinRange = new List<string>();

            foreach (KeyValuePair<string, List<string>> pair in totalData)
            {
                long totalTaxes = 0;

                // Grabs the values that we need to calculate average tax returns 
                string[] info = pair.Value.ElementAt(0).Split(' ');
                string cityState = "";
                for (int i = 0; i < info.Length - 2; i++)
                    cityState += info[i] + " " ;
                string taxWages = info[info.Length-1];
                string taxReturns = info[info.Length - 2];
                
                // Checks to make sure that taxWages and taxReturns are not empty 
                if (taxWages != "" && taxReturns != "")
                    totalTaxes += (Convert.ToInt64(taxWages) / Convert.ToInt64(taxReturns));

                // Builds the string with the info we need to add to our search results 
                string infoToAdd = pair.Key + " " + cityState + "$" + totalTaxes;
                if (totalTaxes >= (taxAmount - 100) && totalTaxes <= (taxAmount + 100))
                {
                    if (pair.Key.Length == 4)
                        taxesWithinRange.Add("0" + infoToAdd);
                    else 
                        taxesWithinRange.Add(infoToAdd);
                }
            }
            taxesWithinRange.Sort();
            return taxesWithinRange;
        }

        // Grabs all the info of zipcodes in a given city state 
        public List<string> zipsInCityState(string city, string state)
        {
            List<string> zipList = new List<string>();

            foreach (KeyValuePair<string, List<string>> pair in totalData)
            {
                long totalTaxes = 0;

                string[] info = pair.Value.ElementAt(0).Split(' ');
                string cityToCheck = "";
                for (int i = 0; i < info.Length - 3; i++)
                    cityToCheck += info[i] + " ";
                string taxWages = info[info.Length - 1];
                string taxReturns = info[info.Length - 2];

                if (taxWages != "" && taxReturns != "")
                    totalTaxes += (Convert.ToInt64(taxWages) / Convert.ToInt64(taxReturns));
                string infoToAdd = pair.Key + " " + cityToCheck + info[info.Length-3] + " $" + totalTaxes;

                if (city.ToLower().Trim() == cityToCheck.ToLower().Trim() && state.ToLower().Trim() == info[info.Length - 3].ToLower().Trim())
                {
                    if (pair.Key.Length == 4)
                        zipList.Add("0" + infoToAdd);
                    else
                        zipList.Add(infoToAdd);
                }
            }
            zipList.Sort();
            return zipList;
            
        }
    }
}
