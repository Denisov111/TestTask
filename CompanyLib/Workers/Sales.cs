using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyLib
{
    class Sales : Worker
    {
        public Sales(int id, string name, DateTime beginDate, Company company)
            : base(id, name, beginDate, company)
        {
            Bonus = Config.SalesBonus;
            MaxBonus = Config.MaxSalesBonus;
            SubordinateBonus = Config.SalesSubordinateBonus;
        }

        override public decimal GetBonuseForSubordinate(DateTime dt)
        {
            decimal bonusesBySub = 0;
            var subWorkers = TreeWalker.Walk<Worker>(this, w => w.GetSubordinateWorkers()).Where(w => w != this).ToList();
            foreach (var subWorker in subWorkers)
                bonusesBySub += subWorker.CalculateSalaryForWorker(dt) * (SubordinateBonus / 100);
            return bonusesBySub;
        }
    }
}
