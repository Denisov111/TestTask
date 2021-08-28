using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    class Company
    {
        static public decimal BaseRate { get; set; }

        static public decimal EmployeeBonus = 3;
        static public decimal MaxEmployeeBonus = 30;

        static public decimal ManagerBonus = 5;
        static public decimal MaxManagerBonus = 40;
        static public decimal ManagerSubordinateBonus = .5m;

        static public decimal SalesBonus = 1;
        static public decimal MaxSalesBonus = 35;
        static public decimal SalesSubordinateBonus = .3m;

        public List<Worker> Workers { get; set; } = new List<Worker>();
        
    }
}
