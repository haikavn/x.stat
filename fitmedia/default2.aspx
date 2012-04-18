﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default2.aspx.cs" Inherits="fitmedia.default2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" media="screen" href="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.16/themes/redmond/jquery-ui.css" />
    
    <link rel="stylesheet" type="text/css" media="screen" href="/js/trirand/themes/ui.jqgrid.css" />    
    <!-- jQuery runtime minified -->   
    <script src="/js/jquery-1.7.1.min.js" type="text/javascript"></script>   

    <!-- The localization file we need, English in this case -->   
    <script src="/js/jqGrid-4.3.1/js/i18n/grid.locale-en.js" type="text/javascript"></script>   
      
    <!-- The jqGrid client-side javascript -->    
    <script src="/js/jqGrid-4.3.1/js/jquery.jqGrid.min.js" type="text/javascript"></script>   
    <script type="text/javascript" src="/js/highcharts/highcharts.js"></script>
    <script type="text/javascript" src="/js/highcharts/modules/exporting.js"></script> 

	
	<script type="text/javascript" src="js/jquery-ui-1.8.18.custom/js/jquery-ui-1.8.18.custom.min.js"></script>

    
</head>
<body>
    <form id="form1" runat="server">
    <div id="links"><select id="projects"></select><br />
    <input id="startdate" type="text">
    <input id="enddate" type="text"><br /><br />
    <a href="#" id="navigate">Calculate</a></div>
    <br />
    <div>
        <table id="datagrid" style="width: 100%"></table>
        <div id="datagridpager"></div>
    </div>
    <br />
    <div id="container" style="width: 100%; height: 400px; margin: 0 auto; float:left"></div> 
    </form>
</body>


</html>
<script type="text/javascript">
    $(document).ready(function () {
        $("a").button();

        $("#projects").change(function (e) {
            $('#navigate').attr('href', 'datagen.aspx?sd=' + $('#startdate').val() + '&ed=' + $('#enddate').val() + '&pid=' + $('#projects').val());
           // jQuery("#datagrid").jqGrid({
            //    url: 'datagen.aspx?sd=' + $('#startdate').val() + '&ed=' + $('#enddate').val() + '&pid=' + $('#projects').val()
            //});
        });

        $("#startdate").datepicker({ dateFormat: "dd.mm.yy", onSelect: function (dateText, inst) { $('#navigate').attr('href', 'datagen.aspx?sd=' + dateText + '&ed=' + $('#enddate').val() + '&pid=' + $('#projects').val()); } });
        $("#enddate").datepicker({ dateFormat: "dd.mm.yy", onSelect: function (dateText, inst) { $('#navigate').attr('href', 'datagen.aspx?sd=' + $('#startdate').val() + '&ed=' + dateText + '&pid=' + $('#projects').val()); } });
        jQuery("#datagrid").jqGrid({
            url: 'ui/griddata.xml',
            datatype: "xml",
            colNames: ['Период', 'Заказы', 'Под. заказы', 'Отказы', 'Сумма заказов', 'Сумма под. заказов', 'Сумма отказов', 'Реал. сумма заказов', 'Сумма скидок', 'Клики', 'Показы', 'Расходы'],
            colModel: [
   		{ name: 'period', index: 'period', width: 90 },
   		{ name: 'total', index: 'total', width: 50, align: "right" },
   		{ name: 'totala', index: 'totala', width: 50, align: "right" },
   		{ name: 'totalr', index: 'totalr', width: 50, align: "right" },
   		{ name: 'totalsum', index: 'totalsum', width: 80, align: "right" },
   		{ name: 'totalsuma', index: 'totalsuma', width: 80, align: "right" },
   		{ name: 'totalsumr', index: 'totalsumr', width: 80, align: "right" },
   		{ name: 'totalsumd', index: 'totalsumd', width: 80, align: "right" },
   		{ name: 'totalsumdd', index: 'totalsumdd', width: 80, align: "right" },
   		{ name: 'clicks', index: 'clicks', width: 80, align: "right" },
   		{ name: 'shows', index: 'shows', width: 80, align: "right" },
   		{ name: 'expenses', index: 'expenses', width: 80, align: "right", summaryType: 'sum', summaryTpl: '<b>{0} Item(s)</b>' }
   	],
            rowNum: 5000,
            height: 250,
            autowidth: true,
            rowList: [10, 20, 30],
            sortname: 'period',
            viewrecords: true,
            sortorder: "desc",
            caption: "Report",
            grouping: false,
            groupingView: {
                groupField: [''],
                groupSummary: [true],
                groupColumnShow: [false],
                groupText: ['<b>{0}</b>'],
                groupCollapse: false,
                groupOrder: ['asc']
            }

        }).navGrid('#datagridpager', { edit: false, add: false, del: false });

        $.getScript("ui/ui.js");

        
    });				
</script> 

