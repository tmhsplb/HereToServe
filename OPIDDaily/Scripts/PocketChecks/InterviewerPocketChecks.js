﻿var rowsToColor = [];

$("#historyGrid").jqGrid({
    url: "GetClientPocketChecks", 
    datatype: "json",
    mtype: "Get",
    colNames: ['Id', 'Date', 'Item', 'Check', 'Status', 'Sender', 'Notes'],
    colModel: [
        { key: false, hidden: true, name: 'Id', index: 'Id' },  // Id may not be unique!
        { key: false, align: 'center', name: 'Date', index: 'Date', formatter: 'date', width: 80, editable: true, sortable: true, search: false },
        { key: false, name: 'Item', index: 'Item', width: 80, editable: true, sortable: false, search: false },
        { key: false, name: 'Check', index: 'Check', width: 80, editable: true, sortable: false, search: false },
        { key: false, name: 'Status', index: 'Status', width: 100, editable: true, edittype: 'select', editoptions: { value: { '': '', 'Cleared': 'Cleared', 'Voided': 'Voided', 'Gift Card': 'Gift Card', 'Gift Card Delivered': 'Gift Card Delivered', 'Voided/No Reissue': 'Voided/No Reissue', 'Voided/Resissued': 'Voided/Reissued', 'Voided/Replaced': 'Voided/Replaced', 'Gift Card Delivered': 'Gift Card Delivered', 'Scammed Check': 'Scammed Check', 'Used': 'Used', 'Not Used': 'Not Used' } }, sortable: false, search: false },
        { key: false, hidden: true, name: 'Sender', index: 'Sender', formatter: rowColorFormatter, editable: false, sortable: false, search: false },
        { key: false, hidden: false, name: 'Notes', index: 'Notes', width: 150, editable: true, sortable: false, search: false, edittype: 'textarea', editoptions: { rows: '2', columns: '10' } }    
    ],
    pager: '#historyPager',
    rowNum: 25,

    height: "100%",
    viewrecords: true,
    loadonce: false,

    gridComplete: function () {
        for (var i = 0; i < rowsToColor.length; i++) {
             // alert("colored row: " + rowsToColor[i].rowId + " " + rowsToColor[i].rowColor);
            $("#" + rowsToColor[i].rowId).css("color", rowsToColor[i].rowColor)
        }
    },

    caption: 'Checks for ' + GetClientName(),
    emptyrecords: 'No records to display',
    jsonReader: {
        root: "rows",
        page: "page",
        total: "total",
        records: "records",
        repeatitems: false,
        id: "Id"
    },
    autowidth: true,
    multiselect: false,
})

jQuery("#historyGrid").jqGrid('navGrid', '#historyPager', { edit: true, add: true, del: false, search: false, refresh: false },
    {
        zIndex: 100,
        url: "EditVisit", 
        closeOnEscape: true,
        closeAfterEdit: true,
        recreateForm: true,
        afterComplete: function (response) {
            if (response.responseText) {
                //  alert("FrontDesk: " + response.responseText);
            }
        }
    },
    {
        zIndex: 100,
        url: "AddPocketCheck",
        closeOnEscape: true,
        closeAfterAdd: true,
        recreateForm: true,
        afterComplete: function (response) {
            if (response.responseText) {
                if (response.responseText != "Success") {
                    alert(response.responseText);
                }
            }
        }
    }
    /*
    {
        zIndex: 100,
        url: "DeletePocketCheck", // "@Url.Action("DeleteVisit", "FrontDesk")",
        closeOnEscape: true,
        closeAfterDelete: true,
        recreateForm: true,
        afterComplete: function (response) {
            if (response.responseText) {
                //   alert(response.responseText);
            }
        }
    }
    */
    );

// See: https://stackoverflow.com/questions/3908171/jqgrid-change-row-background-color-based-on-condition
function rowColorFormatter(cellValue, options, rowObject) {
    if (cellValue != null && cellValue == "FromAgency") {
        rowsToColor[rowsToColor.length] = { rowId: rowObject.Id, rowColor: "#00FF00" };  // green
    } else if (cellValue != null && cellValue == "FromOPID") {
        rowsToColor[rowsToColor.length] = { rowId: rowObject.Id, rowColor: "#FF0000" };  // red
    } 
    return cellValue;
}



// http://www.trirand.com/blog/?page_id=393/help/how-to-use-add-form-dialog-popup-window-set-position
$.extend($.jgrid.edit, {
    recreateForm: true,
    closeAfterAdd: true,
    dataheight: '100%',
    reloadAfterSubmit: true,
    width: 500,
    top: 300,
    left: 150,
    addCaption: "Add Visit",
    editCaption: "Edit Visit",
    bSubmit: "Submit",
    bCancel: "Cancel",
    bClose: "Close",
    saveData: "Data has been changed! Save changes?",
    bYes: "Yes",
    bNo: "No",
    bExit: "Cancel"
});