var plot;
var isGridLoaded = false;
var isTableLoad = false;
var tableLoad;
$(function () {
   
    $.ajax({
        url: '/api/Chart/GetVmNames/',
        type: 'GET',
        data: 'json',
        success: function (res) {
          //  debugger;
            $.each(res, function (index, value) {
              
                if (index == 0) {
                    var firstVMId = value.Id;
                    if (plot != null) {
                        plot.destroy();
                    }
                    if (tableLoad != null) {
                        $("#VMTable").empty();
                    }
                    getVmUtilizationDataBasedOnId(firstVMId);
                    getVMThresholdDataBasedOnId(firstVMId);
                    getVMAllDetails(firstVMId);
                }


            });
           
        }

    });

});

var user

//debugger;
$(document).ready(function () {
    $("#dialog-message").dialog({
        autoOpen: false,
        modal: true,
        width: 600,
        height: 550,
        open: function () {
            $('.ui-widget-overlay').bind('click', function () {
                $('#dialog').dialog('close');
            })
        },

        buttons: {
            "Update": function () {
              //  debugger;
                if ($("#txtVmName").val().trim() == "") {
                    $("#lblVmNameRequired").show();
                }
                else {
                    $("#lblVmNameRequired").hide();
                }
              // debugger;
                var VMData =
  {

      Name: $("#txtVmName").val(),
      Description: $("#txtVmDescription").val(),

      Location: $('#myLocation :selected').text(),

      UtilFileLocation: $("#imgFile").val()
  };
                $("#loading").show();
                saveVMDetails(VMData);


            }
        }
    });
});
$("#lblVmNameRequired").hide();
$("#imgAdd").click(function () {

    $('#dialog-message').dialog('open');
});

$.ajax({
    url: '/api/Chart/GetVmNames/',
    type: 'GET',
    data: 'json',
    success: function (res) {
        fnDisplayListOfVms(res);
    },

});


function saveVMDetails(VMData) {

    // var VMData =
    //{

    //    Name: $("#txtVmName").val(),
    //    Description: $("#txtVmDescription").val(),
    //    Location: $("#txtLocation").val(),

    //    UtilFileLocation: $("#imgFile").val()
    //};



    $.ajax({

        url: '/api/Chart/AddVM/',
        // url: '@Url.Action("AddVM", "Chart")',
        type: 'POST',
        dataType: 'json',

        data: JSON.stringify(VMData),
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            if (data == true) {
                $("#loading").hide();
               // alert(data);
                alert("Data successfully saved ");
                $('#dialog-message').dialog('close');
                count = null;
                $("#ul-vm").empty();
                $("#txtVmName").empty();
                $("#txtVmDescription").empty();
                $("#imgFile").empty();
              //  debugger;
                $.ajax({
                    url: '/api/Chart/GetVmNames/',
                    type: 'GET',
                    data: 'json',
                    success: function (res) {
                        fnDisplayListOfVms(res);
                    },

                });
            }

        },
        error: function (a, b, c) {
            alert('Error occured while performing the action');

        }
        // return values 

    });
}


var count = 1;
function fnDisplayListOfVms(res) {

    $.each(res, function (index, value) {
       // debugger;

        if (count <= res.length) {
            $("#ul-vm").append('<li><a href="#" id="vm' + index + '">' + value.VMName + '</a></li>');

        }
       
        count++
        //debugger;
        var vmSelected = value.Id; 
     
        $("#vm" + index).click(function (event) {
           // alert("you have selected " + vmSelected);
            if (plot != null) {
                plot.destroy();
            }
            if (tableLoad != null) {
                $("#VMTable").empty();
            }
            getVmUtilizationDataBasedOnId(vmSelected);
            getVMThresholdDataBasedOnId(vmSelected);
            getVMAllDetails(vmSelected);


        });

        // cnt = null;
    });
   
}


function fontColorFormat(cellValue, options, rowObject) {
    var color = "blue";
    var cellHtml = "<span style='cursor:pointer; color:" + color + "' originalValue='" + cellValue + "'>" + cellValue + "</span>";
    return cellHtml;
}

var getVMAllDetails = function (id) {
    var tableLoad1 = $.ajax({
        url: '/Home/VMAllDetails/' + id,
        async: false,
        type: 'GET',
        dataType: "json",
        contentType: "application/json; charset=utf-8",

        success: function (data) {
            $("#VMTable").append('<fieldset class="scheduler-border"><legend class="scheduler-border">Machine Details</legend><div class="control-group"><table><tr>' +
               ' <td style="width:50px;" align="left">Name :</td><td><input type="text"  readonly="readonly"  style="width:170px;" align="left" value=' + data.Name + '>' +
               '</td>&nbsp<td></td><td style="width:10px;">CPU&nbsp;' + '(s):</td><td><input type="text"  readonly="readonly"  style="width:100px;" align="left" value=' + data.CPU + '></td>' +
                '<td>&nbsp</td><td style="width:70px;">Memory :</td><td><input type="text"  readonly="readonly"  style="width:70px;" align="left" value=' + data.Memory + '></td> </tr></table>' +
                '<br/><table><tr><td>Thresholds:</td></tr></table><br style="line-height:8px;" /> ' +
                '<table><tr><td align="left">CPU :</td><td align="left"><input type="text" readonly="readonly" style="width:50px;"  value=' + data.ThresholdCPU + '></td><td>&nbsp</td>' +
                '<td align="left">Memory :</td><td align="left"><input type="text"  readonly="readonly"  style="width:50px;"  value=' + data.ThresholdMemory + '></td><td>&nbsp</td>' +
                '<td align="left">Disk&nbsp;' + 'Write : </td><td align="left"><input type="text"  readonly="readonly"  style="width:50px;"  value=' + data.DiskWrite + '></td><td>&nbsp</td>' +
                '<td align="left">Disk&nbsp;' + 'Read : </td><td align="left"><input type="text"  readonly="readonly"  style="width:50px;"  value=' + data.DiskRead + '></td><td>&nbsp</td>' +
                '<td align="left">Network&nbsp;' + 'In : </td><td align="left"><input type="text"  readonly="readonly"  style="width:50px;"  value=' + data.NetworkIn + '></td><td>&nbsp</td>' +
               '<td>&nbsp</td><td align="left">Network&nbsp;' + 'Out : </td><td align="left"><input type="text"  readonly="readonly"  style="width:50px;"  value=' + data.NetworkOut + '></td></tr></table> </div></fieldset>');
        }

    });

    tableLoad = tableLoad1;
}

var getVMThresholdDataBasedOnId = function (id) {

    if (isGridLoaded) {
        $("#grid").setGridParam({ datatype: 'json' });
        $("#grid").setGridParam({ url: '/api/Chart/GetVmAutomationRuleDataBasedOnId/' + id });
        $("#grid").trigger("reloadGrid");
        $("#grid").setGridParam({ datatype: 'local' });
    }
    else {
        $("#grid").jqGrid({
            url: '/api/Chart/GetVmAutomationRuleDataBasedOnId/' + id,
            datatype: 'json',
            mtype: 'Get',
            colNames: ['Id', 'VMID', 'Utilization', 'Start Date', 'StartTime', 'EndDate', 'EndTime', 'Action', 'Value'],
            colModel: [
                { name: 'Id', index: 'Id', hidden: true },
                { name: 'VMID', index: 'VMID', editable: true },
                { name: 'UtilizationType', index: 'UtilizationType' },

                { name: 'StartDate', index: 'StartDate', formatter: 'date', formatoptions: { srcformat: 'Y-m-d', newformat: 'm/d/Y' } },
                { name: 'StartTime', index: 'StartTime' },
                { name: 'EndDate', index: 'EndDate', formatter: 'date', formatoptions: { srcformat: 'Y-m-d', newformat: 'm/d/Y' } },
                { name: 'EndTime', index: 'EndTime' },
                {
                    name: 'Action', index: 'Action',
                    editable: true,
                    edittype: 'select',
                    editoptions: { value: getAllActions() }
                },
                {
                    name: 'Value', index: 'Value',
                    editable: true,
                    edittype: 'select',
                    editoptions: { value: getAllValues() }
                }

            ],
            cellEdit: true,
            rowNum: 10,
            rowList: [10, 20, 30, 40],
            height: '50%',
            width: '100%',
            viewrecords: true,
            emptyrecords: 'No records to display',
            jsonReader: {
                vmU: "vmAutomationRule",
                repeatitems: false,
                Id: "0"
            },
            autowidth: true,
            multiselect: false,

            loadonce: false,
            caption: "CPU Utilization Detail",
            cellsubmit: 'clientArray',
            editurl: 'clientArray'
        });

        isGridLoaded = true;
    }
}

var getVmUtilizationDataBasedOnId = function (id) {

    $.ajax({
        url: '/api/Chart/GetVmUtilizationDataBasedOnId/' + id,
        async: false,
        type: 'GET',
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            // debugger;
            if (result.length == 0) {
                return false;
            }
            else {
                testJqPlot(result, id);
            }
        },
        error: function (a, b, c) {
            alert('An error occured');

        }
    });

    function testJqPlot(res, id) {
        var data = [];
        $.each(res, function (index, value) {
            data.push([Date.parse(value.Date), value.CPUMaximum]);
        });
        var line1 = data;
        var plot2 = $.jqplot('chart1', [line1], {
            title: 'CPU Utilization Graph',
            axes: {
                xaxis: {
                    renderer: $.jqplot.DateAxisRenderer,
                    rendererOptions: {
                        tickRenderer: $.jqplot.CanvasAxisTickRenderer
                    },
                    pad: 0
                },
                yaxis: {
                    pad: 0
                }
            },
            series: [{
                lineWidth: 2,
                markerOptions: { style: 'filledCircle', size: 6 }
            }],
            highlighter: {
                show: true,
                sizeAdjust: 7.5
            },
            cursor: {
                show: true,
                tooltipLocation: 'sw'
            }
        });

        plot = plot2;
    }
}

function getAllValues() {
    var data;
    var options = new Object();
    $.ajax({
        url: '/api/Chart/GetVmValues',
        type: 'GET',
        dataType: "json",
        async: false,
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            data = result;
        },
        error: function (a, b, c) {
            alert('An error occured');

        }
    });

    for (var i = 0; i < data.length; i++) {
        options[data[i].Id] = data[i].ValuesName;
    }
    return options;
}

function getAllActions() {
    var data;
    var options = new Object();
    $.ajax({
        url: '/api/Chart/GetVmActions',
        type: 'GET',
        dataType: "json",
        async: false,
        contentType: "application/json; charset=utf-8",
        success: function (result) {
            data = result;
        },
        error: function (a, b, c) {
            alert('An error occured');

        }
    });

    for (var i = 0; i < data.length; i++) {
        options[data[i].Id] = data[i].ActionName;
    }
    return options;
}


