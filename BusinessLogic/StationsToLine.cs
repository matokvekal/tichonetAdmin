
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Business_Logic
{

using System;
    using System.Collections.Generic;
    
public partial class StationsToLine
{

    public int Id { get; set; }

    public int StationId { get; set; }

    public int LineId { get; set; }

    public int Position { get; set; }

    public System.TimeSpan ArrivalDate { get; set; }

    public string color { get; set; }

    public string distanceFromStation { get; set; }



    public virtual Line Line { get; set; }

    public virtual Station Station { get; set; }

}

}
