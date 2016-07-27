using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace ticonet.Controllers {

    public class GenericApiAndMVCController : Controller {

        /// <summary>
        /// msg -> on client side: response.data.message
        /// (instead 'data' can be 'responseJSON')
        /// </summary>
        protected JsonResult MakeBadRequest(string msg = null) {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            if (msg == null)
                return Json(null, JsonRequestBehavior.AllowGet);
            else
                return Json(new { message = msg }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// msg -> on client side: response.data.message
        /// (instead 'data' can be 'responseJSON')
        /// </summary>
        protected JsonResult MakeSuccesResult(string msg = null) {
            Response.StatusCode = (int)HttpStatusCode.Accepted;
            if (msg == null)
                return Json(null, JsonRequestBehavior.AllowGet);
            else
                return Json(new { message = msg }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// data -> on client side: response.data
        /// (instead 'data' can be 'responseJSON')
        /// </summary>
        protected JsonResult MakeSuccesResult(object data) {
            Response.StatusCode = (int)HttpStatusCode.Accepted;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //TODO incaps
        protected JsonResult MakeJqGridResult<TEntity> (List<TEntity> listOfRows, int rowsCount, int totalRecords, int page) {
            Response.StatusCode = (int)HttpStatusCode.Accepted;
            return Json(
                new {
                    total = (totalRecords + rowsCount - 1) / rowsCount,
                    page,
                    records = totalRecords,
                    rows = listOfRows
                },
                JsonRequestBehavior.AllowGet);
        }
    }
}