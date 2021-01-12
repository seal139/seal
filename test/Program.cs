
using seal.Attributes;
using seal.Base;
using seal.Helper;
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
            [Column("Leader")] public Employee Leader { get; set; }
            [Column("EmpStatus")] public Stts Status { get; set; }
            [Column("Level")] public int? Level { get; set; }

            //public override void Pack(IList<object> values)
            //{
            //    int index = 0;
            //    Code = values[index++].ToString();
            //    Name = values[index++].ToString();
            //    NickName = values[index++].ToString();

            //    Employee e = new Employee();
            //    e.Id = Convert.ToInt32(values[index++]);
            //    Leader = e;
            //    Status = (Stts)Convert.ToInt32(values[index++]);
            //    Level = Convert.ToInt32(values[index++]);
            //    Id = Convert.ToInt32(values[index++]);
            //    Created = DateTime.Now;
            //    LastModified = DateTime.Now;
            //}

            //public override IList<object> Unpack()
            //{
            //    List<object> ret = new List<object>();
            //    ret.Add(Code);

            //    ret.Add(Name);
            //    ret.Add(NickName);
            //    ret.Add(Leader.UniqueIdentifierValue);
            //    ret.Add(Status);
            //    ret.Add(Level);
            //    ret.Add(Id);
            //    ret.Add("");
            //    ret.Add("");

            //    return ret;
            //}
        }

        public enum Stts
        { 
            Contract, Permanent        
        }

        static void Main(string[] args)
        {
            ExpressionBuilder eb = "MyColum";
            eb.Equal("val1").And(ExpressionBuilder.Group(new ExpressionBuilder("Column2")
                .GreaterThan("29").And("Column3").SmallerThan("20")).Or("foo").NotEqual("bar"));
            Console.WriteLine((string)eb);

            Console.ReadLine();


            int cnt = 10000000;
            Stopwatch stopWatch = new Stopwatch();


            //Dictionary<string, string> D = new Dictionary<string, string>();
            //for(int i= 0; i < 10000000; i++)
            //{
            //    D.Add("Dict " + i, "val " + i);
            //}
           
            //stopWatch.Start();
            //var test = D["Dict " + 675000];
            //stopWatch.Stop();

            //Console.WriteLine(test);
            //Console.WriteLine(stopWatch.ElapsedMilliseconds + " ms");
            //Console.ReadLine();


            ISerialization serializer = Serializer.Instance;
            IApi api = Seal.GetInstance();
            api.Serializer = serializer;

            api.Init<Employee>();

            Employee jbo = new Employee();
            jbo.Id = 210801;
            jbo.LastModified = DateTime.Now;
            jbo.Created = DateTime.Now;
            jbo.Name = "Andika";
            jbo.NickName = null;
            jbo.Code = "aap";
            jbo.Leader = null;



            IList<object> des = new List<object>();
            des.Add("Code");
            des.Add("Name");
            des.Add("NickName");
            des.Add("210801");
            des.Add(Stts.Permanent);
            des.Add(1);
            des.Add(123);
            des.Add(DateTime.Now);
            des.Add(DateTime.Now);




            stopWatch = new Stopwatch();
            stopWatch.Start();
            IList<Employee> empList = new List<Employee>();
            for (int i = 0; i < cnt; i++)
            {
                Employee objt = serializer.Deserialize<Employee>(des);
                objt.SetJoinedObjValue("Leader", jbo);
                empList.Add(objt);
            }
            stopWatch.Stop();
            Console.WriteLine("Deserialize " + cnt);
            Console.WriteLine("Total time: " + stopWatch.ElapsedMilliseconds + "ms");
            Console.WriteLine("Average time: " + (double)stopWatch.ElapsedMilliseconds / (double)cnt + "ms");

            Console.WriteLine("============");



            IList<IList<object>> listAll = new List<IList<object>>();


            stopWatch = new Stopwatch();
            stopWatch.Start();
            Employee obj = new Employee();
            obj.Id = 1;
            obj.LastModified = DateTime.Now;
            obj.Created = DateTime.Now;
            obj.Name = "Septian";
            obj.NickName = null;
            obj.Code = "Seal";
            obj.Leader = jbo;
            obj.Status = 0;
            for (int i = 0; i < cnt; i++)
            {

                IList<object> raw = serializer.Serialize<Employee>(obj);
                listAll.Add(raw);
            }
            stopWatch.Stop();
            Console.WriteLine("Serialize " + cnt);
            Console.WriteLine("Total time: " + stopWatch.ElapsedMilliseconds + "ms");
            Console.WriteLine("Average time: " + (double)stopWatch.ElapsedMilliseconds / (double)cnt + "ms");
            Console.ReadLine();

            //ModelFactory q = ModelFactory.GetInstance();
            //TableInfo ti = q["Employee"];

            //Console.WriteLine("ob");


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



