using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace EFCRUD
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("执行Add方法");
            //Add();
            //Console.WriteLine("Add方法执行成功");

            //Console.WriteLine("执行QueryDelay1方法");
            //QueryDelay1();
            //Console.WriteLine("QueryDelay1执行成功");

            //Console.WriteLine("执行QueryDelay2方法");
            //QueryDelay2();
            //Console.WriteLine("执行QueryDelay2方法成功");

            Console.ReadKey();
        }

        #region 新增

        static int Add()
        {
            using (NorthwindEntities db = new NorthwindEntities())
            {
                Customers customers = new Customers
                {
                    CustomerID = "zouqj",
                    Address = "南山区新能源创新产业园",
                    City = "深圳",
                    Phone = "15623235656",
                    CompanyName = "深圳跨境翼电商商务有限公司",
                    ContactName = "zsh"
                };
                DbEntityEntry<Customers> entry = db.Entry<Customers>(customers);
                entry.State = System.Data.Entity.EntityState.Added;
                return db.SaveChanges();
            }
        }

        #endregion

        #region 简单查询+延迟加载

        static void QueryDelay1()
        {
            using (NorthwindEntities db = new NorthwindEntities())
            {
                DbQuery<Customers> dbQuery = db.Customers.Where(u => u.ContactName == "zsh").OrderBy(u => u.ContactName).Take(1) as DbQuery<Customers>;
                //获得延迟查询对象后，调用对象地方法，此时，就会根据之前的条件生成sql语句，查询数据库了
                Customers customers = dbQuery.FirstOrDefault();
                Console.WriteLine(customers.ContactName);
            }
        }

        static void QueryDelay2()
        {
            using (NorthwindEntities db = new NorthwindEntities())
            {
                //真实返回的DbQuery对象，以接口方式返回
                IQueryable<Orders> orders = db.Orders.Where(o => o.CustomerID == "QUICK");
                //此时只查询了订单表
                Orders order = orders.FirstOrDefault();
                //当访问订单对象里的外键实体时，EF会查询订单对应的用户表，查到之后，再将数据装入这个外键实体
                Console.WriteLine(order.Customers.ContactName);

                IQueryable<Orders> orderList = db.Orders;
                foreach (Orders o in orderList)
                {
                    Console.WriteLine(o.OrderID + ":ContactName=" + o.Customers.ContactName);
                }
            }
        }

        #endregion

        #region 根据条件排序和查询

        /// <summary>
        /// 根据条件排序和查询
        /// </summary>
        /// <typeparam name="TKey">排序字段类型</typeparam>
        /// <param name="whereLambda">查询条件Lambda表达式</param>
        /// <param name="orderLambda">排序条件Lambda表达式</param>
        /// <returns></returns>
        public List<Customers> GetListBy<TKey>(Expression<Func<Customers, bool>> whereLambda, Expression<Func<Customers, TKey>> orderLambda)
        {
            using (NorthwindEntities db = new NorthwindEntities())
            {
                return db.Customers.Where(whereLambda).OrderBy(orderLambda).ToList();
            }
        }

        #endregion

        #region 分页查询

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="TKey">排序字段类型</typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="whereLambda">条件Lambda表达式</param>
        /// <param name="orderBy">排序Lambda表达式</param>
        /// <returns></returns>
        public List<Customers> GetPagedList<TKey>(int pageIndex, int pageSize, Expression<Func<Customers, bool>> whereLambda, Expression<Func<Customers, TKey>> orderBy)
        {
            using (NorthwindEntities db = new NorthwindEntities())
            {
                //分页注意：Skip 之前一定要OrderBy
                return db.Customers.Where(whereLambda).OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        #endregion

        #region 修改

        #region 官方推荐方式（先查询，再修改）

        static void Edit1()
        {
            using (NorthwindEntities db = new NorthwindEntities())
            {
                //1.先查询出一个要修改的对象 -- 注意：此时返回的是一个Customers类的代理类对象（包装类对象）
                Customers customer = db.Customers.Where(u => u.CustomerID == "zouqj").FirstOrDefault();
                Console.WriteLine("修改前：" + customer.ContactName);
                //2.修改内容 -- 注意：此时其实操作的是代理类对象的属性，这些属性会将值设置给内部的Customers对象对应的属性，同时标记此属性为已修改状态
                customer.ContactName = "zsh";
                //3.重新保存到数据库 -- 注意：此时EF上下文会检查容器内部所有的对象，先找到标记为修改的对象，然后找到标记为修改的对象属性，生成对应的update语句执行
                db.SaveChanges();
                Console.WriteLine("修改成功");
                Console.WriteLine("修改后：" + customer.ContactName);
            }
        }

        #endregion

        #region 优化的修改方式（创建对象，直接）

        static void Edit2()
        {
            //1.查询出一个要修改的对象
            Customers customer = new Customers
            {
                CustomerID = "zouqj",
                Address = "南山区新能源创新产业园",
                City = "深圳",
                Phone = "15623568989",
                CompanyName = "深圳跨境翼电商商务有限公司",
                ContactName = "zsh"
            };
            using (NorthwindEntities db = new NorthwindEntities())
            {
                //2.将对象加入EF容器，并获取当前实体对象的状态管理对象
                DbEntityEntry<Customers> entry = db.Entry(customer);
                //3.设置该对象为被修改过
                entry.State = System.Data.Entity.EntityState.Unchanged;
                //4.设置该对象的ContactName属性为修改状态，同时entry.State 被修改为Modified状态
                entry.Property("ContactName").IsModified = true;

                //var u = db.Customers.Attach(customer);
                //u.ContactName = "刘德华";
                //5.重新保存到数据库 -- EF上下文会根据实体对象的状态entry.State = Modified 值生成对应的 update sql 语句
                db.SaveChanges();
                Console.WriteLine("修改成功");
                Console.WriteLine(customer.ContactName);
            }

        }

        #endregion

        #endregion

        #region 删除

        static void Delete()
        {
            using (NorthwindEntities db = new NorthwindEntities())
            {
                //1.创建要删除的对象
                Customers customer = new Customers { CustomerID = "zouqj" };
                //2.附加到EF中
                db.Customers.Attach(customer);
                //3.标记为删除 -- 注意：此方法就是标记当前对象为删除状态
                db.Customers.Remove(customer);
                /*
                //也可以使用Entry来附加和删除
                DbEntityEntry<Customers> entry = db.Entry(customer);
                entry.State = System.Data.Entity.EntityState.Deleted;
                */
                //4.执行删除sql
                db.SaveChanges();
                Console.WriteLine("删除成功！");
            }
        }

        #endregion

    }
}
