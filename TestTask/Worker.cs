using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    abstract class Worker
    {
        public string Name { get; set; }
        public DateTime BeginDate { get; set; }
        public Worker? Boss { get; set; }

        public Worker(string name, DateTime date)
        {
            Name = name;
            BeginDate = date;
        }
    }
}
