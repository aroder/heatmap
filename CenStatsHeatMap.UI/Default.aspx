<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CenStatsHeatMap.UI._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="content-type" content="text/html; charset=utf-8" />
    <title>CenStats HeatMap</title>

</head>
<body>
    <form id="form1" runat="server">
        <table border=1 cellpadding=10>
            <tr>
                <td><asp:Button id="Button1" runat="server" OnClick="Button1_Click" Text="Encode/Decode" /></td>
                <td>Encode: <asp:TextBox id="ToEncodeTextBox" runat="server"></asp:TextBox></td>
                <td>Decode: <asp:TextBox id="ToDecodeTextBox" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Google Extended</td>
                <td><asp:Label id="GoogleExtendedEncodedLabel" runat="server" /></td>
                <td><asp:Label id="GoogleExtendedDecodedLabel" runat="server" /></td>
            </tr>
            <tr>
                <td>My Extended</td>
                <td><asp:Label id="MyExtendedEncodedLabel" runat="server" /></td>
                <td><asp:Label id="MyExtendedDecodedLabel" runat="server" /></td>
            </tr>
        </table>
    </form>
</body>
</html>
