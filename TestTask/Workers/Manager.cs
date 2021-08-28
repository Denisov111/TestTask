using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask.Workers
{
    class Manager : Worker
    {
        public List<Worker> SubordinateWorker { get; set; } = new List<Worker>();

        public Manager(string name, DateTime date) : base(name, date)
        {

        }
    }
}
