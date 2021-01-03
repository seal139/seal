
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
        void qwee()
        {

        }

        static void Main(string[] args)
        {
            test q = new test();
            q.qq = "sep";

            ParameterExpression arg = Expression.Parameter(typeof(test), "x");
            Expression expr = Expression.Property(arg,"qq");
            

            var propertyResolver = Expression.Lambda<Func<test, object>>(expr, arg).Compile();
            Expression.Assign(expr, 
            propertyResolver(q)
            Console.WriteLine(propertyResolver(q));
            Console.ReadLine();

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



