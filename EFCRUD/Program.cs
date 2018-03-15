﻿using System;
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


    }
}
