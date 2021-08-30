using NUnit.Framework;
using CompanyLib;
using System;

namespace Tests
{
    class CompanyTests
    {
        Company company;

        [SetUp]
        public void Setup()
        {
            company = new Company();
        }

        [Test]
        public void AddWorkerBadNameTest()
        {
            string name = "+++---";
            DateTime dt = Config.MinDate.AddDays(1);
            CallResult cr = company.AddWorker(name, dt, WorkerType.Employee);
            Assert.IsTrue(!cr.Success);
        }

        [Test]
        public void AddWorkerBadDateTest()
        {
            string name = "John";
            DateTime dt = Config.MinDate.AddDays(-1);
            CallResult cr = company.AddWorker(name, dt, WorkerType.Employee);
            Assert.IsTrue(!cr.Success);
        }

        [Test]
        public void AddWorkerTest()
        {
            string name = "John";
            DateTime dt = Config.MinDate.AddDays(1);
            CallResult cr = company.AddWorker(name, dt, WorkerType.Employee);
            Assert.IsTrue(cr.Success);
        }

    }
}
