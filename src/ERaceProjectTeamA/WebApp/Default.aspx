<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApp._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <%--Insert group logo--%>
        <h1>eRace System</h1>
<%--        <p class="lead">**insert description here*</p>--%>
    </div>
    <h2>Team Members:</h2>
    <div class="row container flex-column">
        <h3>Eronielle Miranda</h3>
            <br />
        <ul>
            <li>Purchasing Subsystem</li>
            <li>Setup/Security Responsibilities
                <ul>
                    <li>Project Setup and Subsystem Setup</li>
                    <li>Markdown drafts</li>
                </ul>
            </li>
            <li>Known Bugs list
                <ul>
                    <li>Bad design of view models</li>
                    <li>Purchase order numbers are not being pass to purchase order items grid view</li>
                    <li>Security login not working</li>
                </ul>
            </li>
        </ul>
    </div>
    <div class="row container">
        <h3>Nathan Matt</h3>
            <br />
        <ul>
            <li>Sales Subsystem</li>
            <li>Setup/Security Responsibilities
                <ul>
                    <li>Security Setup</li>
                </ul>
            </li>
            <li>Known Bugs list
                <ul>

                </ul>
            </li>
        </ul>
    </div>
    <div class="row container">
        <h3>Shingai Zindi</h3>
            <br />
        <ul>
            <li>Racing Subsystem</li>
            <li>Setup/Security Responsibilities
                <ul>
                    <li>Setup - Reverse Engineering DB</li>
                </ul>
            </li>
            <li>Known Bugs list
                <ul>
                    <li>Current car appears at the top of list instead of select a car. Issue with populating serial number drop down list while managing car class drop down list.</li>
                    <li>Roster ListView resets after errors are encoutered during editing. Issue with populating serial number drop down list while managing car class drop down list.</li>
                    <li>Some Money value formatting issues. Will display but decimals may vary.</li>
                </ul>
            </li>
        </ul>
    </div>
    <div class="row container">
        <h3>Tim Salinas</h3>
            <br />
        <ul>
            <li>Receiving Subsystem</li>
            <li>Setup/Security Responsibilities
                <ul>

                </ul>
            </li>
            <li>Known Bugs list
                <ul>

                </ul>
            </li>
        </ul>
    </div>

</asp:Content>
