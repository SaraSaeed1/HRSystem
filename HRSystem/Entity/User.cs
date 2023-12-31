//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HRSystem.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.Experiences = new HashSet<Experience>();
        }
    



        public string Address { get; set; }
        public Nullable<int> Is_Active { get; set; }
        public int Id { get; set; }
        //[Required(ErrorMessage = "Enter UserName")]
        public string UserName { get; set; }

        //[Required(ErrorMessage = "Enter Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        //[Required(ErrorMessage = "Enter RePassword")]
        //[NotMapped]
        [DataType(DataType.Password)]
        //[Compare("Password", ErrorMessage = "Password must match")]
        public string RePassword { get; set; }
        //[Required(ErrorMessage = "Enter Name")]
        public string Name { get; set; }

        public int Salary { get; set; } = 0;
        public Nullable<int> Role_Id { get; set; }
        public Nullable<int> Department_id { get; set; }
        public Nullable<int> Experience_id { get; set; }

        //public virtual Department Department { get; set; }
        public virtual Experience Experience { get; set; }
        public virtual Role Role { get; set; }

public virtual Department Department { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Experience> Experiences { get; set; }
    }
}
