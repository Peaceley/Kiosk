using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kiosk.MedicalServices.Models
{
    public class MedicalService
    {    

        public int MedicalServiceId { get; set; }
        public string MedicalServiceName { get; set; } = "";
        public string MedicalServiceCode { get; set; } = "";

        
    }
}