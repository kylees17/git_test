Imports System.IO
Imports System.Data.SqlClient
Imports ServiceReference1
Imports System.Data
Imports Microsoft.Reporting.WebForms

Partial Class Verify
    Inherits System.Web.UI.Page

    ' Stores the connection type: "T" for Test, "L" for Live
    Dim connectiontype As String = System.Configuration.ConfigurationManager.AppSettings("ConnectionType")

    ' Proxy for the SOAP web service used for DB calls
    Dim webser As New ServiceReference1.WebServiceSoapClient

    ' Holds the current user's internal username
    Dim users As String = ""

    ' Used for pagination in the GridView
    Dim endpage As Integer

    ''' <summary>
    ''' Returns the connection string depending on the selected environment.
    ''' </summary>
    Private Function GetConnectionString() As String
        Dim constring As String = String.Empty

        If connectiontype = "T" Then
            ' Get connection string for Test environment
            constring = webser.GetConnectionString(System.Configuration.ConfigurationManager.ConnectionStrings("DataConnectionTest").ConnectionString)
        End If

        If connectiontype = "L" Then
            ' Get connection string for Live environment
            constring = webser.GetConnectionString(System.Configuration.ConfigurationManager.ConnectionStrings("DataConnectionLive").ConnectionString)
        End If
        Return constring
    End Function

    ''' <summary>
    ''' Runs each time the page loads.
    ''' Redirects to Login if no session exists.
    ''' Loads either verified or unverified job requests.
    ''' </summary>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            If Session("UserName") Is Nothing Then
                ' Redirect to login if user not logged in
                Response.Redirect("Login.aspx")
                Exit Sub
            End If

            ' Determine which job list to load
            If checkIfHasVerifyBy() = True Then
                Call searchJobVerifiedList()
            Else
                Call searchJobVerificationList()
            End If
        End If
    End Sub

    ''' <summary>
    ''' Loads all job requests waiting for verification by the current user.
    ''' </summary>
    Protected Sub searchJobVerificationList()
        Dim con = GetConnectionString()

        Using mainform1 As New MainForm
            ' Get internal user name
            users = mainform1.getUsername(Session("UserName"))

            ' Compose SQL parts
            Dim table = "JRHeader A inner join JRStatus B On A.ControlNo = B.ControlNo Inner join FSBranchDepartment C on A.BranchDept = C.BranchNo"
            Dim columns = "A.ControlNo, A.Subject, RTRIM(Upper(C.BranchName)) As BranchDept, A.RequestedBy, A.DateRequested"
            Dim conditions = "B.JRStatus = 'F' and a.VerifiedBy ='" & users & "'"

            ' Query the data via web service
            Using dt As DataTable = webser.GetSingleTableData(table, columns, conditions, "A.DateRequested Desc", con)
                If dt.Rows.Count = 0 Then lblNotice.Visible = True

                ' Bind results to the GridView
                gvVerify.DataSource = dt
                gvVerify.DataBind()

                ' Add styling to grid rows
                For i As Integer = 0 To gvVerify.Rows.Count - 1
                    If Trim(gvVerify.Rows(i).Cells(0).Text.ToString) = "" Then
                        gvVerify.Rows(i).CssClass = "gridHover1"
                    Else
                        gvVerify.Rows(i).CssClass = "gridHover"
                    End If
                Next
            End Using
        End Using
    End Sub

    ''' <summary>
    ''' Adds client-side event handlers to GridView rows to make them clickable.
    ''' </summary>
    Protected Sub gvVerify_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            ' Add double-click event to each row
            e.Row.Attributes("ondblclick") = Page.ClientScript.GetPostBackClientHyperlink(gvVerify, "Select$" & e.Row.RowIndex, False)
            e.Row.Attributes("style") = "cursor:pointer"
        End If
    End Sub

    ''' <summary>
    ''' When a GridView row is selected, display the job request details modal.
    ''' </summary>
    Protected Sub gvRequest_SelectedIndexChanged(sender As Object, e As EventArgs) Handles gvVerify.SelectedIndexChanged
        If Trim(gvVerify.SelectedRow.Cells(0).Text) <> "" Then
            lblDetails.Text = gvVerify.SelectedRow.Cells(0).Text + " - Details"
            Call jobRequestDetailsShow(gvVerify.SelectedRow.Cells(0).Text)

            ' Show details modal popup
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "modalscript1", _
                "$(function() { $('#div2').fadeIn(500); $('#backgroundpopup').css('opacity', '0.8').fadeIn(2); });", True)
        End If
    End Sub

    ''' <summary>
    ''' Loads details of the selected job request.
    ''' </summary>
    Protected Sub jobRequestDetailsShow(controlNo As String)
        Dim con = GetConnectionString()

        Dim table = "JRHeader A inner join FSBranchDepartment B on A.BranchDept = B.BranchNo inner join reference e on a.Request =e.Code"
        Dim columns = "RTRIM(Upper(B.BranchName)) As BranchDept, e.Description as Request, A.ControlNo, A.Subject, A.Details, A.RequestedBy, A.VerifiedBy, A.ApprovedBy, A.DateRequested, A.DateNeeded, A.isVerified"
        Dim conditions = "A.ControlNo = '" & controlNo & "' and e.type ='NatureOfRequest'"

        Using dt As DataTable = webser.GetSingleTableData(table, columns, conditions, "", con)
            If dt.Rows.Count > 0 Then
                ' Populate labels with data
                lblControlNo.Text = dt.Rows(0).Item("ControlNo").ToString
                lblPlaceConcern.Text = dt.Rows(0).Item("BranchDept").ToString
                lblNatureRequest.Text = dt.Rows(0).Item("Request").ToString
                lblRequestor.Text = dt.Rows(0).Item("RequestedBy").ToString
                lblDateRequsted.Text = dt.Rows(0).Item("DateRequested").ToString
                lblDateNeeded.Text = dt.Rows(0).Item("DateNeeded").ToString
                lblSubject.Text = dt.Rows(0).Item("Subject").ToString
                lblApprovedBy.Text = dt.Rows(0).Item("ApprovedBy").ToString
                txtDetails.Text = dt.Rows(0).Item("Details").ToString
                hdVerifiedBy.Value = dt.Rows(0).Item("VerifiedBy").ToString
                hdisVerified.Value = dt.Rows(0).Item("isVerified").ToString

                ' Hide or show VerifiedBy panel
                If dt.Rows(0).Item("isVerified").ToString = "0" OrElse String.IsNullOrWhiteSpace(dt.Rows(0).Item("VerifiedBy").ToString) Then
                    dvVerifiedBy.Visible = False
                Else
                    dvVerifiedBy.Visible = True
                    lblVerifiedBy.Text = dt.Rows(0).Item("VerifiedBy").ToString
                End If
            Else
                lblControlNo.Text = ""
                lblDetails.Text = "No record found."
            End If
        End Using
    End Sub

    ''' <summary>
    ''' Handles rejection of a job request.
    ''' Updates JRStatus and JRHeader tables.
    ''' </summary>
    Protected Sub btnReject_Click(sender As Object, e As EventArgs)
        Dim con = GetConnectionString()
        If webser.updateTable("UPDATE JRStatus SET JRStatus = 'B' WHERE ControlNo = '" & lblControlNo.Text & "'", con) = True Then
            If webser.updateTable("UPDATE JRHeader SET OfficerApproval='" & Session("UserName") & "' WHERE ControlNo = '" & lblControlNo.Text & "'", con) = True Then
                ScriptManager.RegisterStartupScript(Me, [GetType](), "displayalertmessage", "DisapprovalSuccess();", True)
            Else
                ScriptManager.RegisterStartupScript(Me, [GetType](), "displayalertmessage", "Error();", True)
            End If
        Else
            ScriptManager.RegisterStartupScript(Me, [GetType](), "displayalertmessage", "Error();", True)
        End If
    End Sub

    ''' <summary>
    ''' Handles clicking the Approve button.
    ''' Shows modal if verification data missing.
    ''' Otherwise updates isVerified flag.
    ''' </summary>
    Protected Sub btnApprove_Click(sender As Object, e As EventArgs)
        If hdVerifiedBy.Value = "" Then
            ' Load dropdown and show modal
            Call loadDropDown()

            ' Register client-side script using correct ClientIDs
            Dim script As String = String.Format(
                "$(function() {{ $('#{0}').fadeIn(500); $('#{1}').fadeIn(2).css('opacity','0.8'); }});",
                dvDesignation.ClientID,
                backgroundpopup.ClientID
            )
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "modalscript1", script, True)

            ' Debug message for troubleshooting
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "debugmsg", "alert('Showing modal because hdVerifiedBy is empty');", True)

            Exit Sub
        Else
            If hdisVerified.Value = "0" Then
                Dim con = GetConnectionString()
                If webser.updateTable("UPDATE JRHeader SET isVerified = '1' WHERE ControlNo = '" & lblControlNo.Text & "'", con) = True Then
                    ScriptManager.RegisterStartupScript(Me, [GetType](), "displayalertmessage", "ApprovalSuccess();", True)
                Else
                    ScriptManager.RegisterStartupScript(Me, [GetType](), "displayalertmessage", "Error();", True)
                End If
            Else
                Call loadDropDown()

                Dim script As String = String.Format(
                    "$(function() {{ $('#{0}').fadeIn(500); $('#{1}').fadeIn(2).css('opacity','0.8'); }});",
                    dvDesignation.ClientID,
                    backgroundpopup.ClientID
                )
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "modalscript1", script, True)

                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "debugmsg2", "alert('Showing modal because hdisVerified is not 0');", True)

                Exit Sub
            End If
        End If
    End Sub

    ''' <summary>
    ''' Refreshes the grid when the dummy button is clicked.
    ''' </summary>
    Protected Sub btnDummy_Click(sender As Object, e As EventArgs) Handles btnDummy.Click
        Call searchJobVerificationList()
    End Sub

    ''' <summary>
    ''' Handles GridView pagination.
    ''' </summary>
    Protected Sub gvVerify_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles gvVerify.PageIndexChanging
        gvVerify.PageIndex = e.NewPageIndex
        endpage = gvVerify.PageCount
        Call searchJobVerificationList()
    End Sub

    ''' <summary>
    ''' Handles clicking Save on the modal.
    ''' Updates JRStatus and JRHeader.
    ''' </summary>
    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        Dim priority = ddlPriority.SelectedValue
        Dim status = "V"
        Dim con = GetConnectionString()

        If webser.updateTable("UPDATE JRStatus SET JRStatus = '" & status & "', PriorityLevel='" & priority & "' WHERE ControlNo = '" & lblControlNo.Text & "'", con) = True Then
            If webser.updateTable("UPDATE JRHeader SET OfficerApproval='" & Session("UserName") & "' WHERE ControlNo = '" & lblControlNo.Text & "'", con) = True Then
                ScriptManager.RegisterStartupScript(Me, [GetType](), "displayalertmessage", "ApprovalSuccess();", True)
            Else
                ScriptManager.RegisterStartupScript(Me, [GetType](), "displayalertmessage", "Error();", True)
            End If
        Else
            ScriptManager.RegisterStartupScript(Me, [GetType](), "displayalertmessage", "Error();", True)
        End If
    End Sub

    ''' <summary>
    ''' Populates the dropdown list for Priority selection in the modal.
    ''' </summary>
    Protected Sub loadDropDown()
        If ddlPriority.Items.Count = 0 Then
            ddlPriority.Items.Insert(0, New ListItem("High", 1))
            ddlPriority.Items.Insert(1, New ListItem("Normal", 2))
            ddlPriority.Items.Insert(2, New ListItem("Low", 3))
        End If
        ddlPriority.SelectedIndex = 1
    End Sub

    ''' <summary>
    ''' Generates a PDF report for the job request and opens it in a new window.
    ''' </summary>
    Protected Sub btnprint_Click(sender As Object, e As EventArgs)
        Call generateReport()

        Dim warnings As Warning() = Nothing
        Dim streamids As String() = Nothing
        Dim mimeType As String = Nothing
        Dim encoding As String = Nothing
        Dim extension As String = Nothing
        Dim bytes As Byte()

        Dim FolderLocation As String = Server.MapPath("~/temp/")
        Dim timestamp As String = DateTime.Now.ToString("yyyyMMddHHmmss")
        Dim filepath As String = FolderLocation & "JR-" & timestamp & ".pdf"

        If Not Directory.Exists(FolderLocation) Then
            Directory.CreateDirectory(FolderLocation)
        End If

        ' Render the RDLC report as bytes
        bytes = ReportViewer1.LocalReport.Render("PDF", Nothing, mimeType, encoding, extension, streamids, warnings)

        ' Save PDF to disk
        Using fs As New FileStream(filepath, FileMode.Create)
            fs.Write(bytes, 0, bytes.Length)
        End Using

        ' Open PDF in a new window
        Dim TransferPage As String
        TransferPage = "window.open('temp/JR-" & timestamp & ".PDF','popup_window','fullscreen=yes,resizable=yes');"
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "open", TransferPage, True)

        Call searchJobVerificationList()
    End Sub

    ''' <summary>
    ''' Fills parameters for the RDLC report.
    ''' </summary>
    Protected Sub generateReport()
        ReportViewer1.LocalReport.Refresh()
        ReportViewer1.ProcessingMode = ProcessingMode.Local
        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/lib/JobRequestForm.rdlc")

        Dim parameters As New List(Of ReportParameter) From {
            New ReportParameter("ControlNo", lblControlNo.Text),
            New ReportParameter("Branch", lblPlaceConcern.Text),
            New ReportParameter("DateRequested", lblDateRequsted.Text),
            New ReportParameter("Subject", lblSubject.Text),
            New ReportParameter("NatureOfRequest", lblNatureRequest.Text),
            New ReportParameter("RequestorName", lblRequestor.Text),
            New ReportParameter("RequestorPosition", ""),
            New ReportParameter("VerifiedName", lblVerifiedBy.Text),
            New ReportParameter("VerifiedPosition", "position tba"),
            New ReportParameter("ApprovalName", lblApprovedBy.Text),
            New ReportParameter("ApprovalPosition", "position tba"),
            New ReportParameter("Description", txtDetails.Text),
            New ReportParameter("DateNeeded", lblDateNeeded.Text)
        }

        ReportViewer1.LocalReport.SetParameters(parameters)
        ReportViewer1.LocalReport.Refresh()
    End Sub

    ''' <summary>
    ''' Loads job requests where the current user is the verifier.
    ''' </summary>
    Protected Sub searchJobVerifiedList()
        Dim verifiedby = ""
        Dim con = GetConnectionString()

        ' Determine who the branch head verifier is
        Using dt As DataTable = webser.GetTableDataDT("exec spJRGetVerificationUser '" & Session("UserName") & "','" & Session("branchno") & "'", con)
            If dt.Rows.Count <> 0 Then verifiedby = Trim(dt.Rows(0).Item("BranchHead").ToString)
        End Using

        Dim table = "JRHeader A inner join JRStatus B On A.ControlNo = B.ControlNo Inner join FSBranchDepartment C on A.BranchDept = C.BranchNo"
        Dim columns = "A.ControlNo, A.Subject, RTRIM(Upper(C.BranchName)) As BranchDept, A.RequestedBy, A.DateRequested"
        Dim conditions = "B.JRStatus = 'F' and a.ApprovedBy ='" & verifiedby & "' and (a.verifiedBy='' or a.isVerified=1)"

        Using dt As DataTable = webser.GetSingleTableData(table, columns, conditions, "A.DateRequested Desc", con)
            If dt.Rows.Count = 0 Then lblNotice.Visible = True

            gvVerify.DataSource = dt
            gvVerify.DataBind()

            For i As Integer = 0 To gvVerify.Rows.Count - 1
                If Trim(gvVerify.Rows(i).Cells(0).Text.ToString) = "" Then
                    gvVerify.Rows(i).CssClass = "gridHover1"
                Else
                    gvVerify.Rows(i).CssClass = "gridHover"
                End If
            Next
        End Using
    End Sub

    ''' <summary>
    ''' Checks whether the current user has jobs to verify.
    ''' </summary>
    Protected Function checkIfHasVerifyBy() As Boolean
        Dim verifiedby = ""
        Dim con = GetConnectionString()

        Using dt As DataTable = webser.GetTableDataDT("exec spJRGetVerificationUser '" & Session("UserName") & "','" & Session("branchno") & "'", con)
            If dt.Rows.Count <> 0 Then
                verifiedby = Trim(dt.Rows(0).Item("BranchHead").ToString)
            End If
        End Using

        Dim table = "JRHeader A inner join JRStatus B On A.ControlNo = B.ControlNo Inner join FSBranchDepartment C on A.BranchDept = C.BranchNo"
        Dim columns = "A.ControlNo, A.Subject, RTRIM(Upper(C.BranchName)) As BranchDept, A.RequestedBy, A.DateRequested"
        Dim conditions = "B.JRStatus = 'F' and a.ApprovedBy='" & verifiedby & "' and (a.verifiedBy='' or a.isVerified=1)"

        Using dt As DataTable = webser.GetSingleTableData(table, columns, conditions, "A.DateRequested Desc", con)
            If dt.Rows.Count <> 0 Then Return True
        End Using
        Return False
    End Function

End Class
