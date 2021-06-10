/*
 * Bryan Hayes 
 * CSE 570J
 * 2/13/2020
 * USLocations 
 * A collection of programs that provide information on US states.  
 * 
 * Everything I have tested thus far works.  I have tested all of the cases on the homework document
 * as well as several others, and everything is working very well.  
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace USLocations
{


    public class USLocation
    {
        private List<string[]> totalData = new List<string[]>();
        private List<string[]> stateTaxInfo = new List<string[]>();
        // private Dictionary<string, Tuple<double, double>> zipReturnWage = new Dictionary<string, Tuple<double, double>>();
        // This constructor will initiate the loading of the TSV file.
        // The constructor must return very quickly, perhaps before all 
        // of the zip code information has been completely loaded. Tasks
        // will be needed to do this.
        public USLocation()
        {

        }

        public void loadData(string fileName)
        {
            // StreamReader reader = new StreamReader(fileName);

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
                string[] insert = new string[5];
                insert[0] = temp[1]; insert[1] = temp[3]; insert[2] = temp[4];
                insert[3] = temp[16]; insert[4] = temp[18];
                totalData.Add(insert);
             /*   if (!zipReturnWage.ContainsKey(insert[0]))
                {
                    Tuple<double, double> tempTuple = new Tuple<double, double>(Convert.ToDouble(insert[3]), Convert.ToDouble(insert[4]));
                    zipReturnWage.Add(insert[0], tempTuple);
                }
                else
                {
                    Tuple<double, double> tempTuple;
                    zipReturnWage.TryGetValue(insert[0], out tempTuple);
                    double taxReturn = Convert.ToDouble(insert[3]) + tempTuple.Item1;
                    double taxWages = Convert.ToDouble(insert[4]) + tempTuple.Item2;
                    zipReturnWage[insert[0]] = new Tuple<double, double>(taxReturn, taxWages);
                }*/
            }
            double totalWages = 0;
            double totalReturns = 0;
            SortedSet<string> zipList = new SortedSet<string>();
            List<string> stateInfoList = new List<string>();
            for (int j = 0; j < totalData.Count; j++)
            {
                string zip = totalData[j][0];
                if (zipList.Contains(zip))
                    continue;
                for (int i = 0; i < totalData.Count; i++)
                {
                    if (totalData[i][0] == zip)
                    {
                        if (totalData[i][4] != "")
                            totalWages += Convert.ToInt64(totalData[i][4]);

                        if (totalData[i][3] != "")
                            totalReturns += Convert.ToInt64(totalData[i][3]);
                    }
                }

                double totalTaxes = totalWages / totalReturns;
                string[] stateInfo = new string[4];
                stateInfo[0] = "" + zip;
                stateInfo[1] = totalData[j][1]; 
                stateInfo[2] = totalData[j][2];
                stateInfo[3] = "" + totalTaxes; 
                stateTaxInfo.Add(stateInfo);
                totalWages = 0;
                totalReturns = 0;
                zipList.Add(zip);
            }
        }


        // This method will return all the common names for a particular
        // zip code. The sequence of items in the list will match the order
        // seen in the data file.
        // This method will need to wait, if it is called before the zipcodes
        // have been completely loaded.
        public List<string> LookupZipInfo(int zip)
        {
            string zipString = "" + zip;
            List<string> ret = new List<string>();
            for (int i = 0; i < totalData.Count; i++)
            {
                if (totalData[i][0] == zipString)
                {
                    ret.Add(totalData[i][1] + ", " + totalData[i][2]);
                }
            }
            return ret;
        }


        // Calculates the average tax return for a given state 
        public List<string> avgTaxWithin(double taxAmount)
        {
            List<string> closeStates = new List<string>();
            int ree = stateTaxInfo.Count;
            for (int i = 0; i < stateTaxInfo.Count; i++)
            {
                double totalTaxes = Convert.ToDouble(stateTaxInfo[i][3]);
                if (totalTaxes >= taxAmount - 100 && totalTaxes <= taxAmount + 100)
                    closeStates.Add(stateTaxInfo[i][0] + " " + stateTaxInfo[i][1] + " " + stateTaxInfo[i][2] + " " + stateTaxInfo[i][3]);
            }

            Console.WriteLine(closeStates.Count);
            return closeStates;
            /*double totalWages = 0;
            double totalReturns = 0;
            SortedSet<string> zipList = new SortedSet<string>();
            List<string> stateInfoList = new List<string>();
            for (int j = 0; j < totalData.Count; j++) {
                string zip = totalData[j][0];
                if (zipList.Contains(zip))
                    continue;
                for (int i = 0; i < totalData.Count; i++)
                {
                    if (totalData[i][0] == zip)
                    {
                        if (totalData[i][4] != "")
                            totalWages += Convert.ToInt64(totalData[i][4]);

                        if (totalData[i][3] != "")
                            totalReturns += Convert.ToInt64(totalData[i][3]);
                    }
                }
                double totalTaxes = totalWages / totalReturns;
                if ((totalTaxes >= taxAmount - 100) && (totalTaxes <= taxAmount + 100))
                {
                    string stateInfo = zip + " " + totalData[j][2] + " " + totalData[j][3] + " " + totalTaxes;
                    stateInfoList.Add(stateInfo);
                }
                zipList.Add(zip);
            }*/
            
        }




        /**
        * Returns a map that is keyed to state name. The values in the map are the
        * set of city names that reside in that particular state. The map looks
        * like: "AL" --> { "MONTGOMERY", "MOBILE", ... } "AK" --> { "ANCHORAGE",
        * "BARROW", ...} ...
        *//*

        public IDictionary<string, ISet<string>> GetCityNames()
        {
            IDictionary<string, ISet<string>> names = new Dictionary<string, ISet<string>>();

            for (int i = 0; i < totalData.Count; i++)
            {
                if (names.ContainsKey(totalData[i][2]) == false)
                {
                    ISet<string> temp = new SortedSet<string>();
                    temp.Add(totalData[i][1]);
                    names.Add(totalData[i][2], temp);
                }
                else
                {
                    ISet<string> temp = new SortedSet<string>();
                    names.TryGetValue(totalData[i][2], out temp);
                    temp.Add(totalData[i][1]);
                    names[totalData[i][2]] = temp;
                }
            }
            return names;
        }*/

        /**
        * Returns the city names that appear in both of the given states.
	    * "OH" and "MI" would yield {OXFORD, FRANKLIN, ... }
        */
/*
        public ISet<string> GetCommonCityNames(string state1, string state2)
        {
            List<string> state1List = new List<string>();
            List<string> state2List = new List<string>();
            ISet<string> both = new SortedSet<string>();

            for (int i = 0; i < totalData.Count; i++)
            {
                if (totalData[i][2].Equals(state1))
                    state1List.Add(totalData[i][1]);
                if (totalData[i][2].Equals(state2))
                    state2List.Add(totalData[i][1]);
            }

            for (int i = 0; i < state1List.Count; i++)
            {
                if (state2List.Contains(state1List[i]))
                {
                    both.Add(state1List[i]);
                }
            }
            return both;
        }
*/
        /**
        * Returns all zipcodes that are within a specified distance from a
        * particular zipcode.
        */

      /*  public ISet<int> GetZipCodesCloseTo(int zipCode, double miles)
        {
            ISet<int> zips = new HashSet<int>();
            for (int i = 0; i < totalData.Count; i++)
            {
                if (Distance(zipCode, Convert.ToInt32(totalData[i][0])) * 3960 <= miles)
                    zips.Add(Convert.ToInt32(totalData[i][0]));
            }

            return zips;
        }*/

        /**
        * Ranked list of states, where the ranking is ascending order of number of
        * zipcodes. In the event of a tie, use the state's alphabetical ordering.
        */
/*
        public List<string> MostZipCodes()
        {
            Dictionary<string, ISet<string>> names = new Dictionary<string, ISet<string>>();

            for (int i = 1; i < totalData.Count; i++)
            {
                if (names.ContainsKey(totalData[i][2]) == false)
                {
                    ISet<string> temp = new SortedSet<string>();
                    temp.Add(totalData[i][0]);
                    names.Add(totalData[i][2], temp);
                }
                else
                {
                    ISet<string> temp = new SortedSet<string>();
                    names.TryGetValue(totalData[i][2], out temp);
                    temp.Add(totalData[i][0]);
                    names[totalData[i][2]] = temp;
                }
            }
            List<Tuple<string, int>> zipCodes = new List<Tuple<string, int>>();
            foreach (KeyValuePair<string, ISet<string>> pair in names)
            {
                zipCodes.Add(new Tuple<string, int>(pair.Key, pair.Value.Count));
            }
            zipCodes = zipCodes.OrderBy(t => t.Item2).ToList();
            List<string> ret = new List<string>();
            for (int i = 0; i < zipCodes.Count; i++)
            {
                ret.Add(zipCodes[i].Item1);
            }
            Console.WriteLine(ret[1]);
            return ret;
        }*/
    }
}
