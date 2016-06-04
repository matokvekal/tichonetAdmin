using System.Linq;
using System.Web.Http;
using Business_Logic;
using Business_Logic.Entities;
using ticonet.Models;

namespace ticonet.Controllers
{
    public class MapController : ApiController
    {
        /// <summary>
        /// Select all data for show map
        /// </summary>
        /// <returns></returns>
        [ActionName("State")]
        public MapStateModel GetState()
        {
            var res = new MapStateModel();
            using (var logic = new LineLogic())
            {
                res.Lines = logic.GetList().Select(z => new LineModel(z)).ToList();
                foreach (var line in res.Lines)
                {
                    line.Stations =  logic.GetStations(line.Id)
                        .OrderBy(z=>z.Position)
                        .Select(z=>new StationToLineModel(z))
                        .ToList();
                }
            }
            using (var logic = new StationsLogic() )
            {
                res.Stations = logic.GetList().Select(z => new StationModel(z)).ToList();
                foreach (var station in res.Stations)
                {
                    station.Students = logic.GetStudents(station.Id)
                        .Select(z=> new StudentToLineModel(z))
                        .ToList();
                }
            }
            using (var logic= new tblStudentLogic())
            {
                res.Students = logic.GetActiveStudents()
                    .Select(z => new StudentShortInfo(z))
                    .ToList();
            }
            return res;
        }
    }
}
