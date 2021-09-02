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

        public List<Worker> GetSubordinateWorkers() => SubordinateWorkers;

        public override string ToString()
        {
            string str = "Id: " + Id + " " + Name + ": " + this.GetType().Name;
            if (Salary > 0) str += " $" + Salary.ToString();
            return str;
        }

        public decimal CalculateSalaryForWorker(DateTime dt)
        {
            if (dt < BeginDate) throw new Exception("the worker's start date of work is later than the payroll date");
            return Salary = GetSalary(dt) + GetBonuseForSubordinate(dt);
        }

        public decimal GetSalary(DateTime dt)
        {
            DateTime zeroTime = new DateTime(1, 1, 1);
            TimeSpan span = dt - BeginDate;
            int years = (zeroTime + span).Year - 1;
            decimal yearsBonus = years * Bonus > MaxBonus ? MaxBonus : years * Bonus;
            return Company.BaseRate + Company.BaseRate * (yearsBonus / 100); ;
        }

        abstract public decimal GetBonuseForSubordinate(DateTime dt);
    }
}
