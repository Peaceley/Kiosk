using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kiosk.Visits.Models.Requests
{
    public class VisitCreateRequest
    {
        public int PatientId { get; set; }
        public int MedicalServiceId { get; set; } 
    }
}