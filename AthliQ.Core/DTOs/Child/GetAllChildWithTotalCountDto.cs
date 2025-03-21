using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.DTOs.Child
{
	public class GetAllChildWithTotalCountDto
	{
        public int TotalCount { get; set; }
        public List<GetAllChildDto> Children { get; set; }
    }
}
