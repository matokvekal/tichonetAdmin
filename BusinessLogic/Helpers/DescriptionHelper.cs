namespace Business_Logic.Helpers
{
    public static class DescriptionHelper
    {
        public static string GetBusDescription(Bus bus)
        {
            return string.Format("{0} " +
                "                                                                                                  "
                + " ({1} - {2} - {3} - {4} - {5})",
                bus.Id,
                bus.BusId,
                bus.PlateNumber,
                bus.BusCompany != null ? bus.BusCompany.companyName : string.Empty,
                bus.seats.HasValue ? bus.seats.Value.ToString() : string.Empty,
                bus.price.HasValue ? bus.price.Value.ToString("0.00") : string.Empty
            );
        }
    }
}
