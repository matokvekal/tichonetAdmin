using Business_Logic.MessagesModule.DataObjects;
using Business_Logic.MessagesModule.EntitiesExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule {
    public partial class tblMessageSchedule : IMessagesModuleEntity, IMessageTemplate {

        #region IMessageTemplate realisation

        IQueryable<tblFilter> IMessageTemplate.Filters {
            get {
                return tblTemplate.tblRecepientFilter.tblFilters.AsQueryable();
            }
        }

        IEnumerable<FilterValueContainer> IMessageTemplate.FilterValueContainers {
            get {
                return JsonConvert.DeserializeObject<IEnumerable<FilterValueContainer>>(FilterValueContainersJSON);
            }
        }

        string IMessageTemplate.MsgBody {
            get {
                return MsgBody;
            }
        }

        string IMessageTemplate.MsgHeader {
            get {
                return MsgHeader;
            }
        }

        IQueryable<tblRecepientCard> _Recepients;

        IQueryable<tblRecepientCard> IMessageTemplate.Recepients {
            get {
                if (_Recepients == null) {
                    var ids = JsonConvert.DeserializeObject<int[]>(ChoosenReccardIdsJSON);
                    _Recepients = tblTemplate
                        .tblRecepientFilter
                        .tblRecepientCards.Where(x => ids.Any(y => y == x.Id)).AsQueryable();
                }
                return _Recepients;
            }
        }

        string IMessageTemplate.TableWithKeysName {
            get {
                return tblTemplate.tblRecepientFilter.tblRecepientFilterTableName.Name;
            }
        }

        IQueryable<tblWildcard> IMessageTemplate.Wildcards {
            get {
                return tblTemplate.tblRecepientFilter.tblWildcards.AsQueryable();
            }
        }

        #endregion

    }
}
