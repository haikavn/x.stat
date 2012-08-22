<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="fitmedia._Default" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <title></title>

    <link rel="stylesheet" type="text/css" media="screen" href="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.16/themes/redmond/jquery-ui.css" />
    <link rel="stylesheet" type="text/css" media="screen" href="js/jqGrid-4.3.1/css/ui.jqgrid.css" />  
  
    <!-- jQuery runtime minified -->   
    <script src="js/jquery-1.7.1.min.js" type="text/javascript"></script>   

    <!-- The localization file we need, English in this case -->   
    <script src="js/jqGrid-4.3.1/js/i18n/grid.locale-en.js" type="text/javascript"></script>   
      
    <!-- The jqGrid client-side javascript -->    
    <script src="js/jqGrid-4.3.1/js/jquery.jqGrid.min.js" type="text/javascript"></script>   
	
	<script type="text/javascript" src="js/jquery-ui-1.8.18.custom/js/jquery-ui-1.8.18.custom.min.js"></script>
</head>
<body class='default'>
    <table>
        <tr>
            <th align="left">
                Проекты:
            </th>
            <th align="left">
                Начало:
            </th>
            <th align="left">
                Конец:
            </th>
            <th align="left">
                Группировать:
            </th>
            <th align="left">
                Город:
            </th>
            <th align="left">
                Источник:
            </th>
            <th align="left">
                Оплата:
            </th>
            <th align="left">
                Дни:
            </th>
            <th align="left">
            </th>
        </tr>
        <tr>
            <th align="left">
                <select id="projects"></select>
            </th>
            <th align="left">
                <input id="startdate" type="text">
            </th>
            <th align="left">
                <input id="enddate" type="text">
            </th>
            <th align="left">
                <select id="groups"></select>
            </th>
            <th align="left">
                <select id="city">
                    <option selected="selected" value="0">Все</option>
                    <option value="1">Москва и МО</option>
                    <option value="2">Регионы</option>
                </select>
            </th>
            <th align="left">
                <select id="source">
                    <option selected="selected" value="0">Все</option>
                    <option value="1">Сайт</option>
                    <option value="2">Телефон</option>                
                </select>
            </th>
            <th align="left">
                <select id="payment">
                    <option selected="selected" value="0">Все</option>
                    <option value="1">Наличными</option>
                    <option value="2">Безнал</option>                 
                </select>
            </th>
            <th align="left">
                <select id="days">
                    <option selected="selected" value="0">Все</option>
                    <option value="1">Выходные</option>
                </select>
            </th>
            <th align="left">
                <a href="#" id="navigate">Показать</a>&nbsp;&nbsp;
                <a href="#" id="edit">Редактировать</a>&nbsp;&nbsp;
                <a href="lidataedit.aspx" id="liveinternet">Live Internet</a>&nbsp;&nbsp;
                <a href="nwdgen.aspx" id="notworkingdays">Нерабочие дни</a>
            </th>
        </tr>
    </table>
    <br />
                <select id="balance" name="balance"></select>
    <br />
    <label id="lastexec"></label><br />
    <label id="errors"></label>
    <br />
	<div id="statsChart"></div>
    <br />
    <div>
        <table id="datagrid" style="width: 100%"></table>
        <div id="datagridpager"></div>
    </div>
    
</body>
</html>

<script type="text/javascript">
    $(document).ready(function () {

        // $("a").button();

        $("#projects").change(function (e) {
            $('#navigate').attr('href', 'Default.aspx?sd=' + $('#startdate').val() + '&ed=' + $('#enddate').val() + '&pid=' + $('#projects').val() + '&g=' + $('#groups').val() + '&city=' + $('#city').val() + '&source=' + $('#source').val() + '&payment=' + $('#payment').val() + '&days=' + $('#days').val());
            $('#edit').attr('href', 'dataedit.aspx?sd=' + $('#startdate').val() + '&ed=' + $('#enddate').val() + '&pid=' + $('#projects').val());
            // jQuery("#datagrid").jqGrid({
            //    url: 'datagen.aspx?sd=' + $('#startdate').val() + '&ed=' + $('#enddate').val() + '&pid=' + $('#projects').val()
            //});
        });

        $("#groups").change(function (e) {
            $('#navigate').attr('href', 'Default.aspx?sd=' + $('#startdate').val() + '&ed=' + $('#enddate').val() + '&pid=' + $('#projects').val() + '&g=' + $('#groups').val() + '&city=' + $('#city').val() + '&source=' + $('#source').val() + '&payment=' + $('#payment').val() + '&days=' + $('#days').val());
            $('#edit').attr('href', 'dataedit.aspx?sd=' + $('#startdate').val() + '&ed=' + $('#enddate').val() + '&pid=' + $('#projects').val());
            // jQuery("#datagrid").jqGrid({
            //    url: 'datagen.aspx?sd=' + $('#startdate').val() + '&ed=' + $('#enddate').val() + '&pid=' + $('#projects').val()
            //});
        });

        $("#city").change(function (e) {
            $('#navigate').attr('href', 'Default.aspx?sd=' + $('#startdate').val() + '&ed=' + $('#enddate').val() + '&pid=' + $('#projects').val() + '&g=' + $('#groups').val() + '&city=' + $('#city').val() + '&source=' + $('#source').val() + '&payment=' + $('#payment').val() + '&days=' + $('#days').val());
        });

        $("#source").change(function (e) {
            $('#navigate').attr('href', 'Default.aspx?sd=' + $('#startdate').val() + '&ed=' + $('#enddate').val() + '&pid=' + $('#projects').val() + '&g=' + $('#groups').val() + '&city=' + $('#city').val() + '&source=' + $('#source').val() + '&payment=' + $('#payment').val() + '&days=' + $('#days').val());
        });

        $("#payment").change(function (e) {
            $('#navigate').attr('href', 'Default.aspx?sd=' + $('#startdate').val() + '&ed=' + $('#enddate').val() + '&pid=' + $('#projects').val() + '&g=' + $('#groups').val() + '&city=' + $('#city').val() + '&source=' + $('#source').val() + '&payment=' + $('#payment').val() + '&days=' + $('#days').val());
        });

        $("#days").change(function (e) {
            $('#navigate').attr('href', 'Default.aspx?sd=' + $('#startdate').val() + '&ed=' + $('#enddate').val() + '&pid=' + $('#projects').val() + '&g=' + $('#groups').val() + '&city=' + $('#city').val() + '&source=' + $('#source').val() + '&payment=' + $('#payment').val() + '&days=' + $('#days').val());
        });

        $("#startdate").datepicker({ dateFormat: "dd.mm.yy", onSelect: function (dateText, inst) { $('#navigate').attr('href', 'Default.aspx?sd=' + $('#startdate').val() + '&ed=' + $('#enddate').val() + '&pid=' + $('#projects').val() + '&g=' + $('#groups').val() + '&city=' + $('#city').val() + '&source=' + $('#source').val() + '&payment=' + $('#payment').val() + '&days=' + $('#days').val()); } });
        $("#enddate").datepicker({ dateFormat: "dd.mm.yy", onSelect: function (dateText, inst) { $('#navigate').attr('href', 'Default.aspx?sd=' + $('#startdate').val() + '&ed=' + $('#enddate').val() + '&pid=' + $('#projects').val() + '&g=' + $('#groups').val() + '&city=' + $('#city').val() + '&source=' + $('#source').val() + '&payment=' + $('#payment').val() + '&days=' + $('#days').val()); } });
        jQuery("#datagrid").jqGrid({
            url: 'ui/griddata.xml',
            datatype: "xml",
            colNames: [ 'Период', 'Заказы', 'Под. заказы', 'Отказы', 'Сумма заказов', 'Сумма под. заказов', 'Сумма отказов', 'Реал. сумма заказов', 'Сумма скидок', 'Клики', 'Показы', 'Расходы'],
            colModel: [
   		{ name: 'period', index: 'period', width: 90 },
   		{ name: 'total', index: 'total', width: 50, align: "right", summaryType: 'sum' },
   		{ name: 'totala', index: 'totala', width: 50, align: "right", summaryType: 'sum' },
   		{ name: 'totalr', index: 'totalr', width: 50, align: "right", summaryType: 'sum' },
   		{ name: 'totalsum', index: 'totalsum', width: 80, align: "right", summaryType: 'sum' },
   		{ name: 'totalsuma', index: 'totalsuma', width: 80, align: "right", summaryType: 'sum' },
   		{ name: 'totalsumr', index: 'totalsumr', width: 80, align: "right", summaryType: 'sum' },
   		{ name: 'totalsumd', index: 'totalsumd', width: 80, align: "right", summaryType: 'sum' },
   		{ name: 'totalsumdd', index: 'totalsumdd', width: 80, align: "right", summaryType: 'sum' },
   		{ name: 'clicks', index: 'clicks', width: 80, align: "right", summaryType: 'sum' },
   		{ name: 'shows', index: 'shows', width: 80, align: "right", summaryType: 'sum' },
   		{ name: 'expenses', index: 'expenses', width: 80, align: "right", summaryType: 'sum' }
   	],
            rowNum: 5000,
            height: 400,
            autowidth: true,
            rowList: [10, 20, 30],
            sortname: 'period',
            viewrecords: true,
            sortorder: "desc",
            caption: "Report",
            grouping: false,
            groupingView: {
                groupField: ['report'],
                groupSummary: [true],
                groupColumnShow: [false],
                groupText: [],
                groupCollapse: false,
                groupOrder: ['asc']
            }

        }).navGrid('#datagridpager', { edit: false, add: false, del: false });

        $.getScript("ui/ui.js");
    });				
</script> 


