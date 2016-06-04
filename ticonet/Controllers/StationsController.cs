using System.Collections.Generic;
using System.Globalization;
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
    }
}
