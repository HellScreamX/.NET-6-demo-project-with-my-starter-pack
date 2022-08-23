using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.ExceptionLayer
{
    public static class ErrorManager
    {
        // List of IdentityErrors
        public static Dictionary<string, string> identityErrors = new Dictionary<string, string>()
        {
            { "DuplicateEmail","DuplicateEmail" },
            { "DuplicateUserName","DuplicateUserName" },
            { "InvalidToken","InvalidToken" },
            { "PasswordMismatch", "PasswordMismatch" }
        };

        // Just overload this method when you have different argument.
       public static string GetErrorCode(IdentityResult result)
       {
            var errorMsg = result.Errors.First().Code;
            string? errorCode;
            if (identityErrors.TryGetValue(errorMsg, out errorCode))
            {
                return errorCode;
            }
            else
            {
                return "-1"; // In case error msg doesn't exist in the list
            }
        }

    }
}
