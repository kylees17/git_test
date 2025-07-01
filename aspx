<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MainForm.Master" CodeBehind="Verify.aspx.vb" Inherits="JRequest.Verify" %>

    <%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <link href="css/loan.css" rel="stylesheet" type="text/css" media="all" />
 <script src="js/pagination.js" type="text/javascript"></script>

    <style>
         .gvPagerCss span {
             font-weight: bold;
         }

         .gridHover:hover {
             cursor: pointer;
             background-color: #aebfd0;
         }

         .gridHover1:hover {
             cursor: default;
             background-color: transparent;
         }

            .Gvheader {text-align:center;}
            .paddingRight {padding-right: 15px;}
            .paddingLeft {padding-left: 10px;}

                 #dvDesignation {
                    width: 345px;
                    font-family: "lucida grande",tahoma,verdana,arial,sans-serif;
                    background: none repeat scroll 0 0 #FFFFFF;
                    border: 3px solid #2c66b6;
                    border-radius: 3px 3px 3px 3px;
                    color: #333333;
                    display: none;
                    font-size: 14px;
                    top: 25%;
                    left: 58%;
                    margin-left: -202px;
                    position: fixed;
                    z-index: 2;
                    height: 190px;
                 }      

                  .smallDetail {visibility:hidden;}

                 .resultDetail {width: 30%;}
     
                 .cssddlApproved { position:absolute ;width: 140px;}

                  .divTodo {padding-bottom: 20px;width: 100%;text-align: right;padding-right: 140px;}   
                  
                  @media (max-width:1260px){
                    #div2 {
                        max-width: 950px;
                        width: 70%;
                        left: 465px;
                    }
                }
                       #lblNotice {
                             width: 100%;
                             display: block;
                             text-align: center;
                             font-size: 18px;
                         }

                @media (max-width: 992px) {
                    #div2 {
                        width: 100%;
                        left: 0px;
                        margin-left: 20px;
                        max-width: 95.5%;
                        overflow-y:auto 
                    }
                    .resultDetail {width: 25%;}
                }

                @media (max-width: 767px) {
                    .smallDetail {visibility: collapse;}
                    .detailContent {width: 30%;}
                    .resultDetail {width:50%;}
                    .requestwidth {margin-left: 30px;width: 30%;}
                    .smallDivflex1 {display:block}
                    .detailContent.smallDetail { width: 20%;}
                    .divTodo {
                         padding-bottom: 20px;
                         width: 100%;
                         text-align: center;
                         padding-right: 0px;
                     }
                }
                 #div2 {
                    max-width: 950px;
                    width: 70%;
                  
                 }   
    </style>




     <script>
         function ApprovalSuccess() {
             Swal.fire({
                 position: 'center',
                 icon: 'info',
                 title: 'Requested Job has been Verified',
                 showConfirmButton: true,
                 confirmButtonText: 'Yes',
                 customClass: 'swal-size-sm'
             }).then((result) => {
                 if (result.isConfirmed) {
                     printPop();
                     document.getElementById("btnDummy").click();
                 }
             });
         }

         function DisapprovalSuccess() {
             Swal.fire({
                 position: 'center',
                 icon: 'info',
                 title: 'Requested Action is Done',
                 showConfirmButton: true,
                 confirmButtonText: 'Yes',
                 customClass: 'swal-size-sm'
             }).then((result) => {
                 if (result.isConfirmed) {
                     document.getElementById("btnDummy").click();
                 }
             });
         }

         function Error() {
             Swal.fire({
                 position: 'center',
                 icon: 'info',
                 title: 'Something went Wrong',
                 text: 'Please Try Again Later',
                 showConfirmButton: true,
                 confirmButtonText: 'Yes',
                 customClass: 'swal-size-sm'
             }).then((result) => {
                 if (result.isConfirmed) {
                     document.getElementById("btnDummy").click();
                 }
             });
         }

         function printPop(){
             Swal.fire({
                 title: 'Do you want to print?',
                 showDenyButton: true,
                 confirmButtonText: 'Yes',
                 denyButtonText: 'No'
             }).then((result) => {
                 if (result.isConfirmed) {
                     document.getElementById("btnprint").click();
                     //document.getElementById("btnDummy").click();
                 } else if (result.isDenied) {
                     document.getElementById("btnDummy").click();
                 }
                 })
         }


 </script>

    <div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" >
        <ContentTemplate>
            <asp:GridView ID="gvVerify" runat="server" CellPadding="0" GridLines="none"
                AutoGenerateColumns="false" BorderColor="White" CellSpacing="0" BackColor="Transparent" BorderWidth="1px"
                CssClass="responsiveDepositAccnts" RowStyle-Height="35" AllowPaging="true" PageSize="10"
                OnRowDataBound="gvVerify_RowDataBound" Width="100%">
                <HeaderStyle BackColor="Transparent" BorderStyle="Solid" BorderWidth="1px" BorderColor="#CCCCCC" Height="35" />
                <Columns>                        
                    <asp:BoundField HeaderStyle-CssClass="Gvheader" HeaderText="Control No" NullDisplayText=" "
                        ItemStyle-HorizontalAlign="Center" ItemStyle-Width="10%" DataField="ControlNo" />
                    <asp:BoundField HeaderStyle-CssClass="Gvheader" NullDisplayText=" " HeaderText="Subject"
                        ItemStyle-HorizontalAlign="left" ItemStyle-CssClass="marginLeft" ItemStyle-Width="35%" DataField="Subject" />
                    <asp:BoundField HeaderStyle-CssClass="Gvheader" NullDisplayText=" " HeaderText="Branch / Department" 
                        ItemStyle-HorizontalAlign="Center" ItemStyle-CssClass="marginRight" ItemStyle-Width="15%" DataField="BranchDept" />
                    <asp:BoundField HeaderStyle-CssClass="Gvheader" NullDisplayText=" " HeaderText="Requested By" 
                        ItemStyle-HorizontalAlign="Center" ItemStyle-CssClass="marginRight" ItemStyle-Width="15%" DataField="RequestedBy" />
                    <asp:BoundField HeaderStyle-CssClass="Gvheader" NullDisplayText=" " HeaderText="Date Requested"
                        ItemStyle-HorizontalAlign="Center" ItemStyle-CssClass="marginRight"  ItemStyle-Width="10%" DataField="DateRequested" DataFormatString="{0:MM/dd/yyyy}" />
                    <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <asp:Button ID="btnSelect" runat="server"
                            CommandName="SelectRow"
                            CommandArgument='<%# Eval("ControlNo") %>'
                            Text="🔍 Verify"
                            CssClass="btn btn-sm btn-primary" />
                    </ItemTemplate>
                </asp:TemplateField>
                </Columns>
                <%--<RowStyle CssClass="gridHover" />--%>
                <PagerStyle CssClass="gvPagerCss bs-pagination" BackColor="#2c66b6" ForeColor="White" HorizontalAlign="Center" />
                <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" Position="Bottom" PageButtonCount="5" />
            </asp:GridView>  
            
            <asp:label runat="server" ID="lblNotice" Visible="false" ClientIDMode="Static"  Text="Nothing to Display"/>              
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
      <!--hidden fields-->
 <asp:button ID="btnDummy" style="visibility:hidden" runat="server" OnClick="btnDummy_Click" ClientIDMode="Static" ></asp:button>
 <%--<asp:button ID="btnprint" style="display:none " runat="server" OnClick="btnprint_Click" ClientIDMode="Static" ></asp:button>--%>
 <asp:hiddenfield runat="server" ID="hdVerifiedBy" />
 <asp:hiddenfield runat="server" ID="hdisVerified" />

 <div style="display: none">
     <rsweb:ReportViewer ID="ReportViewer1" runat="server">
     </rsweb:ReportViewer>
 </div>

 <!-- inquiry pop up -->
 <div id="div2" style="height: 500px;"><%--height: auto; --%>
     <asp:Button ID="Button2" runat="server" Text="Button" Style="display: none" UseSubmitBehavior="false" />
     <div style="width: 100%; height: 50px; background-color: #2c66b6; margin: 0 auto; padding: 0; position: absolute; left: 0; top: 0;">
         <div class="modalHeader" style="font-size: 18px;display: flex;height: 50px;align-items: center;">
             <i class="fa fa-question-circle" style="margin-left: 15px;"></i>
             <asp:label runat="server" ID="lblDetails" style="margin-left: 15px;"/>
             <i class="icon-remove-sign" style="cursor: pointer;right: 15px;position: absolute;" onclick="jQuery('#backgroundpopup').fadeOut('normal');jQuery('#div2').fadeOut('normal');"></i>
         </div>
     </div>

     <asp:UpdatePanel ID="UpdatePanel4" runat="server" style="margin-top: 15px; padding-top: 5px">
         <ContentTemplate>
             <div>
                 <div style="margin-top:30px" class="personal">
                     <div class="smallDivflex">
                         <asp:Label runat="server" class="myfonts detailContent">Branch / Dept Concern :</asp:Label>
                         <asp:Label ID="lblPlaceConcern" CssClass="result myfonts resultDetail" runat="server" />
                     </div>

                     <div class="smallDivflex">
                         <asp:Label runat="server" class="myfonts requestwidth " Style="">Requested By :</asp:Label>
                         <asp:Label ID="lblRequestor" CssClass="result myfonts" Style="" runat="server" />
                     </div>
                 </div>
             </div>

             <div>
                 <div style="" class="personal">
                     <div class="smallDivflex">
                         <asp:Label runat="server" class="myfonts detailContent">Nature of Request :</asp:Label>
                         <asp:Label ID="lblNatureRequest" CssClass="result myfonts resultDetail" Text="" runat="server" />
                     </div>

                     <div class="smallDivflex">
                         <asp:Label  runat="server" class="myfonts requestwidth">Control No :</asp:Label>
                         <asp:Label ID="lblControlNo" CssClass="result myfonts" Text="" runat="server" />
                     </div>
                     
                 </div>
             </div>

             <div>
                 <div style="" class="personal">
                     <div class="smallDivflex">
                         <asp:Label runat="server" class="myfonts detailContent">Date Requested :</asp:Label>
                         <asp:Label ID="lblDateRequsted" CssClass="result myfonts resultDetail" Style="" runat="server" />
                     </div>

                    <%-- <div class="smallDivflex">
                        <asp:Label runat="server" class="myfonts requestwidth">Date Needed :</asp:Label>
                        <asp:Label ID="lblDateNeeded" CssClass="result myfonts resultDetail" Style="" runat="server" />
                     </div>--%>
                 </div>
             </div>

             <div style="display:none">
                 <div style="" class="personal">
                     <div class="smallDivflex">
                         <asp:Label runat="server" class="myfonts detailContent">Approved By :</asp:Label>
                         <asp:Label ID="lblApprovedBy" CssClass="result myfonts resultDetail" Style="" runat="server" />
                     </div>

                  
                 </div>
             </div>

             <div>
                 <div style="padding-top: 15px;" class="personal">
                     <div class="smallDivflex">
                        <asp:Label runat="server" class="myfonts detailContent">Subject :</asp:Label>
                        <asp:Label ID="lblSubject" CssClass="result myfonts resultDetail" Style="" runat="server" />
                     </div>
                 </div>
             </div>

             <div>
                 <div style="padding-top: 15px;" class="Details">
                     <div class="smallDivflex1 smallDivflex" style="text-decoration: none;padding-bottom: 20px;">
                         <asp:Label runat="server" ClientIDMode="Static" class="myfonts detailContent">Details:</asp:Label>
                         <br />
                         <asp:Label runat="server" ClientIDMode="Static" class="myfonts detailContent" style="visibility:hidden;margin:0px 0px">Details:</asp:Label>
                         
                         <asp:TextBox runat="server" ClientIDMode="Static" ID="txtDetails" CssClass="result myfonts" style="margin-left: 5px;height: 140px;width: 60%;" TextMode="MultiLine" Enabled="false"></asp:TextBox>
                     </div>
                 </div>
             </div>

             <div runat="server" id="dvVerifiedBy" visible="true" style="width: auto;padding-top: 10px;" >
                 <asp:label runat="server" Text="Verified By:" CssClass ="myfonts detailContent" ClientIDMode="Static" />
                 <asp:label runat="server" ID="lblVerifiedBy" />
             </div>

             <br />
             <div style="" class="divTodo">
                 <asp:Button ID="btnReject" runat="server" Text="Disapprove" class="myfonts" Style="width: 100px;" OnClick="btnReject_Click"  />
                 <asp:Button ID="btnApprove" runat="server" Text="Approve" class="myfonts" Style="width: 100px;" OnClick="btnApprove_Click" />
             </div>
         </ContentTemplate>
     </asp:UpdatePanel>
 </div>
 <!-- end of inquiry -->

  <div id="dvDesignation" style=""><%--height: auto; --%>
     <asp:Button ID="Button1" runat="server" Text="Button" Style="display: none" UseSubmitBehavior="false" />
     <div style="width: 100%; height: 50px; background-color: #2c66b6; margin: 0 auto; padding: 0; position: absolute; left: 0; top: 0;">
         <div class="modalHeader" style="font-size: 18px;display: flex;height: 50px;align-items: center;">
             <i class="fa fa-question-circle" style="margin-left: 15px;"></i>
             <asp:label ID="Label4" runat="server" style="margin-left: 15px;" Text="Section"/>
             <i class="icon-remove-sign" style="cursor: pointer;right: 15px;position: absolute;" onclick="jQuery('#backgroundpopup').fadeOut('normal');jQuery('#dvDesignation').fadeOut('normal');"></i>
         </div>
     </div>

     <asp:UpdatePanel ID="UpdatePanel2" runat="server" style="margin-top: 15px; padding-top: 5px">
         <ContentTemplate>
             <div>
                 <br /><br />
                 <div style="margin-top: 25px;display: flex;" class="personal">
                     <div class="" style="display:flex">
                         <asp:Label runat="server" class="myfonts detailContent" style="width:170px;text-align: left; ">Priority Level :</asp:Label>

                         <div style="width: 30%; padding-left: 5px; height: 30px;">
                             <asp:DropDownList runat="server" CssClass="cssddlApproved" ID="ddlPriority" style="position: absolute;z-index: 9999;" />
                             <%--OnMouseDown="this.size=3;" OnFocusOut="this.size=1;" OnDblClick="this.size=1;" --%>
                         </div>
                     </div>
                 </div>
             </div>

             <br />
             <div style="width: 100%;text-align: center;" >
                 <asp:Button ID="btnSave" runat="server" Text="Done" class="myfonts" Style="width: 100px;" OnClick="btnSave_Click"  />
             </div>
         </ContentTemplate>
     </asp:UpdatePanel>
 </div>
</asp:Content>
