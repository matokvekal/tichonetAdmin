using Business_Logic;
using Business_Logic.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using ticonet.Models;

namespace ticonet.Controllers
{
    public class ReportApiController : ApiController{

        [System.Web.Mvc.HttpGet]
        public JsonResult GetLinesTotalStatistic(int year) {
            //TODO
            DateTime date = new DateTime(year, 1, 1);
            List<object> result = new List<object>();
            using (var logic = new LineLogic()) {
                for (int i = 0; i < 12; i++) {
                    //result.Add(new LinesTotalStatistic { linesCount = i, totalPrice = 100, totalStudents = 10 });
                    result.Add(logic.GetLinesTotalStatistic(date.AddMonths(i), date.AddMonths(i + 1)));
                }
            }
            return new JsonResult { Data = result };
        }

    }
}
