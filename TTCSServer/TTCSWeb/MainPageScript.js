var URL = "http://192.168.70.210:5001/TTCS";
var DataSource = "";

function SubscribeInformation(SiteName, DeviceName) {
    $.ajax({
        url: URL,
        type: 'GET',
        data: { SiteName: SiteName, DeviceName: DeviceName },
        cache: false,
        dataType: null,
        async: false,
        success: function (data) {
            DataSource = JSON.parse(data);
            LastestData();
        },
        error: branchAjaxFailed
    });
}

function SubscribeInformation(SiteName) {
    $.ajax({
        url: URL,
        type: 'GET',
        data: { SiteName: SiteName },
        cache: false,
        dataType: null,
        async: false,
        success: function (data) {
            DataSource = JSON.parse(data);
            LastestData();
        },
        error: branchAjaxFailed
    });
}

function GetDataByDeviceName(DeviceName, CommandName) {
    for (var i = 0; i < DataSource.length; i++)
        if (DataSource[i].DeviceName == DeviceName)
            for (var j = 0; j < DataSource[i].Info.length; j++)
                if (DataSource[i].Info[j].CommandName == CommandName)
                    return DataSource[i].Info[j];

    return null;
}

function GetDataByCommandName(CommanName) {
    for (var i = 0; i < DataSource.length; i++)
        if (DataSource[i].CommandName == CommanName)
            return DataSource[i];

    return null;
}

function branchAjaxFailed() {

}

$(document).ready(function () {

    $("#button").click(function () {
        var NameValue = "EAK";

        //alert(showGetResult());

        //var test = $.get("http://192.168.43.68:5001/TTCS", { NameValue: "EAK" });
        //alert(JSON.stringify(test));

        //$.get("http://192.168.43.68:5001/TTCS/?NameValue=EAK", function (a) {
        //    alert("Data: " + JSON.stringify(a));
        //})
        $.ajax({
            url: "http://192.168.70.210:5001/TTCS",
            type: 'GET',
            data: { SiteName: "T07MAirfoce", DeviceName: "T07AFTS", CommandName: "T07AFTS.Telescope.DateTime" },
            cache: false,
            dataType: null,
            async: false,
            success: function (data) {
                alert(JSON.stringify(data));
            },
            error: branchAjaxFailed
        });
    });

    function branchAjaxFailed(result) {
        if (result.status == 200 && result.statusText == "OK") {
            //this is here only because chrome breaks on this method only for no reason whatsoever.
            //chrome sees the request as failed, but everything happens fine...
            alert("Chrome error but OK :" + result.responseText);
            //branchDetailsSuccess(result.responseText);
        }
        else {
            alert("FAILED : " + result.status + ' ' + result.statusText);
        }
    }

    function showGetResult(callback) {
        var scriptURL = "http://192.168.70.210:5001/TTCS/GetName?NameValue=EAK";
        return $.get(scriptURL, {}, callback);
    }

    $("#Telescope24Mnu").click(function () {
        window.location("http://192.168.70.210:5000//Web/Telescope24/Telescope24.html");
    });

    $("#NearEarthMnu").click(function () {
        //$.ajax({
        //    url: "http://192.168.70.210:5001/TTCS",
        //    type: 'GET',
        //    data: { SiteName: "T07MAirfoce", DeviceName: "T07AFTS", CommandName: "T07AFTS.Telescope.DateTime" },
        //    cache: false,
        //    dataType: null,
        //    async: false,
        //    success: function (data) {
        //        alert(JSON.stringify(data));
        //    },
        //    error: branchAjaxFailed
        //});

        //$.ajax({
        //    url: "http://192.168.70.210/TTCS",
        //    type: 'GET',
        //    data: { SiteName: "T07MAirfoce", DeviceName: "T07AFTS" },
        //    cache: false,
        //    dataType: null,
        //    async: false,
        //    success: function (data) {
        //        alert(JSON.stringify(data));
        //    },
        //    error: branchAjaxFailed
        //});

        $.ajax({
            url: "http://192.168.70.210:5001/TTCS",
            type: 'GET',
            data: { SiteName: "T07MAirfoce" },
            cache: false,
            dataType: null,
            async: false,
            success: function (data) {
                alert(JSON.stringify(data));
            },
            error: branchAjaxFailed
        });
    });
});