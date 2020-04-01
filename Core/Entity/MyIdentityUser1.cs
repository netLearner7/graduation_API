using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entity
{
    public class MyIdentityUser:IdentityUser
    {
        public List<User_Meeting>  user_Meetings { get; set; }
    }
}
