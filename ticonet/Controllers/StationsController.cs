using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using Business_Logic;
using Business_Logic.Helpers;
using ticonet.Models;

namespace ticonet.Controllers
{
    public class StationsController : ApiController
    {
        [System.Web.Http.ActionName("List")]
        public List<StationModel> GetList()
        {
            var res = new List<StationModel>();
            using (var logic = new StationsLogic())
            {
                var lst = logic.GetList();
                foreach (var st in lst)
                {
                    res.Add(new StationModel(st));
                }
            }
            return res;
        }


        [System.Web.Http.ActionName("Save")]
        public JsonResult PostSave(StationModel model)
        {
            double lat = 0;
            double lng = 0;
            double.TryParse(StringHelper.FixDecimalSeparator(model.StrLat), out lat);
            double.TryParse(StringHelper.FixDecimalSeparator(model.StrLng), out lng);

            var station = new Station
            {
                Id = model.Id,
                color = model.Color,
                StationName = model.Name,
                Lattitude = lat.ToString(CultureInfo.InvariantCulture),
                Longitude = lng.ToString(CultureInfo.InvariantCulture)
            };
            var res= new SaveStationResultModel();
            
            using (var logic = new StationsLogic())
            {
                var stRes = logic.Save(station);
                res.Station = stRes == null ? null : new StationModel(stRes);
                res.Students = logic.GetAttachedStudentsIds(station.Id);
            }

            return new JsonResult { Data = res };
        }

        [System.Web.Http.ActionName("Delete")]
        public JsonResult PostDelete(int id)
        {
            bool res;
            using (var logic = new StationsLogic())
            {
                res = logic.Delete(id);
            }
            return new JsonResult { Data = res };
        }

        [System.Web.Http.ActionName("AttachStudent")]
        public JsonResult PostAttachStudent(AttachStudentModel model)
        {
            bool res;
            using (var logic = new StationsLogic())
            {
                res = logic.AttachStudent(model.StudentId, model.StationId, model.Distance);
            }
            return new JsonResult { Data = res };
        }

        [System.Web.Http.ActionName("AddToLine")]
        public SaveStationToLineResult PostAddToLine(AddStationToLineModel model)
        {
            var ts = new TimeSpan(model.Hours,model.Minutes,0);
            var res = new SaveStationToLineResult();
            using (var logic = new StationsLogic())
            {
                res.Done = logic.AddToLine(
                    model.StationId, 
                    model.LineId, 
                    ts, 
                    model.Position, 
                    model.ChangeColor);

                res.Station = new StationModel(logic.GetStation(model.StationId));
                res.Station.Students = logic.GetStudents(model.StationId)
                        .Select(z => new StudentToLineModel(z))
                        .ToList();
            }
            using (var logic = new LineLogic())
            {
                res.Line = new LineModel(logic.GetLine(model.LineId));
                res.Line.Stations = logic.GetStations(model.LineId)
                        .OrderBy(z => z.Position)
                        .Select(z => new StationToLineModel(z))
                        .ToList();
            }
            return res;
        }

        [System.Web.Http.ActionName("SaveOnLine")]
        public SaveStationToLineResult PostSaveOnLine(AddStationToLineModel model)
        {
            var ts = new TimeSpan(model.Hours, model.Minutes, 0);
            var res = new SaveStationToLineResult();
            using (var logic = new StationsLogic())
            {
                res.Done = logic.SaveOnLine(
                    model.StationId,
                    model.LineId,
                    ts,
                    model.Position,
                    (model.StrChangeColor??"off").ToLower()=="on");

                res.Station = new StationModel(logic.GetStation(model.StationId));
                res.Station.Students = logic.GetStudents(model.StationId)
                        .Select(z => new StudentToLineModel(z))
                        .ToList();
            }
            using (var logic = new LineLogic())
            {
                res.Line = new LineModel(logic.GetLine(model.LineId));
                res.Line.Stations = logic.GetStations(model.LineId)
                        .OrderBy(z => z.Position)
                        .Select(z => new StationToLineModel(z))
                        .ToList();
            }
            return res;
        }

        [System.Web.Http.ActionName("DeleteFomLine")]
        public SaveStationToLineResult PostDeleteFomLine(AddStationToLineModel model)
        {
            var res = new SaveStationToLineResult();
            using (var logic = new StationsLogic())
            {
                logic.DeleteFromLine(model.StationId, model.LineId);
                res.Station = new StationModel{Id = model.StationId};
            }
            using (var logic = new LineLogic())
            {
                res.Line = new LineModel(logic.GetLine(model.LineId))
                {
                    Stations = logic.GetStations(model.LineId)
                        .OrderBy(z => z.Position)
                        .Select(z => new StationToLineModel(z))
                        .ToList()
                };
            }
            return res;
        }
    }
}
