using Business_Logic.MessagesModule.DataObjects;
using Business_Logic.MessagesModule.EntitiesExtensions;
using Business_Logic.SqlContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule.Mechanisms {

    public class MessageProductionData {
        /// <summary>
        /// Contains text data:
        /// groupped by recepient adress,
        /// IDictionary contains: key: name of field of DataBase Table, value: value of this field.
        /// </summary>
        public IEnumerable< IGrouping<string, IDictionary<string, object>> > TextProductionData { get; set; }
        /// <summary>
        /// Contains wildcards presented as:
        /// key: {code} word thas should be in text template;
        /// value: name of field in DataBase Table.
        /// </summary>
        public IEnumerable<KeyValuePair<string,string>> wildCards { get; set; }
    }

    public class MessageDataCollector : BatchCreationComponent {
        readonly ISqlLogic sqlLogic;

        public MessageDataCollector (BatchCreationManager manager, ISqlLogic _sqlLogic) : base(manager)
        {
            sqlLogic = _sqlLogic;
        }

        /// <summary>
        /// Returns enumerable for each Recepient in template.
        /// </summary>
        public IEnumerable< MessageProductionData > Collect (IMessageTemplate templ) {
            var output = new List<MessageProductionData>();
            //Migrate all needed data to memory, because it will be intensive processed
            var filters = templ.Filters.ToArray();
            var wildcards = templ.Wildcards.ToArray();
            var recepients = templ.Recepients.ToArray();
            var userInputedValues = templ.FilterValueContainers.ToArray();

            Dictionary<int, ValueOperatorPair[]> filtsToValOps = GetFiltersActualSettings(filters, userInputedValues);

            var Condition = SqlPredicate.BuildAndNode();
            if (filters.Length > 0) {
                foreach (var f in filters) {
                    var orNode = SqlPredicate.BuildOrNode();
                    var valops = filtsToValOps[f.Id];
                    foreach (var valop in valops) {
                        orNode.Append(SqlPredicate.BuildEndNode(f.Key, valop.Operator, valop.Value, f.Type));
                    }
                    Condition.Append(orNode);
                }
            }

            //Build list of needed colomns
            var colomns = wildcards.Select(x => x.Key)
                .Concat(recepients.Select(x => x.EmailKey))
                .Concat(recepients.Select(x => x.NameKey))
                .Concat(recepients.Select(x => x.PhoneKey)).Distinct();

            var sqlData = sqlLogic.FetchData(colomns, templ.TableWithKeysName, "dbo", Condition);

            var wildcardsSummed = wildcards.SelectMany(x => x.ToKeyValues());

            foreach (var rec in recepients) {
                var prodData = new MessageProductionData();
                prodData.TextProductionData = sqlData
                    .Where(x => !string.IsNullOrWhiteSpace(x[rec.EmailKey].ToString()))
                    .GroupBy(x => x[rec.EmailKey].ToString());
                prodData.wildCards = wildcardsSummed.Concat(rec.ToKeyValues());
                output.Add(prodData);
            }

            return output;
        }

        //------------------------------------------
        //Private Part

        /// <summary>
        /// Returns Dictionary:
        /// key: filter ID
        /// value: actual settings
        /// </summary>
        Dictionary<int, ValueOperatorPair[]> GetFiltersActualSettings(tblFilter[] filters, FilterValueContainer[] userInputedValues) {
            var filtsToValOps = new Dictionary<int, ValueOperatorPair[]>();
            foreach (var f in filters) {
                var valops = tblFilterHelper.GetValueOperatorPairs(f, sqlLogic);
                if (NullBoolToBool(f.allowUserInput)) {
                    valops = valops
                        //HERE WE USE STRING CHECK to Compare... =\
                        .Where(x => userInputedValues.Any
                            (y => y.FilterId == f.Id && y.Values != null
                                && y.Values.Any(z => ToStringSafe(z) == x.Value.ToString())
                            )
                        )
                        .ToArray();
                }
                filtsToValOps.Add(f.Id, valops);
            }

            return filtsToValOps;
        }

        //------------------------------------------
        //Utility Part

        static bool NullBoolToBool (bool? b) {
            return b.HasValue && b.Value;
        }

        static string ToStringSafe(object obj) {
            if (obj == null)
                return "";
            return obj.ToString();
        }

    }


}
