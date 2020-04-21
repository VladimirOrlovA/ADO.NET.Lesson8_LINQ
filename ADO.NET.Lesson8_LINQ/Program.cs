using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ADO.NET.Lesson8_LINQ
{
    class Program
    {
        static List<Area> areas;
        static List<Timer> timers;
        static void Main(string[] args)
        {
            //Example1();
            //Example2();
            //Example3();
            //Example4();
            //Example5();
            //Example6();

            Pract();

            Console.WriteLine("Complete");

            Console.ReadKey();
        }

        static void Show(IEnumerable<string> n)
        {

            foreach (var item in n)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine(new string('=', 70));
        }

        static void Example1()
        {
            string[] name = new string[] { "Evgeniy", "Mark", "Noi" };
            IEnumerable<string> vs = name.OrderBy(o => o.Split().Last());
            Show(name);
        }

        static void Example2()
        {
            string[] name = new string[] { "Evgeniy", "Mark", "Noi" };

            IEnumerable<string> q2_0 = name.OrderBy(o => o.Split().Last());

            Show(q2_0);

            var q2_1 = name
                .Where(w => w.Length == name.OrderBy(o2 => o2.Length)
                                            .Select(s2 => s2.Length)
                                            .First());

            Show(q2_1);

            var q2_2 = from n in name
                       where n.Length ==
                       (from n2 in name
                        orderby n2.Length
                        select n2.Length).First()
                       select n;

            Show(q2_2);

            var q2_3 = from n in name
                       where n.Length == name.OrderBy(o => o.Length).First().Length
                       select n;

            Show(q2_3);
        }


        // ключевое слово INTO
        static void Example3()
        {
            string[] name = new string[] { "Evgeniy", "Mark", "Noi" };

            var query = from n in name
                        select Regex.Replace(n, "[v,r]", "");

            Show(query);

            var query1 = from n in name
                         select Regex.Replace(n, "[v,r]", "")
                         // сюда в перем noVr результат строчек 57 58
                         into noVr

                         where noVr.Length > 2
                         orderby noVr
                         select noVr;

            Show(query1);
        }

        static void Example4()
        {
            string[] users = new string[] { "Evgeniy", "Mark", "Noi", "Jack", "Ive" };

            IEnumerable<User> clients = users.Select(s => new User { Name = s });

            List<User> users1 = new List<User>();
            foreach (string item in users)
            {
                User u = new User();
                u.Name = item;
                users1.Add(u);
            }

            // var1
            var result = users.Select(s => new { name = s, length = s.Length });
            // var2
            IEnumerable<Object> result1 = users.Select(s => new { name = s, length = s.Length });
        }

        // к примеру Example4()
        class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        // ключевое слово Let
        static void Example5()
        {
            string[] users = new string[] { "Evgeniy", "Mark", "Noi", "Jack", "Ive" };

            var result = from u in users

                             // сюда будет помещен только один элемент из 122 - from u in users
                         let vow = Regex.Replace(u, "[v, r]", "")

                         where vow.Length > 5
                         orderby vow
                         select u;
        }

        static void Example6()
        {
            List<UserExp6> users6 = new List<UserExp6>
                {
                    new UserExp6 {Name="Том", Age=23, Languages = new List<string> {"английский", "немецкий" }},
                    new UserExp6 {Name="Боб", Age=27, Languages = new List<string> {"английский", "французский" }},
                    new UserExp6 {Name="Джон", Age=29, Languages = new List<string> {"английский", "испанский" }},
                    new UserExp6 {Name="Элис", Age=24, Languages = new List<string> {"испанский", "немецкий" }}
                };

            var selectedUsers = users6.SelectMany(u => u.Languages,
                            (u, l) => new { User = u, Lang = l })
                            .Where(u => u.Lang == "английский" && u.User.Age < 28)
                            .Select(u => u.User);

            foreach (UserExp6 user in selectedUsers)
                Console.WriteLine($"{user.Name} - {user.Age}");
        }

        class UserExp6
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public List<string> Languages { get; set; }
            public UserExp6()
            {
                Languages = new List<string>();
            }
        }

        /// <summary>
        ///  Home Work
        /// </summary>
        static void Pract()
        {
            string connStr = ConfigurationManager.ConnectionStrings["SqlClient"].ConnectionString;
            SqlConnection connection = new SqlConnection(connStr);
            string cmd = "SELECT * FROM Area";
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd, connection);
            DataSet dataSet = new DataSet();

            dataAdapter.Fill(dataSet, "Area");

            string cmd1 = "SELECT * FROM Timer";
            SqlDataAdapter dataAdapter1 = new SqlDataAdapter(cmd1, connection);

            dataAdapter1.Fill(dataSet, "Timer");

            var collArea = dataSet.Tables["Area"].Rows.Cast<DataRow>();
            var collTimer = dataSet.Tables["Timer"].Rows.Cast<DataRow>();


            areas = collArea.Select(s => new Area
            {
                AreaId = (int)s["AreaId"],
                TypeArea = (int)s["TypeArea"],
                IP = s["IP"].ToString(),
                WorkingPeople = (int)s["WorkingPeople"]
            })
                .ToList();

            timers = collTimer.Select(s => new Timer
            {
                TimerId = (int)s["TimerId"],
                UserId = (int)s["UserId"],
                AreaId = (int)s["AreaId"],
                DocumentId = (int)s["DocumentId"],
                DateStart = Convert.ToDateTime(s["DateStart"]),
                DateFinish = Convert.ToDateTime(s["DateFinish"] != DBNull.Value? s["DateFinish"] : null),
                DurationInSeconds = (int)s["DurationInSeconds"]
            })
                .ToList();


            /*a.	Отобразить только те зоны/участки из таблицы
             * Area, у которых количество рабочих(WorkingPeople) больше 2.*/

            var result = areas.Where(w => w.WorkingPeople >= 2);

            /*b.	Используя таблицу Area, отобразить только 
             * те зоны/участки которые относятся к зоне разборке (AssemblyArea = 1)*/

            var result1 = areas.Where(w => w.AssemblyArea == 1);

            /*c.	Для таблицы Area, отобразить только первые 10 зое/участков*/

            var result2 = areas.Take(10);

            /*d.	Для таблицы Area, отобразить зоны/участки от 5 по 8*/

            var result3 = areas.Skip(5).Take(3);

            /*e.	Для таблицы Area, отобразить зоны/участки, пока не 
             * будет найдена зона/участок с не нулевым порядком выполнения
             * (OrderExecution)*/

            var result4 = areas.TakeWhile(tw => tw.OrderExecution != 0);
            // после 0 пропустит

            /*f.	Для таблицы Area, отобразить зоны/участки, пока не 
             * будет найдена зона/участок с нулевым порядком выполнения
             * (OrderExecution)*/

            var result5 = areas.SkipWhile(sw => sw.OrderExecution == 0);
            // до 0 пропустит

            /*g.Для таблицы Area, исключить зоны/ участки с повторяющимися IP*/

            var resultIP = areas.Select(s => s.IP).Distinct();
            var result6 = areas.Where(s => resultIP.Contains(s.IP));
            var result6_1 = areas;

            //var result6 = areas.Distinct();


            /*h.	Для таблиц Area и Timer, отобразить данные из таблицы
             * – Timer, только для зон/участок – 22, 23, 24, 25, 26, 27, 28*/

            var query = from a in areas
                        join t in timers
                        on a.AreaId equals t.AreaId
                        where a.AssemblyArea == 28
                        select new { a.WorkingPeople, t.DateStart, t.DateFinish }; 
                        

            foreach (var item in query)
            {
                Console.WriteLine(item.WorkingPeople);
            }

        }

        class Area
        {
            public int AreaId { get; set; }
            public int TypeArea { get; set; }
            public string IP { get; set; }
            public int WorkingPeople { get; set; }
            public int AssemblyArea { get; set; }

            public int? OrderExecution { get; set; }  // nullable
        }

        public class Timer
        {
            public int TimerId { get; set; }
            public int UserId { get; set; }
            public int AreaId { get; set; }
            public int DocumentId { get; set; }
            public DateTime DateStart { get; set; }
            public DateTime? DateFinish { get; set; }
            public int DurationInSeconds { get; set; }
        }

    }
}
