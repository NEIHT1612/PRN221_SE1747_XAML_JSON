using System;
using System.Collections.Generic;

namespace CRUD_WPF.DataAccess
{
    public partial class Department
    {
        public Department()
        {
            Students = new HashSet<Student>();
        }

        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;

        public virtual ICollection<Student> Students { get; set; }
    }
}
