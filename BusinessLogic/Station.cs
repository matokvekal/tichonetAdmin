
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
    
public partial class Station
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Station()
    {

        this.StationsToLines = new HashSet<StationsToLine>();

        this.StudentsToLines = new HashSet<StudentsToLine>();

    }


    public int Id { get; set; }

    public string Lattitude { get; set; }

    public string Longitude { get; set; }

    public string StationName { get; set; }

    public string color { get; set; }

    public int StationType { get; set; }

    public string Address { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<StationsToLine> StationsToLines { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<StudentsToLine> StudentsToLines { get; set; }

}

}
