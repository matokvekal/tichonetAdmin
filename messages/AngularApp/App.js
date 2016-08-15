var AngularApp;
(function (AngularApp) {
    var App = angular.module('App', ['ngRoute', 'ngAnimate']);
    App.controller('NotificationController', AngularApp.Controllers.NotificationController);
    App.controller('MessageModulePageController', AngularApp.Controllers.MessageModulePageController);
    App.controller('MFiltersController', AngularApp.Controllers.MFiltersController);
    App.controller('TemplatesController', AngularApp.Controllers.TemplatesController);
    App.controller('SendMessagesController', AngularApp.Controllers.SendMessagesController);
    //todo move to directives
    //DRAG-N-DROP directives
    App.directive('draggable', function () {
        return function (scope, element) {
            var el = element[0];
            el.draggable = true;
            el.addEventListener('dragstart', function (e) {
                e.dataTransfer.effectAllowed = 'move';
                e.dataTransfer.setData('Text', this.id);
                this.classList.add('drag');
                return false;
            }, false);
            el.addEventListener('dragend', function (e) {
                this.classList.remove('drag');
                return false;
            }, false);
        };
    });
    App.directive('droppable', function () {
        return {
            scope: {
                drop: '&' // parent
            },
            link: function (scope, element) {
                var el = element[0];
                el.addEventListener('dragover', function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    e.dataTransfer.dropEffect = 'move';
                    // allows us to drop
                    //if (e.preventDefault) e.preventDefault();
                    this.classList.add('drag-over');
                    return false;
                }, false);
                el.addEventListener('dragenter', function (e) {
                    this.classList.add('drag-over');
                    return false;
                }, false);
                el.addEventListener('dragleave', function (e) {
                    this.classList.remove('drag-over');
                    return false;
                }, false);
                el.addEventListener('drop', function (e) {
                    // Stops some browsers from redirecting.
                    e.preventDefault();
                    e.stopPropagation();
                    //if (e.stopPropagation) e.stopPropagation();
                    var binId = this.id;
                    var item = document.getElementById(e.dataTransfer.getData('Text'));
                    this.classList.remove('drag-over');
                    //use this if you want manipulate DOM
                    //var item = document.getElementById(e.dataTransfer.getData('Text'));
                    //this.appendChild(item);
                    // call the drop passed drop function
                    scope.$apply(function (scope) {
                        var fn = scope['drop']();
                        if ('undefined' !== typeof fn) {
                            fn(item.id, binId, item.getAttribute('draggableclass'));
                        }
                    });
                    return false;
                }, false);
            }
        };
    });
})(AngularApp || (AngularApp = {}));
//# sourceMappingURL=App.js.map