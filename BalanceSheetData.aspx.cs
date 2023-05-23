using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class FAMS_Master_BalanceSheetData : System.Web.UI.Page
{
    BAL_EmployeeLevelAccess EmployeeLevelBAL = new BAL_EmployeeLevelAccess();
    Message msg = new Message();
    BAL_FA_LedgerMapping Notes = new BAL_FA_LedgerMapping();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            fill_company();
            fill_Finance_year();
          
        }
    }


    protected void fill_company()
    {
        DDLCompanyName.Items.Clear();
        DataTable dtCompany = EmployeeLevelBAL.LoadEmployeeCompanyAccess(Session["LogIn_Code"].ToString());
        if (dtCompany.Rows.Count > 0)
        {
            DDLCompanyName.DataSource = dtCompany;
            DDLCompanyName.DataTextField = "Comp_Name";
            DDLCompanyName.DataValueField = "Comp_Code";
            DDLCompanyName.DataBind();
            DDLCompanyName.Items.Insert(0, new ListItem("Select Company Name", "0"));
        }
    }



    protected void fill_Finance_year()
    {
        DateTime dtm = new DateTime();
        dtm = Convert.ToDateTime(DateTime.Now);
        ArrayList Year = new ArrayList();
      
        Year.Add(Convert.ToString(dtm.Year - 3) + "-" + Convert.ToString(dtm.Year - 2));
        Year.Add(Convert.ToString(dtm.Year-2) + "-" + Convert.ToString(dtm.Year-1));
        Year.Add(Convert.ToString(dtm.Year-1) + "-" + Convert.ToString(dtm.Year));
        Year.Add(Convert.ToString(dtm.Year) + "-" + Convert.ToString(dtm.Year + 1));
        //Year.Add(Convert.ToString(dtm.Year + 1) + "-" + Convert.ToString(dtm.Year + 2));
  
        ddlFinanceYear.DataSource = Year;
        ddlFinanceYear.DataBind();
        ddlFinanceYear.Items.Insert(0, new ListItem("Select Finance Year", "0"));
       
    }


    protected void btnSave_Click1(object sender, EventArgs e)
    {
        DataTable dtjv = new DataTable();
        ErrorContainer.Visible = true;


        dtjv.Columns.Add("Comp_Code", typeof(string));
        dtjv.Columns.Add("Note", typeof(string));
  
        dtjv.Columns.Add("Items", typeof(string));
        dtjv.Columns.Add("Qty", typeof(decimal));
        dtjv.Columns.Add("Rate", typeof(decimal));
        dtjv.Columns.Add("bs_Id", typeof(decimal));
        DataRow dr = null;

        dr = dtjv.NewRow();
        foreach (GridViewRow gvr in GVBalance.Rows)
        {

            Label lblNote = (Label)gvr.FindControl("lblNote");
            Label lblItem = (Label)gvr.FindControl("lblItem");
            TextBox txtQTY = (TextBox)gvr.FindControl("txtQTY");
            TextBox txtRATE = (TextBox)gvr.FindControl("txtRATE");
            HiddenField bs_Id = (HiddenField)gvr.FindControl("hdnbsid");

            decimal qty = 0;
            decimal rate = 0;
            if (decimal.TryParse(txtQTY.Text, out qty) && decimal.TryParse(txtRATE.Text, out rate))
            {
                if (qty > 0 && rate > 0)
                {


                    dr["Comp_Code"] = Convert.ToString(DDLCompanyName.SelectedValue);
                    dr["NOTE"] = Convert.ToString(lblNote.Text);
                    dr["Items"] = Convert.ToString(lblItem.Text);
                    dr["QTY"] = Convert.ToInt32(txtQTY.Text);
                    dr["RATE"] = Convert.ToDecimal(txtRATE.Text);
                    dr["bs_Id"] = Convert.ToDecimal(bs_Id.Value);
                    dtjv.Rows.Add(dr);
                }
               
            }


            //DataView hdnAction = dtjv.DefaultView;
            //hdnAction.RowFilter = "Status='I'";

            //DataTable hdnAction = hdnAction.ToTable();

            //hdnAction.RowFilter = "Status='U'";

            //DataTable dtInsert = hdnAction.ToTable(); 

            string result = "";
            if (Convert.ToString(DDLCompanyName.SelectedValue) != "0" && dtjv.Rows.Count > 0)
            {

                result = Notes.Update_BS_NOTES(DDLCompanyName.SelectedValue, dtjv, Session["Employee_Code"].ToString());
                if (result == "")
                {

                    GVBalance.Visible = false;
                    DDLCompanyName.ClearSelection();
                    ddlFinanceYear.ClearSelection();

                    msg.ShowMessage("Record Updation is Successfully done.", null, ErrorContainer, MyMessage, "Success");
                }
                else if (result != "")
                {

                    msg.ShowMessage("Record Updation Failed. Please Try Again.!", null, ErrorContainer, MyMessage, "Warning");
                }


                else
                {
                    msg.ShowMessage("Record Updation Failed. Please Try Again.!", null, ErrorContainer, MyMessage, "Warning");

                }
            }
        }
    }

    protected void btnUpdate_Click1(object sender, EventArgs e)
    {

    }
    protected void fill_note()
    {

        DataTable dt = Notes.SELECT_BS_NOTES(ddlFinanceYear.SelectedValue, DDLCompanyName.SelectedValue);

 
                GVBalance.DataSource = dt;
                GVBalance.DataBind();
                ErrorContainer.Visible = false;
                GVBalance.Visible = true;

        }



    protected void ddlFinanceYear_SelectedIndexChanged(object sender, EventArgs e)
    {
         fill_note();
    }
    protected void GVBalance_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField bs_Id = (HiddenField)gvr.FindControl("hdnbsid");
            HiddenField hdnAction = (HiddenField)gvr.FindControl("hdnAction");
            if (bs_Id.Value == null || bs_Id.Value == "0")
            {
                hdnAction.Value = "I";
            }
            else 
            {
                hdnAction.Value = "U";
            }

        }
    }
}