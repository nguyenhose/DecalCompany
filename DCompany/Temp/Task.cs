//------------------------------------------------------------------------------
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
    
    public partial class Task
    {
        public int Id { get; set; }
        public string Task1 { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
        public Nullable<int> TimeComplete { get; set; }
        public Nullable<int> State { get; set; }
        public Nullable<bool> Invisible { get; set; }
        public Nullable<int> UserId { get; set; }
    
        public virtual Staff Staff { get; set; }
    }
}
