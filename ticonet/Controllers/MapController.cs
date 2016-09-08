using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Business_Logic;
using Business_Logic.Entities;
using Business_Logic.Helpers;
using log4net;
using ticonet.Models;

namespace ticonet.Controllers
{
    public class MapController : ApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MapController));

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
                    line.Stations = logic.GetStations(line.Id)
                        .OrderBy(z => z.Position)
                        .Select(z => new StationToLineModel(z))
                        .ToList();
                }
            }
            using (var logic = new StationsLogic())
            {
                res.Stations = logic.GetList().Select(z => new StationModel(z)).ToList();
                foreach (var station in res.Stations)
                {
                    station.Students = logic.GetStudents(station.Id)
                        .Select(z => new StudentToLineModel(z))
                        .ToList();
                }
            }
            using (var logic = new tblStudentLogic())
            {
                res.Students = logic.GetActiveStudents()
                    .Select(z => new StudentShortInfo(z))
                    .ToList();
            }
            return res;
        }

        [ActionName("SaveLine")]
        public EditLineResultModel PostSaveLine(LineModel data)
        {
            var res = new EditLineResultModel();
            using (var logic = new LineLogic())
            {
                res.Line = new LineModel(
                    logic.SaveLine(
                                data.Id,
                                data.LineNumber,
                                data.Name,
                                data.Color,
                                data.Direction));
                res.Line.Stations = logic.GetStations(res.Line.Id)
                        .OrderBy(z => z.Position)
                        .Select(z => new StationToLineModel(z))
                        .ToList();
            }
            using (var logic = new StationsLogic())
            {
                res.Stations = logic.GetStationForLine(res.Line.Id)
                    .Select(z => new StationModel(z))
                    .ToList();
                foreach (var station in res.Stations)
                {
                    station.Students = logic.GetStudents(station.Id)
                        .Select(z => new StudentToLineModel(z))
                        .ToList();
                }
            }
            using (var logic = new tblStudentLogic())
            {
                res.Students = logic.GetStudentsForLine(res.Line.Id)
                    .Select(z => new StudentShortInfo(z))
                    .ToList();
            }
            return res;
        }

        [ActionName("SaveGeometry")]
        public List<StationToLineModel> PostSaveGeometry(SaveGeometryModel model)
        {
            var res = new List<StationToLineModel>() ;
            using (var logic = new LineLogic())
            {
                var line = logic.GetLine(model.Id);
                if (line!= null)
                {
                    line.PathGeometry = model.Data;
                    logic.SaveChanges();

                    string fs = "0:0";
                    StationsToLine st = null;
                    st = line.Direction == 0 ? line.StationsToLines.OrderBy(s=>s.Position).Last() : line.StationsToLines.OrderBy(s => s.Position).First();
                    if (line.StationsToLines.Select(l => l.ArrivalDate).Max() > st.ArrivalDate && line.Direction==0)
                        st.ArrivalDate = line.StationsToLines.Select(l => l.ArrivalDate).Max();
                    fs = st.ArrivalDate.Hours + ":" + st.ArrivalDate.Minutes;
                    var data = new SaveDurationsModel
                    {
                        LineId = model.Id,
                        Durations = model.Durations,
                        FirstStation = fs
                    };
                    var ln = logic.ReCalcTimeTable(data);
                    if (ln != null)
                    {
                        res = ln.StationsToLines
                            .OrderBy(z=>z.Position)
                            .Select(z => new StationToLineModel(z))
                            .ToList();
                    }
                }
            }
            return res;
        }

        [ActionName("deleteLine")]
        public EditLineResultModel PostDeleteLine(int id)
        {
            var res = new EditLineResultModel();
            using (var logic = new LineLogic())
            {
                res.Done = logic.DeleteLine(id);
            }
            res.Line = new LineModel { Id = id };
            return res;
        }
        [ActionName("LineActiveSwitch")]
        public LineActiveSwitchModel PostLineActiveSwitch(LineActiveSwitchModel data)
        {
            var res = new LineActiveSwitchModel();
            using (var logic = new LineLogic())
            {
                res.Done = logic.SwitchActive(data.LineId, data.Active);
                res.LineId = data.LineId;
                res.Active = data.Active;
            }
            return res;
        }

        [ActionName("SaveDurations")]
        public LineModel PostSaveDurations(SaveDurationsModel data)
        {
            LineModel res = null;
            using (var logic = new LineLogic())
            {
                var ln = logic.ReCalcTimeTable(data);
                if (ln != null)
                {
                    res = new LineModel(ln)
                    {
                        Stations = ln.StationsToLines.Select(z => new StationToLineModel(z)).ToList()
                    };
                }
            }
            return res;
        }

        [ActionName("SaveState")]
        public bool PostSaveState(CurrentStateModel data)
        {
            var res = false;
            try
            {
                MapHelper.CenterLat = data.CenterLat;
                MapHelper.CenterLng = data.CenterLng;
                MapHelper.Zoom = data.Zoom;
                MapHelper.HiddenLines = data.HiddenLines;
                MapHelper.HiddenStations = data.HiddenStations;
                MapHelper.HiddenStudents = data.HiddenStudents;
                MapHelper.ShowStationsWithoutLine = data.ShowStationsWithoutLine;
                res = true;
            }
            catch (Exception)
            {
                res = false;
                throw;
            }
            return res;
        }


        [HttpPost]
        public bool RefreshColor()
        {
            using (tblStudentLogic logic = new tblStudentLogic())
            {
                return logic.RefreshColor();
            }
        }

    }
}
