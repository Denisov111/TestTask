using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask.Workers
{
    class Sales : Worker
    {
        public List<Worker> SubordinateWorker { get; set; } = new List<Worker>();

        public Sales(string name, DateTime date) : base(name, date)
        {

        }
    }
}
