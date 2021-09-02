using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace CompanyLib
{
    public class Company
    {
        /// <summary>
        /// Base rate for salsry calculation
        /// </summary>
        public decimal BaseRate { get; set; }
        /// <summary>
        /// List of all workers
        /// </summary>
        List<Worker> Workers { get; set; } = new List<Worker>();


        /// <summary>
        /// Adding an employee with data validation
        /// </summary>
        /// <param name="name"></param>
        /// <param name="beginDate"></param>
        /// <param name="workerType"></param>
        /// <returns></returns>
        public CallResult AddWorker(string name, DateTime beginDate, WorkerType workerType)
        {
            CallResult nameRslt = CheckName(name);
            if (!nameRslt.Success) return nameRslt;
            CallResult dateRslt = CheckDate(beginDate);
            if (!dateRslt.Success) return dateRslt;
            Workers.Add(CreateWorker(name, beginDate, workerType));
            return new CallResult();
        }

        /// <summary>
        /// Name validation based config
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private CallResult CheckName(string name)
        {
            CallResult cr = new CallResult();
            bool isOk = Regex.IsMatch(name, Config.RegexNamePattern);
            if (!isOk) cr.Error = new Error("enter correct name");
            return cr;
        }

        /// <summary>
        /// Date validation based config
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private CallResult CheckDate(DateTime date)
        {
            CallResult cr = new CallResult();
            if (date == null)
                cr = new CallResult() { Error = new Error("date can not be null") };
            if (date < Config.MinDate)
                cr = new CallResult() { Error = new Error("date must be more than " + Config.MinDate.ToString()) };
            return cr;
        }

        /// <summary>
        /// Create worker with unique id
        /// </summary>
        /// <param name="name"></param>
        /// <param name="beginDate"></param>
        /// <param name="workerType"></param>
        /// <returns></returns>
        private Worker CreateWorker(string name, DateTime beginDate, WorkerType workerType)
        {
            int? maxId = Workers.Max(w => (int?)w.Id);
            int id = maxId == null ? 1 : (int)++maxId;
            switch(workerType)
            {
                case WorkerType.Employee:
                    return new Employee(id, name, beginDate, this);
                case WorkerType.Manager:
                    return new Manager(id, name, beginDate, this);
                case WorkerType.Sales:
                    return new Sales(id, name, beginDate, this);
                default:
                    throw new Exception();
            }
        }

        /// <summary>
        /// Get list of workers
        /// </summary>
        /// <returns></returns>
        public List<Worker> GetWorkers()
        {
            return Workers;
        }

        /// <summary>
        /// Clear list of workers
        /// </summary>
        public void ClearWorkers()
        {
            Workers.Clear();
        }

        /// <summary>
        /// Establishing relationships between employees
        /// </summary>
        /// <param name="workerWithoutBoss"></param>
        /// <param name="boss"></param>
        public CallResult SetBoss(Worker workerWithoutBoss, Worker boss)
        {
            var subWorkers = TreeWalker.Walk<Worker>(workerWithoutBoss, w => w.GetSubordinateWorkers()).Where(w => w != workerWithoutBoss);
            if (subWorkers.Contains(boss))
                return new CallResult() { Error = new Error("can not set boss, the boss is a subordinate") };
            boss.GetSubordinateWorkers().Add(workerWithoutBoss);
            workerWithoutBoss.Boss = boss;
            return new CallResult();
        }

        /// <summary>
        /// Set salary for all workers and return summ
        /// </summary>
        /// <param name="baseRate"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public CallResult<decimal> CalculateSalary(int baseRate, DateTime dt)
        {
            bool isCorrectBeginDates = CheckCorrectBeginDates(dt);
            if (!isCorrectBeginDates)
                return new CallResult<decimal>() { Error = new Error("some of the employees have a work start date later than the payroll date, the tree structure is not correct for this date") };
            BaseRate = baseRate;
            var bosses = Workers.Where(w => w.Boss == null);
            foreach (Worker worker in bosses)
                worker.CalculateSalaryForWorker(dt);
            decimal summ = Workers.Sum(w => w.Salary);
            return new CallResult<decimal>() { Data = summ };
        }

        /// <summary>
        /// Checking the date for compliance with the fact that it is not less than any date of the employee's start of work,
        /// otherwise the calculation does not make sense, since the payroll structure will be violated
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private bool CheckCorrectBeginDates(DateTime dt)
        {
            var workers = Workers.Where(w => w.BeginDate > dt).FirstOrDefault();
            if (workers == null)
                return true;
            return false;
        }
    }
}
