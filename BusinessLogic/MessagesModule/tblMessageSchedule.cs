//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Business_Logic.MessagesModule
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblMessageSchedule
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblMessageSchedule()
        {
            this.tblMessageBatches = new HashSet<tblMessageBatch>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int tblTemplateId { get; set; }
        public Nullable<System.DateTime> ScheduleDate { get; set; }
        public string RepeatMode { get; set; }
        public bool IsActive { get; set; }
        public Nullable<bool> InArchive { get; set; }
        public bool IsSms { get; set; }
        public string MsgHeader { get; set; }
        public string MsgBody { get; set; }
        public string FilterValueContainersJSON { get; set; }
        public string ChoosenReccardIdsJSON { get; set; }
    
        public virtual tblTemplate tblTemplate { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMessageBatch> tblMessageBatches { get; set; }
    }
}
