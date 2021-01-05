
using seal.Attributes;
using seal.Base;
using seal.Interface;
using seal.IntfImpl;
using System;
using System.Collections.Generic;
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
            [Column("Code")] public string Code { get; set; }
            [Column("Name")] public string Name { get; set; }
            [Column("NickName")] public string NickName { get; set; }
        }


        static void Main(string[] args)
        {
            ISerialization serializer = Serializer.Instance;
            IApi api = Seal.GetInstance();

            api.Init<Employee>();

            api.Serializer = serializer;

            Employee obj = new Employee();
            obj.Id = 1;
            obj.LastModified = DateTime.Now;
            obj.Created = DateTime.Now;
            obj.Name = "Septian";
            obj.NickName = "Ian";
            obj.Code = "Seal";



            IDictionary<string, object> raw =  serializer.Serialize<Employee>(obj);
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



