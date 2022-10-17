using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class Empleado
    {
        public string EmployeeID { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public long Abiertos { get; set; }
        public long Cerrados30D { get; set; } 
    }
}
