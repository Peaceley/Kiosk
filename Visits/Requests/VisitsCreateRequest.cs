using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kiosk.Tokens.Requests
{
    public class VisitsCreateRequest
    {
        public string? PatientNo {get; set;}
        public string? MedicalServiceCode {get; set;}
    }
}