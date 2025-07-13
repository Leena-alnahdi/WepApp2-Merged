using System;
using System.Collections.Generic;

namespace WepApp2.Models
{
    public partial class User
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string PhoneNumber { get; set; }

        public string UserRole { get; set; }

        public string Faculty { get; set; }

        public string Department { get; set; }

        public string UserPassWord { get; set; }

        public DateTime LastLogIn { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
    }
}