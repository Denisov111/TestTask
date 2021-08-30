using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyLib
{
    public enum WorkerType { Employee, Sales, Manager }

    public class Worker
    {
        //It's public for testing
        //All must be private since properties must be set after checking in the appropriate methods
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BeginDate { get; set; }
        public Worker Boss { get; set; }
        public WorkerType WorkerType { get; set; }
        public decimal Salary { get; set; }
        Company Company { get; set; }
        List<Worker> SubordinateWorkers { get; set; } = new List<Worker>();

        public Worker()
        {
        }

        public Worker(int id, string name, DateTime beginDate, WorkerType workerType, Company company)
        {
            Id = id;
            Name = name;
            BeginDate = beginDate;
            WorkerType = workerType;
            Company = company;
        }

        public List<Worker> GetSubordinateWorkers()
        {
            return SubordinateWorkers;
        }

        public override string ToString()
        {
            string str = "Id: " + Id + " " + Name + ": " + WorkerType.ToString();
            if (Salary > 0)
                return str + " $" + Salary.ToString();
            return str;
        }

        public decimal CalculateSalaryForWorker(DateTime dt)
        {
            if (dt < BeginDate) return 0;
            //first we set the salary on this worker, then we return
            Salary = GetSalary(dt) + GetBonuses(dt);
            return Salary;
        }

        /// <summary>
        /// This not beautiful, this functionality should be located in separate classes that inherit from an abstract class, for example:
        /// Employee : Worker
        /// Manager : Worker
        /// Sales : Worker
        /// {
        ///    GetBonuses(DateTime dt);
        ///    GetSalary(DateTime dt);
        /// }
        /// But for testing purposes I had to do this
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public decimal GetSalary(DateTime dt)
        {
            switch (WorkerType)
            {
                case WorkerType.Employee:
                    return GetSalary(dt, Config.EmployeeBonus, Config.MaxEmployeeBonus);
                case WorkerType.Manager:
                    return GetSalary(dt, Config.ManagerBonus, Config.MaxManagerBonus);
                case WorkerType.Sales:
                    return GetSalary(dt, Config.SalesBonus, Config.MaxSalesBonus);
                default:
                    throw new Exception("unknow type of worker");
            }
        }

        private decimal GetBonuses(DateTime dt)
        {
            switch (WorkerType)
            {
                case WorkerType.Employee:
                    return 0;
                case WorkerType.Manager:
                    return GetManagerBonuses(dt);
                case WorkerType.Sales:
                    return GetSalesBonuses(dt);
                default:
                    throw new Exception("unknow type of worker");
            }
        }

        private decimal GetSalesBonuses(DateTime dt)
        {
            decimal bonusesBySub = 0;
            var subWorkers = TreeWalker.Walk<Worker>(this, w => w.GetSubordinateWorkers()).Where(w => w != this).ToList();
            foreach (var subWorker in subWorkers)
                bonusesBySub += subWorker.CalculateSalaryForWorker(dt) * (Config.ManagerSubordinateBonus / 100);
            return bonusesBySub;
        }

        private decimal GetManagerBonuses(DateTime dt)
        {
            decimal bonusesBySub = 0;
            foreach (var subWorker in SubordinateWorkers)
                bonusesBySub += subWorker.CalculateSalaryForWorker(dt) * (Config.ManagerSubordinateBonus / 100);
            return bonusesBySub;
        }

        private decimal GetSalary(DateTime dt, decimal bonus, decimal maxBonus)
        {
            DateTime zeroTime = new DateTime(1, 1, 1);
            TimeSpan span = dt - BeginDate;
            int years = (zeroTime + span).Year - 1;
            decimal yearsBonus = years * bonus > maxBonus ? maxBonus : years * bonus;
            decimal salary = Company.BaseRate + Company.BaseRate * (yearsBonus / 100);
            return salary;
        }
    }
}
