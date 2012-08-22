<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="logview.aspx.cs" Inherits="fitmedia.logview" EnableViewState="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    
    <form id="form1" runat="server">
    <div>
        <asp:DropDownList ID="projects" runat="server"></asp:DropDownList>
        <asp:DropDownList ID="logtypes" runat="server">
        <asp:ListItem Text="All" Value="0"></asp:ListItem>
        <asp:ListItem Text="Errors" Value="1"></asp:ListItem>
        <asp:ListItem Text="Success" Value="2"></asp:ListItem>
        </asp:DropDownList>
        <asp:Button ID="refresh" runat="server" OnClick="refresh_Click" />
     <asp:Repeater ID="log" runat="server" EnableViewState="false">
        <headertemplate>
        <table border="1">
            <tr>
            <td><b>Date</b></td>
            <td><b>Error status</b></td>
            <td><b>Message</b></td>
            <td><b>Method</b></td>
            </tr>
        </headertemplate>
  
        <itemtemplate>
        <tr>
            <td> <%# Eval("dt") %> </td>
            <td> <%# Eval("iserror") %> </td>
            <td> <%# Eval("errormsg") %> </td>
            <td> <%# Eval("methodname") %> </td>
        </tr>
        </itemtemplate>
  
        <footertemplate>
        </table>
        </footertemplate>

    </asp:Repeater>
    </div>
    </form>
</body>
</html>
