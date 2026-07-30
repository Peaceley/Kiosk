using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kiosk.Visits.Models.Responses
{
    public class VisitResponse
    {
        public int VisitNo{get;set;}
        public string? PatientNo{get; set;}
        public string? MedicalCode{get; set;}
        public DateTime VisitDate {get; set;}
        public DateTime CreatedAt {get; set;}
    }
}