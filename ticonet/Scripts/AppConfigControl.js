function AppConfigControl(urldata) {

    this.UpdateSettings = function (settingDict) {
        $.ajax({
            url: urldata.update,
            data: { settings: settingDict },
            type: 'POST',
        });
    }

    this.GetSettings = function(settingArr, callBack) {
        $.ajax({
            url: urldata.get,
            data: { settings: settingArr },
            type: 'POST',
            success: function (data) {
                callBack(data)
            }
        });
    }
}
