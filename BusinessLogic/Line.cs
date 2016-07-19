
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Business_Logic
{

using System;
    using System.Collections.Generic;
    
public partial class Line
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Line()
    {

        this.BusesToLines = new HashSet<BusesToLine>();

        this.StationsToLines = new HashSet<StationsToLine>();

        this.tblSchedules = new HashSet<tblSchedule>();

    }


    public int Id { get; set; }

    public string LineName { get; set; }

    public string LineNumber { get; set; }

    public string HexColor { get; set; }

    public int Direction { get; set; }

    public bool IsActive { get; set; }

    public Nullable<int> totalStudents { get; set; }

    public string PathGeometry { get; set; }

    public Nullable<System.TimeSpan> Duration { get; set; }

    public Nullable<bool> Sun { get; set; }

    public Nullable<System.DateTime> SunTime { get; set; }

    public Nullable<bool> Mon { get; set; }

    public Nullable<System.DateTime> MonTime { get; set; }

    public Nullable<bool> Tue { get; set; }

    public Nullable<System.DateTime> TueTime { get; set; }

    public Nullable<bool> Wed { get; set; }

    public Nullable<System.DateTime> WedTime { get; set; }

    public Nullable<bool> Thu { get; set; }

    public Nullable<System.DateTime> ThuTime { get; set; }

    public Nullable<bool> Fri { get; set; }

    public Nullable<System.DateTime> FriTime { get; set; }

    public Nullable<bool> Sut { get; set; }

    public Nullable<System.DateTime> SutTime { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<BusesToLine> BusesToLines { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<StationsToLine> StationsToLines { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<tblSchedule> tblSchedules { get; set; }

}

}
