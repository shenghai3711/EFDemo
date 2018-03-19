using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MvcFirstCode.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }

        /// <summary>
        /// 订单明细价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 订单明细数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 外键，如果属性名和Order主键名称一样，默认会当成外键，可以不用加ForeignKey特性。
        /// ForeignKey里面的值要和导航属性的名称一致
        /// </summary>
        //[ForeignKey(nameof(MvcFirstCode.Models.Order.OrderId))]
        public int OrderId { get; set; }

        /// <summary>
        /// 导航属性
        /// </summary>
        public virtual Order Order { get; set; }
    }
}