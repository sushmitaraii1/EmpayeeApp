//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EmpayeeApp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Calculation
    {
        public int UserId { get; set; }
        public string Employee_Name { get; set; }
        public bool Married { get; set; }
        public decimal Monthly_Salary { get; set; }
        public bool CIT { get; set; }
        public bool PF { get; set; }
        public decimal Taxable_Amount { get; set; }
        public decimal Tax { get; set; }
        public decimal Salary { get; set; }
        public System.DateTime Month { get; set; }
        public int Days { get; set; }
        public int Leave { get; set; }
        public int StaffId { get; set; }
        public decimal Allowance { get; set; }
        public decimal Bonus { get; set; }
    
        public virtual Staff Staff { get; set; }
    }
}