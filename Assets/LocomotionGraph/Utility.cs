using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace LocomotionGraph
{
    public class Utility : MonoBehaviour
    {
        public static List<List<bool>> GetBoolMap(string bitmapFilename)
        {
            IEnumerable<string> file = File.ReadLines(bitmapFilename);

            List<List<bool>> listBoolMap = new List<List<bool>>();
            foreach (string line in file)
            {

                List<bool> rowBoolMap = new List<bool>();
                foreach (char bit in line)
                {
                    if (bit == '0') rowBoolMap.Add(false);
                    else rowBoolMap.Add(true);
                }

                listBoolMap.Add(rowBoolMap);

            }

            return listBoolMap;
        }
        
    }

}

