using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.DTOs.User
{
    public class GetAllUsersToReturnDto
    {
        public int Count { get; set; }
        public List<GetAllUserDto> GetAllUserDtos { get; set; }
    }
}
