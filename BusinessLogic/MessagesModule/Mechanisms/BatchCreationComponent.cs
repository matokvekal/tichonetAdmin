using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule.Mechanisms {

    public abstract class BatchCreationComponent {

        protected readonly MessagesModuleLogic logic;

        protected BatchCreationComponent (BatchCreationManager manager) {
            if (manager.IsDisposed)
                throw new InvalidOperationException("Cannot create BatchCreationComponent:" + GetType()+ " on Disposed BatchCreationManager");
            logic = manager.Logic;
        }

    }
}
