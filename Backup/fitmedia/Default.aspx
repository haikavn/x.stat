<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="fitmedia._Default" %>

<%@ Register Assembly="Trirand.Web" TagPrefix="trirand" Namespace="Trirand.Web.UI.WebControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" media="screen" href="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.16/themes/redmond/jquery-ui.css" />
    
    <link rel="stylesheet" type="text/css" media="screen" href="/js/trirand/themes/ui.jqgrid.css" />
    
    <!-- jQuery runtime minified -->   
    <script src="/js/jquery-1.7.1.min.js" type="text/javascript"></script>   

    <!-- The localization file we need, English in this case -->   
    <script src="/js/trirand/i18n/grid.locale-en.js" type="text/javascript"></script>   
      
    <!-- The jqGrid client-side javascript -->    
    <script src="/js/trirand/jquery.jqGrid.min.js" type="text/javascript"></script>   
    <script type="text/javascript" src="/js/highcharts/highcharts.js"></script>
    <script type="text/javascript" src="/js/highcharts/modules/exporting.js"></script>    
    
  
</head>
<body>
    <form id="form1" runat="server">
    <div style="float:left">
    <asp:TreeView ID="tvCampaigns" runat="server" ShowCheckBoxes="Leaf" OnTreeNodeCheckChanged="tvCampaigns_CheckChanged">
    </asp:TreeView>
    </div>
    <div style="float:left;">
        <div>
        <asp:Table runat="server">
        <asp:TableRow>
        <asp:TableCell VerticalAlign="Top">
        <asp:Label runat="server" Text="Начало"></asp:Label>
        <asp:Calendar ID="Calendar1" runat="server" OnSelectionChanged="Startdate_SelChange"></asp:Calendar><br />
                <asp:Label ID="Label1" runat="server" Text="Конец"></asp:Label>
        <asp:Calendar ID="Calendar2" runat="server" OnSelectionChanged="Enddate_SelChange"></asp:Calendar>
        </asp:TableCell>
        <asp:TableCell>
       <trirand:JQGrid ID="JQGrid1" runat="server" Width="1000px" Height="400px">           
           <Columns>      
               <trirand:JQGridColumn DataField="period" HeaderText="Период" />                      
               <trirand:JQGridColumn DataField="total" HeaderText="Заказы" Width="50" />                
               <trirand:JQGridColumn DataField="totala" HeaderText="Под. заказы" Width="70" />                
               <trirand:JQGridColumn DataField="totalr" HeaderText="Отказы" Width="50" /> 
               <trirand:JQGridColumn DataField="totalsum" HeaderText="Сумма заказов" Width="85"  /> 
               <trirand:JQGridColumn DataField="totalsuma" HeaderText="Сумма под. заказов" Width="120" /> 
               <trirand:JQGridColumn DataField="totalsumr" HeaderText="Сумма отказов" Width="85" /> 
               <trirand:JQGridColumn DataField="totalsumd" HeaderText="Реальная сумма заказов" Width="140" />     
               <trirand:JQGridColumn DataField="totalsumdd" HeaderText="Сумма скидок" Width="90" />             
               <trirand:JQGridColumn DataField="clicks" HeaderText="Клики" Width="50" />  
               <trirand:JQGridColumn DataField="shows" HeaderText="Показы" Width="50" />
               <trirand:JQGridColumn DataField="expenses" HeaderText="Расходы" Width="50" />                                                                          
          </Columns>                       
          <PagerSettings PageSize="12" />
          
           <ToolBarSettings ShowSearchButton="true" />           
           <PagerSettings NoRowsMessage="No data in grid." />  
             
            <AppearanceSettings ShowFooter="true" />              
       </trirand:JQGrid>
               </asp:TableCell>
        </asp:TableRow>
        </asp:Table>

            </div>

<div>
<div id="container" style="width: 800px; height: 400px; margin: 0 auto; float:left"></div>     
<div id="container1" style="width: 800px; height: 400px; margin: 0 auto; float:left"></div>     
</div>
    </div>

  
    </form>
</body>
</html>
