
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
    
public partial class tblMessageQueue
{

    public int pk { get; set; }

    public Nullable<long> messageId { get; set; }

    public Nullable<int> messageType { get; set; }

    public Nullable<System.DateTime> dateCreate { get; set; }

    public System.DateTime DueDate { get; set; }

    public Nullable<bool> processed { get; set; }

    public Nullable<int> status { get; set; }

    public string referance { get; set; }

    public Nullable<long> totalReject { get; set; }

    public Nullable<long> totalSent { get; set; }

}

}
