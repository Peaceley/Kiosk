using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kiosk.Visits.Responses
{
    public class VisitResponse
    {
        public int VisitId { get; set; }
        public string VisitNo { get; set; } = "";
        public string PatientNo { get; set; } = "";
        public string PatientName { get; set; } = "";
        public string MedicalServiceName { get; set; } = "";
        public string MedicalServiceCode { get; set; } = "";
        public string Token { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime VisitDate { get; set; }
    }
}