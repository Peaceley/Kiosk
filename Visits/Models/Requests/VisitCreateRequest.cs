using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kiosk.Visits.Models.Requests
{
    public class VisitCreateRequest
    {
        
        public string? PatientNo{get; set;}
        public string? MedicalCode{get; set;}
        public DateTime VisitDate {get; set;}
        

    }
}