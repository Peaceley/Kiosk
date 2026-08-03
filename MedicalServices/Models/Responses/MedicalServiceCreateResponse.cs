using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kiosk.MedicalServices.Models.Responses
{
    public class MedicalServiceCreateResponse
    {
        public int MedicalId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}