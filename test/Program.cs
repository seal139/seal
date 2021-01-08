
using seal.Attributes;
using seal.Base;
using seal.Interface;
using seal.IntfImpl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace test
{
    class Program
    {
        [Entity("Employee")]
        public class Employee : ModelTable
        { 

            [Column("Code")] public String Code { get; set; }
            [Column("Name")] public String Name { get; set; }
            [Column("NickName")] public String NickName { get; set; }
            [Column("Leader")] public Employee? Leader { get; set; }
            [Column("EmpStatus")] public Stts Status { get; set; }
            [Column("Level")] public int? Level { get; set; }
        }

        public enum Stts
        { 
            Contract, Permanent        
        }

        static void Main(string[] args)
        {
            ISerialization serializer = Serializer.Instance;
            IApi api = Seal.GetInstance();
            api.Serializer = serializer;

            api.Init<Employee>();

          

            IDictionary<string, object> des = new Dictionary<string, object>();
            des.Add("Leader", 2);
           
            des.Add("Code", "foo");
            des.Add("Name", "bar");
            des.Add("NickName", null);
            des.Add("Status", 0);
            des.Add("Level", null);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            IList<Employee> empList = new List<Employee>();
            for (int i = 0; i < 3000000; i++)
            {
                empList.Add(serializer.Deserialize<Employee>(des));
            }
            stopWatch.Stop();
            Console.WriteLine("Deserialize " + 3000000);
            Console.WriteLine("Total time: " + stopWatch.ElapsedMilliseconds + "ms");
            Console.WriteLine("Average time: " + (double)stopWatch.ElapsedMilliseconds / (double)3000000 + "ms");
            Console.ReadLine();


            Employee jbo = new Employee();
            jbo.Id = 1000;
            jbo.LastModified = DateTime.Now;
            jbo.Created = DateTime.Now;
            jbo.Name = "Andika";
            jbo.NickName = null;
            jbo.Code = "aap";
           jbo.Leader = null;


            IList<IDictionary<string, object>> listAll = new List<IDictionary<string, object>>();

          

            Employee obj = new Employee();
            obj.Id = 1;
            obj.LastModified = DateTime.Now;
            obj.Created = DateTime.Now;
            obj.Name = "Septian";
            obj.NickName = null;
            obj.Code = "Seal";
            obj.Leader = jbo;
            obj.Status = 0;
            for (int i = 0; i < 3000000; i++)
            {
               
                IDictionary<string, object> raw = serializer.Serialize<Employee>(obj);
                listAll.Add(raw);
            }
            stopWatch.Stop();
            Console.WriteLine("Serialize " + 3000000);
            Console.WriteLine("Total time: " + stopWatch.ElapsedMilliseconds + "ms");
            Console.WriteLine("Average time: " + (double)stopWatch.ElapsedMilliseconds / (double)int.MaxValue + "ms");
            Console.ReadLine();



            Console.WriteLine("ob");


            //Employee.InitMapping<Employee>();

            //Timer t = new Timer();
            //t.Start();
            //EmployeeDao empDao = new EmployeeDao();
            //BusinessEntityDao beDao = new BusinessEntityDao();

            //seal.Transactor.setConnection(seal.Transactor.ConnectionStringBuilder("SEPTIAN-WST\\SQLSERVER", "AdventureWorks2019", "septianPr", "septian13"));


            //List<Employee> empList = empDao.QueryBuilderSelect();
            //int i = 0;
            //foreach(Employee emp in empList)
            //{
            //    Console.Write(++i + " | ");
            //    Console.Write(emp.KTP + " | ");
            //    Console.Write(emp.LoginID + " | ");
            //    Console.Write(emp.JobTitle + " | ");
            //    Console.Write(emp.OrgLevel.RowGuID + " | ");
            //    Console.WriteLine(emp.LastModified );
            //}

            //t.Stop();
            //Console.WriteLine("\n\r\n\r\n\r Done ->" + i + " Data... Elapsed time: " + t.Interval / 1000 + " seconds");
            //Console.ReadLine();


        }
    }

    class test
    {
        public string qq { set; get; }
    }
}



