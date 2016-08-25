using Business_Logic.MessagesModule;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ticonet.ParentControllers {

    public class QueryFilter : IQueryFilter {
        public string key { get; set; }
        public object val { get; set; }
        public string op { get; set; }
    }

    public class NgResult {
        public string message { get; protected set; }
        public bool successful { get; protected set; }
        protected NgResult() { }

        public static NgResult Succes (string message = null) {
            return new NgResult { successful = true, message = message };
        }
        public static NgResult Fail(string message = null) {
            return new NgResult { successful = false, message = message };
        }
    }

    public class FetchResult<TModel> : NgResult {
        public static FetchResult<TModel> Succes (IEnumerable<TModel> items, int allquerycount, string message = null) {
            return new FetchResult<TModel> {
                successful = true,
                items = new List<TModel>(items),
                allquerycount = allquerycount,
                message = message
            };
        }
        public static new FetchResult<TModel> Fail(string message = null) {
            return new FetchResult<TModel> {
                successful = false,
                items = null,
                allquerycount = 0,
                message = message
            };
        }

        public List<TModel> items { get; protected set; }
        public int allquerycount { get; protected set; }

        protected FetchResult() {}
    }

    public abstract class NgController<TModel> : JsonController {

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult Fetch(int? Skip, int? Count, QueryFilter[] filters) {
            var result = _fetch(Skip, Count, filters);
            return NgResultToJsonResult(result);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult Manage(string mode, TModel[] models) {
            NgResult result;
            switch (mode) {
                case "cr":
                case "create":
                    result = _create(models);
                    break;
                case "up":
                case "update":
                    result = _update(models);
                    break;
                case "dl":
                case "delete":
                    result = _delete(models);
                    break;
                default:
                    result = NgResult.Fail("Undefined Manage mode: " + mode);
                    break;
            }
            return NgResultToJsonResult(result);
        }

        protected JsonResult NgResultToJsonResult(NgResult result) {
            return result.successful ?
                MakeSuccesResult(result) :
                MakeBadRequest(result.message);
        }

        protected abstract FetchResult<TModel> _fetch(int? Skip, int? Count, QueryFilter[] filters);
        protected abstract NgResult _create(TModel[] models);
        protected abstract NgResult _update(TModel[] models);
        protected abstract NgResult _delete(TModel[] models);
    }
}