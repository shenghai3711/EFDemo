using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            DataModel dataModel = new DataModel();
            T_Customer t_Customer = new T_Customer
            {
                Address = "东海五彩金轮",
                Age = 27,
                UserName = "楚留香"
            };
            dataModel.T_Customer.Add(t_Customer);
            dataModel.SaveChanges();
            Console.WriteLine("ok");

            Console.ReadKey();
        }
    }
}
