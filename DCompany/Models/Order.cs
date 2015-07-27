﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DCompany.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class Order
    {
        public Order()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
            this.Processes = new HashSet<Process>();
        }

        public int Id { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string Note { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public Nullable<System.DateTime> DateInfo { get; set; }
        public Nullable<int> State { get; set; }
        public Nullable<int> PayTimes { get; set; }
        public Nullable<int> CreaterId { get; set; }
        public Nullable<bool> Invisible { get; set; }
        [Required(ErrorMessage = "Hãy nhập ngày xuất hàng!")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public Nullable<System.DateTime> Deadline { get; set; }
        [Required(ErrorMessage = "Hãy nhập tên đơn hàng!")]
        [StringLength(100, ErrorMessage = "Địa tối đa chỉ được {1} kí tự!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Hãy nhập giá đơn hàng!")]
        public Nullable<double> TotalCash { get; set; }
        [Required(ErrorMessage = "Hãy nhập ngày bắt đầu!")]
        public Nullable<System.DateTime> DateStart { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Process> Processes { get; set; }
    }
}
