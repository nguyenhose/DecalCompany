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
    
    public partial class Process
    {
        public int Id { get; set; }
        public Nullable<int> OrderId { get; set; }
           
        public Nullable<int> OutPutQ { get; set; }
     
        public Nullable<int> Width { get; set; }
          
        public Nullable<int> Height { get; set; }
         
        public Nullable<int> Square { get; set; }
        public Nullable<int> Done { get; set; }
        public Nullable<int> Rest { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    
        public virtual Order Order { get; set; }
    }
}