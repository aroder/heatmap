<%@ Page Title="" Language="C#" MasterPageFile="~/One.Master" AutoEventWireup="true"
    CodeBehind="HeatMapUnitTests.aspx.cs" Inherits="CenStatsHeatMap.UI.HeatMapUnitTests1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        table td
        {
            vertical-align: top;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table border="1" cellpadding="8">
        <tr>
            <td>
                <div>
                    Exception Smiley</div>
                <img height="100" width="100" src="/images/HeatMapGenerator.ashx" />
            </td>
            <td>
                <div>
                    100x100</div>
                <img src="/images/HeatMapGenerator.ashx?size=100x100&data=50x50" />
            </td>
            <td>
                <div>
                    50x50</div>
                <img src="/images/HeatMapGenerator.ashx?size=50x50&data=25x25" />
            </td>
        </tr>
        <tr>
            <td>
                <img src="/images/HeatMapGenerator.ashx?data=0x0-1,20x20-2,40x40,60x60-3,80x80,100x100" />
            </td>
            <td>
                <div>100x100, window 25x25</div>
                <img src="/images/HeatMapGenerator.ashx?data=50x50&size=100x100&w=25x25&wp=25x25" />
            </td>
        </tr>
    </table>
</asp:Content>
