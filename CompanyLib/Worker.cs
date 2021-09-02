using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyLib
{
    public enum WorkerType { Employee, Sales, Manager }

    abstract public class Worker
    {
        protected decimal Bonus { get; set; }
        protected decimal MaxBonus { get; set; }
        protected decimal SubordinateBonus { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BeginDate { get; set; }
        public Worker Boss { get; set; }
        public decimal Salary { get; set; }
        protected Company Company { get; set; }
        List<Worker> SubordinateWorkers { get; set; } = new List<Worker>();

        public Worker()
        {
        }

        public Worker(int id, string name, DateTime beginDate, Company company)
        {
            Id = id;
            Name = name;
            BeginDate = beginDate;
            Company = company;
        }

        public List<Worker> GetSubordinateWorkers()
        {
            return SubordinateWorkers;
        }

        public override string ToString()
        {
            string str = "Id: " + Id + " " + Name + ": " + this.GetType().Name;
            if (Salary > 0)
                return str + " $" + Salary.ToString();
            return str;
        }

        public decimal CalculateSalaryForWorker(DateTime dt)
        {
            if (dt < BeginDate) return 0;
            //first we set the salary on this worker, then we return
            Salary = GetSalary(dt) + GetBonuseForSubordinate(dt);
            return Salary;
        }
          
        public decimal GetSalary(DateTime dt)
        {
            DateTime zeroTime = new DateTime(1, 1, 1);
            TimeSpan span = dt - BeginDate;
            int years = (zeroTime + span).Year - 1;
            decimal yearsBonus = years * Bonus > MaxBonus ? MaxBonus : years * Bonus;
            decimal salary = Company.BaseRate + Company.BaseRate * (yearsBonus / 100);
            return salary;
        }

        abstract public decimal GetBonuseForSubordinate(DateTime dt);
    }
}
