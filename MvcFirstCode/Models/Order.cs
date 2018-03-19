using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcFirstCode.Models
{
    public class Order
    {
        /// <summary>
        /// 如果属性名称后面包含Id，则默认会当作主键，不用添加Key特性
        /// </summary>
        [Key]
        public int OrderId { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [StringLength(50)]
        public string OrderCode { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal OrderAmount { get; set; }

        /// <summary>
        /// 导航属性设置成virtual，可以延迟加载
        /// </summary>
        public virtual List<OrderDetail> OrderDetail { get; set; }
    }
}