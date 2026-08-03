using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kiosk.Visits.Requests
{
    public class VisitsCreateFullRequest
    {
        public int VisitId { get; set; }
        public string VisitNo {get; set;} = string.Empty;
        public int PatientId {get; set;} 
        public string? PatientNo{get; set;}
        public int MedicalServiceId {get; set;}
        public string? MedicalServicesCode {get; set;}
        public DateTime VisitDate {get; set;}
        public DateTime CreatedAt {get; set;}
    }
}