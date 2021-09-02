using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using CompanyLib;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.IO;
using MahApps.Metro.Controls;

namespace TestTask
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public Company Company { get; set; }
        public static Random rnd = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        public MainWindow()
        {
            InitializeComponent();
            Company = new Company();
            workersDataGrid.ItemsSource = Company.GetWorkers();
        }

        #region interface

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e) => AddWorker();
        
        private void Button_Click_1(object sender, RoutedEventArgs e) => CreateTestData();

        private void Button_Click_2(object sender, RoutedEventArgs e) => CalculateSalary();

        private void Button_Click_3(object sender, RoutedEventArgs e) => CalcOneWorker();

        private void CreateWorkersTree()
        {
            workersTreeView.Items.Clear();
            Worker boss = Company.GetWorkers().Where(w => w.Boss == null).First();
            workersTreeView.Items.Add(CreateItem(boss));
        }

        private TreeViewItem CreateItem(Worker worker)
        {
            TreeViewItem item = new TreeViewItem() { Header = worker.ToString(), IsExpanded = true };
            foreach (Worker w in worker.GetSubordinateWorkers())
                item.Items.Add(CreateItem(w));
            return item;
        }

        #endregion interface

        #region logic

        /// <summary>
        /// create random test data
        /// </summary>
        private void CreateTestData()
        {
            Company.ClearWorkers();

            string[] lines = File.ReadAllLines("Names.txt");
            List<string> names = new List<string>();
            foreach (string s in lines)
            {
                string[] ss = s.Split(';');
                names.Add(ss[1]);
            }

            for (int i = 0; i < 100; i++)
            {
                string name = names[rnd.Next(0, names.Count - 1)];
                DateTime dt = new DateTime(rnd.Next(1980, 2015), rnd.Next(1, 13), rnd.Next(1, 29));
                WorkerType wt = (WorkerType)rnd.Next(0, 3);
                CallResult cr = Company.AddWorker(name, dt, wt);
                if (!cr.Success) MessageBox.Show(cr.Error.Message);
            }

            while (true)
            {
                var res = Company.GetWorkers().Where(w => w.Boss == null);
                if (res.Count() < 2) break;
                Worker workerWithoutBoss = res.ToList()[rnd.Next(0, res.Count())];
                var potentialBosses = Company.GetWorkers().Where(w => w.GetType().Name != typeof(Employee).Name && w != workerWithoutBoss).ToList();
                var boss = potentialBosses[rnd.Next(0, potentialBosses.Count)];
                var subWorkers = TreeWalker.Walk<Worker>(workerWithoutBoss, w => w.GetSubordinateWorkers()).Where(w => w != workerWithoutBoss);
                if (!subWorkers.Contains(boss))
                {
                    Company.SetBoss(workerWithoutBoss, boss);
                    continue;
                }
            }

            workersDataGrid.Items.Refresh();
            CreateWorkersTree();
        }

        /// <summary>
        /// Call to Company.AddWorker for create one worker for test
        /// </summary>
        private void AddWorker()
        {
            if (BeginDateDatePicker.SelectedDate == null)
            {
                MessageBox.Show("selected date can not be null");
                return;
            }
            CallResult cr = Company.AddWorker(InputNameText.Text, (DateTime)BeginDateDatePicker.SelectedDate, (WorkerType)WorkerTypeComboBox.SelectedIndex);
            if (!cr.Success) MessageBox.Show(cr.Error.Message);
            workersDataGrid.Items.Refresh();
        }

        /// <summary>
        /// Calculate salary for all company workers and output summ
        /// </summary>
        private void CalculateSalary()
        {
            int baseRate = Int32.Parse(baseRateTextBox.Text.Replace(" ", ""));
            DateTime? dt = salaryDateDatePicker.SelectedDate;
            if (dt == null)
            {
                MessageBox.Show("select salary date");
                return;
            }
            CallResult<decimal> cr = Company.CalculateSalary(baseRate, (DateTime)dt);
            if (!cr.Success)
                MessageBox.Show(cr.Error.Message);
            else
                MessageBox.Show(cr.Data.ToString());
            workersDataGrid.Items.Refresh();
            CreateWorkersTree();
        }

        /// <summary>
        /// calculatiom salary for 0ne worker
        /// </summary>
        private void CalcOneWorker()
        {
            int id = Int32.Parse(workerIdTextBox.Text.Replace(" ", ""));
            var worker = Company.GetWorkers().Where(w => w.Id == id).FirstOrDefault();
            if (worker == null)
            {
                MessageBox.Show("unknow worker");
                return;
            }
            DateTime? dt = salaryDateDatePicker.SelectedDate;
            if (dt == null)
            {
                MessageBox.Show("select salary date");
                return;
            }
            Company.BaseRate = Int32.Parse(baseRateTextBox.Text.Replace(" ", ""));
            worker.CalculateSalaryForWorker((DateTime)dt);
            MessageBox.Show(worker.Salary.ToString());
        }

        #endregion logic
    }
}
