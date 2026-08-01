using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kiosk.MedicalServices.Models.Requests
{
    public class MedicalService
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = "";
        public string Prefix { get; set; } = "";
        
    }
}