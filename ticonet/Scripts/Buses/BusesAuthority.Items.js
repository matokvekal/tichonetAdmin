busesAuthority.items = {
    list: [],
    editBus: function (id) {
        if (id && id !== busesAuthority.table.lastSel) {
            busesAuthority.table.grid.restoreRow(busesAuthority.table.lastSel);
            busesAuthority.table.lastSel = id;
        }
        busesAuthority.table.grid.editRow(id, true);

        $('.js-bus-edit-button-container-' + id).hide();
        $('.js-bus-save-button-container-' + id).show();
    },

    cancelEditBus: function (id) {
        busesAuthority.table.grid.restoreRow(id);
        $('.js-bus-save-button-container-' + id).hide();
        $('.js-bus-edit-button-container-' + id).show();
    },

    saveBus: function (id) {
        busesAuthority.table.grid.saveRow(id, true);
    },

    addBus: function () {
        busesAuthority.table.grid.addRow(0);
    },

    deleteBus: function (id) {
    }
}