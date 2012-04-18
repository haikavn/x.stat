<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="fitmedia._Default" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" media="screen" href="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.16/themes/redmond/jquery-ui.css" />
    
    <!-- jQuery runtime minified -->   
    <script src="/js/jquery-1.7.1.min.js" type="text/javascript"></script>   

    <!-- The localization file we need, English in this case -->   
    <script src="/js/trirand/i18n/grid.locale-en.js" type="text/javascript"></script>   
      
    <!-- The jqGrid client-side javascript -->    
    <script src="/js/trirand/jquery.jqGrid.min.js" type="text/javascript"></script>   
    <script type="text/javascript" src="/js/highcharts/highcharts.js"></script>
    <script type="text/javascript" src="/js/highcharts/modules/exporting.js"></script>
    
    <link rel="stylesheet" href="/js/jqwidgets/jqwidgets/styles/jqx.base.css" type="text/css" />    
    <script type="text/javascript" src="/js/jqwidgets/jqwidgets/jqxcore.js"></script>    
    <script type="text/javascript" src="/js/jqwidgets/jqwidgets/jqxdata.js"></script>     
    <script type="text/javascript" src="/js/jqwidgets/jqwidgets/jqxbuttons.js"></script>    
    <script type="text/javascript" src="/js/jqwidgets/jqwidgets/jqxscrollbar.js"></script>    
    <script type="text/javascript" src="/js/jqwidgets/jqwidgets/jqxmenu.js"></script>    
    <script type="text/javascript" src="/js/jqwidgets/jqwidgets/jqxgrid.js"></script>    
    <script type="text/javascript" src="/js/jqwidgets/jqwidgets/jqxgrid.selection.js"></script>    
    <script type="text/javascript" src="/js/jqwidgets/scripts/gettheme.js"></script>   
    <script type="text/javascript" src="/js/jqwidgets/jqwidgets/jqxdatetimeinput.js"></script>
    <script type="text/javascript" src="/js/jqwidgets/jqwidgets/jqxcalendar.js"></script>
    <script type="text/javascript" src="/js/jqwidgets/jqwidgets/jqxtooltip.js"></script>
    <script type="text/javascript" src="/js/jqwidgets/jqwidgets/globalization/jquery.global.js"></script> 
    <script type="text/javascript" src="/js/jqwidgets/jqwidgets/jqxtree.js"></script>        
    <script type="text/javascript" src="/js/jqwidgets/jqwidgets/jqxcheckbox.js"></script>        
    <script type="text/javascript" src="/js/jqwidgets/jqwidgets/jqxbuttons.js"></script>

    <script type="text/javascript">
        var isfirstload = true;
        var pItem = null;
        var cItem = null;
        var cItemCount = 0;
        var cCurIndex = 0;
        var rCount = 0;
  
    </script>

    <script language=javascript>
        function myPostBack() {
            var res = $.get("datagen.aspx?sd=01.01.2012", function () {
                loadData();
            });            
           // data = new Array();
           // var source = {localdata: data, datatype: "array"};
           // $("#jqxgrid").jqxGrid({ source: source });
           // $('#jqxgrid').jqxGrid('updatebounddata');
            
            //$('#jqxgrid').jqxGrid('refreshdata');

            //var o = window.event.srcElement;
            //if (o.tagName == "INPUT" && o.type == "checkbox") {
                //__doPostBack("", "");
            //}
        }
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <div id="uijs">
    <script type="text/javascript" src="ui/ui.js" ></script>  
    </div>
    <div id="jqxTree" style="float: left;">
    <ul></ul>
    </div>
    <div id="startdate" style="float: left;"></div>
    <div id="enddate" style="float: left;"></div>
                    <div id="jqxgrid"></div>
                    <div>
                    <input type="button" value="Button" id='jqxButton' />
                    </div>
    <div style="float:left;">
        <div style="vertical-align: top">
        <asp:Table runat="server">
        <asp:TableRow runat="server">
            <asp:TableCell runat="server">
            </asp:TableCell>
            <asp:TableCell runat="server">
    
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
        <asp:TableCell VerticalAlign="Top">
            <asp:TreeView ID="tvCampaigns" runat="server" ShowCheckBoxes="Root,Parent,Leaf" OnTreeNodeCheckChanged="tvCampaigns_CheckChanged" OnClick="myPostBack();">    </asp:TreeView><br/>
        </asp:TableCell>
        <asp:TableCell VerticalAlign="Top">
                <asp:Table ID="Table1" runat="server">
        <asp:TableRow ID="TableRow1" runat="server">
        <asp:TableCell ID="TableCell1" runat="server">
            <asp:Label ID="Label1" runat="server" Text="Начало"></asp:Label>
            <asp:Calendar ID="Calendar1" runat="server" OnSelectionChanged="Startdate_SelChange"></asp:Calendar><br />

        </asp:TableCell>
        <asp:TableCell ID="TableCell2" runat="server">
            <asp:Label ID="Label2" runat="server" Text="Конец"></asp:Label>
            <asp:Calendar ID="Calendar2" runat="server" OnSelectionChanged="Enddate_SelChange"></asp:Calendar><br />

        </asp:TableCell>
        </asp:TableRow>
        </asp:Table><br/>
        
            <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="False" AllowPaging="true" PageSize="10" OnPageIndexChanging="gvData_PageIndexChanging" ShowFooter="True" OnRowDataBound="gvData_RowDataBound" Width="100%">
            <Columns>
                <asp:BoundField HeaderText="Период" DataField="period" HeaderStyle-Width="10%"></asp:BoundField>
                <asp:BoundField HeaderText="Заказы" DataField="total" HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Right"></asp:BoundField>
                <asp:BoundField HeaderText="Под. заказы" DataField="totala" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="5%"></asp:BoundField>
                <asp:BoundField HeaderText="Отказы" DataField="totalr" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="5%"></asp:BoundField>
                <asp:BoundField HeaderText="Сумма заказов" DataField="totalsum" ItemStyle-HorizontalAlign="Right"  HeaderStyle-Width="7%"></asp:BoundField>
                <asp:BoundField HeaderText="Сумма под. заказов" DataField="totalsuma" ItemStyle-HorizontalAlign="Right"  HeaderStyle-Width="7%"></asp:BoundField>
                <asp:BoundField HeaderText="Сумма отказов" DataField="totalsumr" ItemStyle-HorizontalAlign="Right"  HeaderStyle-Width="7%"></asp:BoundField>
                <asp:BoundField HeaderText="Реал. сумма заказов" DataField="totalsumd" ItemStyle-HorizontalAlign="Right"  HeaderStyle-Width="7%"></asp:BoundField>
                <asp:BoundField HeaderText="Сумма скидок" DataField="totalsumdd" ItemStyle-HorizontalAlign="Right"  HeaderStyle-Width="7%"></asp:BoundField>
                <asp:BoundField HeaderText="Клики" DataField="clicks" ItemStyle-HorizontalAlign="Right"  HeaderStyle-Width="5%"></asp:BoundField>
                <asp:BoundField HeaderText="Показы" DataField="shows" ItemStyle-HorizontalAlign="Right"  HeaderStyle-Width="5%"></asp:BoundField>
                <asp:BoundField HeaderText="Расходы" DataField="expenses" ItemStyle-HorizontalAlign="Right"  HeaderStyle-Width="7%"></asp:BoundField>
            </Columns>

            <FooterStyle BackColor="#CCCC99" Font-Bold="True" HorizontalAlign="Right" Font-Size="10"></FooterStyle>
            <PagerStyle ForeColor="Black" HorizontalAlign="Center" 
                BackColor="#F7F7DE"></PagerStyle>
            <HeaderStyle ForeColor="White" Font-Bold="True" Font-Size="11" 
                BackColor="#6B696B"></HeaderStyle>
            <AlternatingRowStyle BackColor="White"></AlternatingRowStyle>
            <RowStyle BackColor="#F7F7DE" Font-Size="10"></RowStyle>

            </asp:GridView>
            </asp:TableCell>
        </asp:TableRow>
        </asp:Table>

        </div>

<div>
<div id="container" style="width: 100%; height: 400px; margin: 0 auto; float:left"></div>     
<div id="container1" style="width: 1000px; height: 400px; margin: 0 auto; float:left"></div>     
</div>
    </div>

  
    </form>
</body>
</html>
