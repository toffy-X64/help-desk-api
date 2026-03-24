using HelpDesk.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Core.Models.Users
{
    public static class UserErrors
    {
        public static Error InvalidCredentials => new Error("Invalid creadentials provided");
        public static Error EmailAlreadyTaken => new Error("Email is already taken", "Email");
    }
}
