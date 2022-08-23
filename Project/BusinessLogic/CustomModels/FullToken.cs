using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable disable
namespace Ndif.BusinessLogicLayer.CustomModels
{
    public class FullToken
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
