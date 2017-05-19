TTCSSub();
SubscribeInformation("T24M");

function TTCSSub() {
    setInterval(function () {
        try {
            SubscribeInformation("T24M");
        } catch (err) { }
    }, 1000);
}

function LastestData() {
    $("#TelescopeAz").val(GetDataByDeviceName("T24TS", "deviceserver.devices.telescope.attributes.position.detvel.actfocusrotcompel").Value);
}

$(document).ready(function () {

    function DisplayValue() {
        //$("#TelescopeAz").val(GetDataByCommandName("tcs.information.eosdomeserverlifetime").Value)
        //$("#TelescopeAz").val());
        //$("#TelescopeAz").val(GetDataByDeviceName("T24TS", "deviceserver.devices.telescope.attributes.position.detvel.actfocusrotcompel").Value);
    }
});