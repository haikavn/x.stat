<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="datagrid.aspx.cs" Inherits="fitmedia.datagrid" %>

<%@ Register Assembly="Trirand.Web" TagPrefix="trirand" Namespace="Trirand.Web.UI.WebControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    
<link rel="stylesheet" type="text/css" media="screen" href="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.16/themes/redmond/jquery-ui.css" />
    
<link rel="stylesheet" type="text/css" media="screen" href="/js/trirand/themes/ui.jqgrid.css" />    
<!-- jQuery runtime minified -->   
<script src="/js/jquery-1.7.1.min.js" type="text/javascript"></script>   

<!-- The localization file we need, English in this case -->   
<script src="/js/trirand/i18n/grid.locale-en.js" type="text/javascript"></script>   
  
<!-- The jqGrid client-side javascript -->    
<script src="/js/trirand/jquery.jqGrid.min.js" type="text/javascript"></script>    

<!-- This jQuery UI reference is needed only for the demo (code tabs). jqGrid per se does not need it. -->    
</head>
<body>
    <form id="form1" runat="server">
    <div>    
       <trirand:JQGrid ID="JQGrid1" runat="server" Width="600px" Height="400px">           
           <Columns>                
               <trirand:JQGridColumn DataField="Account" />                
               <trirand:JQGridColumn DataField="Clicks" />                
               <trirand:JQGridColumn DataField="Cost" /> 
               <trirand:JQGridColumn DataField="CTR" /> 
               <trirand:JQGridColumn DataField="Impressions" /> 
               <trirand:JQGridColumn DataField="AvgCPC" /> 
               <trirand:JQGridColumn DataField="Conversions" />                                              
          </Columns>                       
           
           <ToolBarSettings ShowSearchButton="true" />           
           <PagerSettings NoRowsMessage="No data in grid." />  
             
            <AppearanceSettings ShowFooter="true" />              
       </trirand:JQGrid> 
       <br />
       <br />
       <br />
    
     <trirand:JQGrid ID="JQGrid2" runat="server" Width="600px" Height="400px">           
           <Columns>                
               <trirand:JQGridColumn DataField="Campaign" />                
               <trirand:JQGridColumn DataField="Clicks" />                
               <trirand:JQGridColumn DataField="Cost" /> 
               <trirand:JQGridColumn DataField="CTR" /> 
               <trirand:JQGridColumn DataField="Impressions" /> 
               <trirand:JQGridColumn DataField="AvgCPC" /> 
               <trirand:JQGridColumn DataField="Conversions" /> 
               <trirand:JQGridColumn DataField="AdNetwork" />                                                             
               <trirand:JQGridColumn DataField="Device" />                                              
               <trirand:JQGridColumn DataField="ClickType" />                                                                                                                         
          </Columns>                       
           
           <ToolBarSettings ShowSearchButton="true" />           
           <PagerSettings NoRowsMessage="No data in grid." />  
             
            <AppearanceSettings ShowFooter="true" />              
       </trirand:JQGrid> 
    
           <br />
       <br />
       <br />
    
     <trirand:JQGrid ID="JQGrid3" runat="server" Width="600px" Height="400px">           
           <Columns>                
               <trirand:JQGridColumn DataField="AdGroup" />                
               <trirand:JQGridColumn DataField="Clicks" />                
               <trirand:JQGridColumn DataField="Cost" /> 
               <trirand:JQGridColumn DataField="CTR" /> 
               <trirand:JQGridColumn DataField="Impressions" /> 
               <trirand:JQGridColumn DataField="AvgCPC" /> 
               <trirand:JQGridColumn DataField="AvgPosition" />                
               <trirand:JQGridColumn DataField="Conversions" /> 
               <trirand:JQGridColumn DataField="AdNetwork" />                                                                            
               <trirand:JQGridColumn DataField="Device" />                                              
               <trirand:JQGridColumn DataField="ClickType" />                                                                                                                         
          </Columns>                       
           
           <ToolBarSettings ShowSearchButton="true" />           
           <PagerSettings NoRowsMessage="No data in grid." />  
             
            <AppearanceSettings ShowFooter="true" />              
       </trirand:JQGrid> 
       
           <br />
       <br />
       <br />
    
     <trirand:JQGrid ID="JQGrid4" runat="server" Width="600px" Height="400px">           
           <Columns>                
               <trirand:JQGridColumn DataField="Ad" />                
               <trirand:JQGridColumn DataField="Clicks" />                
               <trirand:JQGridColumn DataField="Cost" /> 
               <trirand:JQGridColumn DataField="CTR" /> 
               <trirand:JQGridColumn DataField="Impressions" /> 
               <trirand:JQGridColumn DataField="AvgCPC" /> 
               <trirand:JQGridColumn DataField="AvgPosition" />                
               <trirand:JQGridColumn DataField="Conversions" /> 
                                                                       
          </Columns>                       
           
           <ToolBarSettings ShowSearchButton="true" />           
           <PagerSettings NoRowsMessage="No data in grid." />  
             
            <AppearanceSettings ShowFooter="true" />              
       </trirand:JQGrid> 
       
           <br />
       <br />
       <br />
    
     <trirand:JQGrid ID="JQGrid5" runat="server" Width="600px" Height="400px">           
           <Columns>                
               <trirand:JQGridColumn DataField="Keyword" />                
               <trirand:JQGridColumn DataField="Clicks" />                
               <trirand:JQGridColumn DataField="Cost" /> 
               <trirand:JQGridColumn DataField="CTR" /> 
               <trirand:JQGridColumn DataField="Impressions" /> 
               <trirand:JQGridColumn DataField="AvgCPC" /> 
               <trirand:JQGridColumn DataField="FirstPageCPC" />                
               <trirand:JQGridColumn DataField="AvgPosition" />                
               <trirand:JQGridColumn DataField="Conversions" /> 
                                                                       
          </Columns>                       
           
           <ToolBarSettings ShowSearchButton="true" />           
           <PagerSettings NoRowsMessage="No data in grid." />  
             
            <AppearanceSettings ShowFooter="true" />              
       </trirand:JQGrid> 
       
                  <br />
       <br />
       <br />
    
     <trirand:JQGrid ID="JQGrid6" runat="server" Width="600px" Height="400px">           
           <Columns>                
               <trirand:JQGridColumn DataField="QueryTerm" />                
               <trirand:JQGridColumn DataField="MatchType" />                               
               <trirand:JQGridColumn DataField="Clicks" />                
               <trirand:JQGridColumn DataField="Cost" /> 
               <trirand:JQGridColumn DataField="Impressions" /> 
                                        
          </Columns>                       
           
           <ToolBarSettings ShowSearchButton="true" />           
           <PagerSettings NoRowsMessage="No data in grid." />  
             
            <AppearanceSettings ShowFooter="true" />              
       </trirand:JQGrid>           
       
       
                         <br />
       <br />
       <br />
    
     <trirand:JQGrid ID="JQGrid7" runat="server" Width="600px" Height="400px">           
           <Columns>                
               <trirand:JQGridColumn DataField="Campaign" />                
               <trirand:JQGridColumn DataField="Country" />                               
               <trirand:JQGridColumn DataField="Region" />                               
               <trirand:JQGridColumn DataField="MetroArea" /> 
               <trirand:JQGridColumn DataField="City" />                                                                                                          
               <trirand:JQGridColumn DataField="Clicks" />                
               <trirand:JQGridColumn DataField="Cost" /> 
               <trirand:JQGridColumn DataField="Impressions" /> 
                                        
          </Columns>                       
           
           <ToolBarSettings ShowSearchButton="true" />           
           <PagerSettings NoRowsMessage="No data in grid." />  
             
            <AppearanceSettings ShowFooter="true" />              
       </trirand:JQGrid>      
    </div>
    </form>
</body>
</html>
