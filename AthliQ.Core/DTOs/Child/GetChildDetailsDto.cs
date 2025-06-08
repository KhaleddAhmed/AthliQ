
using AthliQ.Core.DTOs.Test;

namespace AthliQ.Core.DTOs.Child
{
	public class GetChildDetailsDto
	{
		public string Name { get; set; }
		public string Gender { get; set; }
		public DateOnly DateOfBirth { get; set; }
		public string SchoolName { get; set; }
		public string? ClubName { get; set; }

		public string PreferredSport { get; set; }
		public string PreferredSportAr { get; set; }
		public string ParentSportHistory { get; set; }
		public string ParentSportHistoryAr { get; set; }

		public double Height { get; set; }
		public double Weight { get; set; }

		public string ImageFrontURL { get; set; }
		public string ImageSideURL { get; set; }

		public List<TestWithValueDto> Tests { get; set; }

		public string Category { get; set; }
		public string CategoryAr { get; set; }
		public List<string> Sports { get; set; }
		public List<string> SportsAr { get; set; }
	}
}
