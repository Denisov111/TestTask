using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyLib
{
    public class Employee : Worker
    {
        public Employee(int id, string name, DateTime beginDate, Company company)
            :base(id, name, beginDate, company)
        {
            Bonus = Config.EmployeeBonus;
            MaxBonus = Config.MaxEmployeeBonus;
        }

        override public decimal GetBonuseForSubordinate(DateTime dt) => 0;

        public new List<Worker> GetSubordinateWorkers() => new List<Worker>();
    }
}
