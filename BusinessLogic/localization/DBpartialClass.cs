namespace Business_Logic
{
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(tblFamilyMetaData))]
    public partial class tblFamily
    {
    }
    public class tblFamilyMetaData
    {
        [Editable(false)]
        [localizedSystemDisplayName("tblFamily.familyId")]
        public object familyId { get; set; }

        [Required(ErrorMessage = "שדה חובה")]
        //[Display(Name = "מספר זהות")]
        [localizedSystemDisplayName("tblFamily.ParentId")]
        public object ParentId { get; set; }

        //[localizedSystemDisplayName("tblFamily.parent1Type")]
        //[Required(ErrorMessage = "שדה חובה")]
        //public object parent1Type { get; set; }

        [localizedSystemDisplayName("tblFamily.parent1FirstName")]
        [Required(ErrorMessage = "שדה חובה")]
        public object parent1FirstName { get; set; }

        [localizedSystemDisplayName("tblFamily.parent1LastName")]
        [Required(ErrorMessage = "שדה חובה")]
        public object parent1LastName { get; set; }

        [localizedSystemDisplayName("tblFamily.parent1Email")]
        [Required(ErrorMessage = "שדה חובה")]
        public object parent1Email { get; set; }

        [localizedSystemDisplayName("tblFamily.parent1EmailConfirm")]
        public object parent1EmailConfirm { get; set; }

        [localizedSystemDisplayName("tblFamily.parent1GetAlertByEmail")]
        public object parent1GetAlertByEmail { get; set; }

        [localizedSystemDisplayName("tblFamily.parent1CellPhone")]
        [Required(ErrorMessage = "שדה חובה")]
        public object parent1CellPhone { get; set; }

        [localizedSystemDisplayName("tblFamily.parent1CellConfirm")]
        public object parent1CellConfirm { get; set; }

        [localizedSystemDisplayName("tblFamily.parent1GetAlertBycell")]
        public object parent1GetAlertBycell { get; set; }



        [localizedSystemDisplayName("tblFamily.parent2Type")]
        public object parent2Type { get; set; }
        [localizedSystemDisplayName("tblFamily.parent2FirstName")]
        public object parent2FirstName { get; set; }
        [localizedSystemDisplayName("tblFamily.parent2LastName")]
        public object parent2LastName { get; set; }
        [localizedSystemDisplayName("tblFamily.parent2Email")]
        public object parent2Email { get; set; }
        [localizedSystemDisplayName("tblFamily.paymentOk")]
        public object paymentOk { get; set; }
        [localizedSystemDisplayName("tblFamily.parent2EmailConfirm")]
        public object parent2EmailConfirm { get; set; }
        [localizedSystemDisplayName("tblFamily.parent2GetAlertByEmail")]
        public object parent2GetAlertByEmail { get; set; }
        [localizedSystemDisplayName("tblFamily.parent2CellPhone")]
        public object parent2CellPhone { get; set; }
        [localizedSystemDisplayName("tblFamily.parent2CellConfirm")]
        public object parent2CellConfirm { get; set; }
        [localizedSystemDisplayName("tblFamily.parent2GetAlertBycell")]
        public object parent2GetAlertBycell { get; set; }

        [localizedSystemDisplayName("tblFamily.date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public object date { get; set; }

        [localizedSystemDisplayName("tblFamily.paymentDateConfirm")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public object paymentDateConfirm { get; set; }

          [Required(ErrorMessage = "שדה חובה")]
        [localizedSystemDisplayName("tblFamily.iAgree")]
        public object iAgree { get; set; }

        [localizedSystemDisplayName("tblFamily.AllreadyUse")]
        public object allredyUsed { get; set; }

    }
    [MetadataType(typeof(tblStudentMetaData))]
    public partial class tblStudent
    {
    }
    public class tblStudentMetaData
    {
        [Editable(false)]
        public object pk { get; set; }
        [Required(ErrorMessage = "שדה חובה")]
        [localizedSystemDisplayName("tblStudent.familyId")]
        public object familyId { get; set; }
        [Required(ErrorMessage = "שדה חובה")]
        [localizedSystemDisplayName("tblStudent.studentId")]
        public object studentId { get; set; }
        [Required(ErrorMessage = "שדה חובה")]
        [localizedSystemDisplayName("tblStudent.firstName")]
        public object firstName { get; set; }
        [Required(ErrorMessage = "שדה חובה")]
        [localizedSystemDisplayName("tblStudent.lastName")]
        public object lastName { get; set; }
        [Required(ErrorMessage = "שדה חובה")]
        [localizedSystemDisplayName("tblStudent.yearRegistration")]
        public object yearRegistration { get; set; }

        [localizedSystemDisplayName("tblStudent.Shicva")]
        public object Shicva { get; set; }
        [Required(ErrorMessage = "שדה חובה")]
        [localizedSystemDisplayName("tblStudent.@class")]
        public object @class { get; set; }
        [Required(ErrorMessage = "שדה חובה")]
        [localizedSystemDisplayName("tblStudent.city")]
        public object city { get; set; }
        [Required(ErrorMessage = "שדה חובה")]
        [localizedSystemDisplayName("tblStudent.street")]
        public object street { get; set; }
        [Required(ErrorMessage = "שדה חובה")]
        [localizedSystemDisplayName("tblStudent.houseNumber")]
        public object houseNumber { get; set; }
        [localizedSystemDisplayName("tblStudent.zipCode")]
        public object zipCode { get; set; }
        [localizedSystemDisplayName("tblStudent.CellPhone")]
        public object CellPhone { get; set; }
        [localizedSystemDisplayName("tblStudent.CellConfirm")]
        public object CellConfirm { get; set; }
        [localizedSystemDisplayName("tblStudent.GetAlertByCell")]
        public object GetAlertByCell { get; set; }
        [localizedSystemDisplayName("tblStudent.Email")]
        public object Email { get; set; }
        [localizedSystemDisplayName("tblStudent.EmailConfirm")]
        public object EmailConfirm { get; set; }
        [localizedSystemDisplayName("tblStudent.GetAlertByEmail")]
        public object GetAlertByEmail { get; set; }
        [localizedSystemDisplayName("tblStudent.paymentStatus")]
        public object paymentStatus { get; set; }
        [localizedSystemDisplayName("tblStudent.registrationStatus")]
        public object registrationStatus { get; set; }
        [localizedSystemDisplayName("tblStudent.dateCreate")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public object dateCreate { get; set; }
        [localizedSystemDisplayName("tblStudent.lastUpdate")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public object lastUpdate { get; set; }
        [localizedSystemDisplayName("tblStudent.subsidy")]
        public object subsidy { get; set; }
    }
}




