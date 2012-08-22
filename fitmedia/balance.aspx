<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="balance.aspx.cs" Inherits="fitmedia.balance" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <title></title>
    <script src="js/jquery-1.7.1.min.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="balance">
    
    </div>
    </form>
</body>
</html>
<script type="text/javascript">
    $(document).ready(function () {

        $.getScript("ui/balance_ui.js");
    });				
</script> 