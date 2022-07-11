<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="WebApp.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    
    <h3>Users</h3>
    <table>
        <tr>
            <th>Username</th>
            <th>Password</th>
            <th>Role</th>
        </tr>
        <tr>
            <td>AlexN</td>
            <td>Pa$$w0rd</td>
            <td>Director</td>
        </tr>
        <tr>
            <td>KarenY</td>
            <td>Pa$$w0rd</td>
            <td>Office Manager</td>
        </tr>
        <tr>
            <td>MartaT</td>
            <td>Pa$$w0rd</td>
            <td>Race Coordinator</td>
        </tr>
        <tr>
            <td>MarlaK</td>
            <td>Pa$$w0rd</td>
            <td>Food Service</td>
        </tr>
    </table>
    

    <h3>Connection String Details</h3>
    <div>
        name=\"DefaultConnection" connectionString="Data Source=.;Initial Catalog=eRace;Integrated Security=True" providerName="System.Data.SqlClient"
    </div>
    <div>
        name="ERaceDB" connectionString="Data Source=.;Initial Catalog=eRace;Integrated Security=True" providerName="System.Data.SqlClient"
    </div>
    
</asp:Content>
