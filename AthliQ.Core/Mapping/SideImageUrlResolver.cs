using AthliQ.Core.DTOs.Child;
using AthliQ.Core.Entities.Models;
using AutoMapper;
using Microsoft.Extensions.Configuration;


namespace AthliQ.Core.Mapping
{
	internal class SideImageUrlResolver(IConfiguration _configuration) : IValueResolver<Child, GetChildDetailsDto, string>
	{
		public string Resolve(Child source, GetChildDetailsDto destination, string destMember, ResolutionContext context)
		{
			var baseUrl = _configuration.GetSection("Urls")["BaseUrl"];
			if (string.IsNullOrEmpty(source.ImageSideURL))
				return string.Empty;
			return $"{baseUrl}Images/{source.ImageSideURL}";
		}
	}
}
