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

namespace USLocation
{


    public class USLocations
    {
        private List<string[]> totalData = new List<string[]>();
        private Dictionary<string, Tuple<double, double>> latLong = new Dictionary<string, Tuple<double, double>>();
        // This constructor will initiate the loading of the TSV file.
        // The constructor must return very quickly, perhaps before all 
        // of the zip code information has been completely loaded. Tasks
        // will be needed to do this.
        public USLocations()
        {
            
        }

        public async Task loadData(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            string splitter = "\t";
            await reader.ReadLineAsync();
            while (!reader.EndOfStream)
            {
                string line = await reader.ReadLineAsync();
                string[] temp = line.Split(splitter.ToCharArray());
                string[] insert = new string[9];
                insert[0] = temp[1]; insert[1] = temp[3]; insert[2] = temp[4];
                insert[3] = temp[6]; insert[4] = temp[7]; insert[5] = temp[13];
                insert[6] = temp[16]; insert[7] = temp[17]; insert[8] = temp[18];
                totalData.Add(insert);

                if (!latLong.ContainsKey(insert[0]) && insert[3] != "" && insert[4] != "")
                {
                    Tuple<double, double> temp1 = new Tuple<double, double>(Convert.ToDouble(insert[3]), Convert.ToDouble(insert[4]));
                    latLong.Add(insert[0], temp1);
                }
            }

        }


        // This method will return the number of miles between two zip codes.
        // Since zipcodes can appear multiple times, the location is based
        // on the first record that has a matching zipcode.
        // This method will need to wait, if it is called before the zipcodes
        // have been completely loaded.
        // 
        // Look up "Haversine" formula to do this one.
        public double Distance(int zip1, int zip2)
        {
            Tuple<double, double> pos1;
            latLong.TryGetValue(Convert.ToString(zip1), out pos1);
            Tuple<double, double> pos2;
            latLong.TryGetValue(Convert.ToString(zip2), out pos2);
            if (pos2 == null || pos1 == null)
                return Int32.MaxValue;
            var dLat = toRadians(pos2.Item1 - pos1.Item1);
            var dLon = toRadians(pos2.Item2 - pos1.Item2);
            double lat1 = toRadians(pos1.Item1);
            double lat2 = toRadians(pos2.Item1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            return 2 * Math.Asin(Math.Sqrt(a));
        }

        // Converts a given angle into radians 
        public double toRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        // This method will return all the common names for a particular
        // zip code. The sequence of items in the list will match the order
        // seen in the data file.
        // This method will need to wait, if it is called before the zipcodes
        // have been completely loaded.
        public List<string> Lookup(int zip)
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
        public double avgTax(string state)
        {
            double totalWages = 0;
            double totalReturns = 0;
            for (int i = 0; i < totalData.Count; i++)
            {
                if (totalData[i][2] == state)
                {
                    if (totalData[i][8] != "")
                        totalWages += Convert.ToInt64(totalData[i][8]);

                    if (totalData[i][6] != "")
                        totalReturns += Convert.ToInt64(totalData[i][6]);
                }
            }
            
            return totalWages / totalReturns;
        }




         /**
         * Returns a map that is keyed to state name. The values in the map are the
	     * set of city names that reside in that particular state. The map looks
	     * like: "AL" --> { "MONTGOMERY", "MOBILE", ... } "AK" --> { "ANCHORAGE",
         * "BARROW", ...} ...
         */

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
        }

        /**
        * Returns the city names that appear in both of the given states.
	    * "OH" and "MI" would yield {OXFORD, FRANKLIN, ... }
        */

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
                if (state2List.Contains(state1List[i])) { 
                    both.Add(state1List[i]);
                }
            }
            return both;
        }

        /**
        * Returns all zipcodes that are within a specified distance from a
        * particular zipcode.
        */

        public ISet<int> GetZipCodesCloseTo(int zipCode, double miles)
        {
            ISet<int> zips = new HashSet<int>();
            for (int i = 0; i < totalData.Count; i++)
            {
                if (Distance(zipCode, Convert.ToInt32(totalData[i][0])) * 3960 <= miles)
                    zips.Add(Convert.ToInt32(totalData[i][0]));
            }

            return zips;
        }

        /**
        * Ranked list of states, where the ranking is ascending order of number of
        * zipcodes. In the event of a tie, use the state's alphabetical ordering.
        */

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
        }
    }
}
