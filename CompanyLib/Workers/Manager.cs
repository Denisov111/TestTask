using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyLib
{
    class Manager : Worker
    {
        public Manager(int id, string name, DateTime beginDate, Company company)
            : base(id, name, beginDate, company)
        {
            Bonus = Config.ManagerBonus;
            MaxBonus = Config.MaxManagerBonus;
            SubordinateBonus = Config.ManagerSubordinateBonus;
        }

        override public decimal GetBonuseForSubordinate(DateTime dt)
        {
            decimal bonusesBySub = 0;
            foreach (var subWorker in GetSubordinateWorkers())
                bonusesBySub += subWorker.CalculateSalaryForWorker(dt) * (SubordinateBonus / 100);
            return bonusesBySub;
        }
    }
}
