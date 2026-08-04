using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Kiosk.MedicalServices.MedicalServiceHelper
{
    public class MedicalHelper
    {
        public static string GenerateServiceCode(string servicename)
        {

            if (string.IsNullOrWhiteSpace(servicename))
            {
                throw new ArgumentException("Medical Service name can't be empty");
            }
            //removing the the extra space

            servicename = servicename.Trim();

            //splitting the name into words
            // 
        
            string[] words = servicename.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            string servicecode;

            if (words.Length == 1)
            {
                // Example: Laboratory -> LAB
                servicecode = words[0].Length >= 3
                    ? words[0].Substring(0, 3)
                    : words[0];
            }
            else
            {
                // Example: General Consultation -> General -> GEN
                servicecode = words[0].Length >= 3
                    ? words[0].Substring(0, 3)
                    : words[0];
            }
            //return value
            return servicecode.ToUpper();
        }

        // internal static async Task<bool> MedicalServiceExists(string medicalServiceCode, IDbConnection connection)
        // {
        //     throw new NotImplementedException();
        // }
    }
}
