
namespace AthliQ.Core.Entities.Models
{
    public class Child : BaseEntity
    {
        #region General Info
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string SchoolName { get; set; }
        public string? ClubName { get; set; }
        #endregion

        #region HealthPolicies
        public bool IsAgreeDoctorApproval { get; set; } = false;
        public bool IsNormalBloodTest { get; set; } = false;
        #endregion

        #region Sports
        public int? SportHistoryId { get; set; }
        public int ParentSportHistoryId { get; set; }
        public int SportPreferenceId { get; set; }
        #endregion

        #region Physical aspects
        public double Height { get; set; }
        public double Weight { get; set; }
        public string ImageFrontURL { get; set; }
        public string ImageSideURL { get; set; }

        public bool IsNormalBodyImage { get; set; }
        #endregion

        #region Relationships
        public string AthliQUserId { get; set; }
        public AthliQUser AthliQUser { get; set; }

        public virtual ICollection<ChildTest> ChildTests { get; set; }
        public virtual ICollection<ChildResult> ChildResults { get; set; }
        #endregion
    }
}
