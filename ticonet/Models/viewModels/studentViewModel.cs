using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business_Logic;



namespace ticonet
{
    public class studentViewModel
    {
        public tblStudent EditableTblStudents { get; set; }
        public List<string> clas { get; set; }

        //public string streetName { get; set; }
       
            
        public bool CellConfirm
        {
            get
            {
                return EditableTblStudents.CellConfirm == true;
            }
            set
            {
                EditableTblStudents.CellConfirm = value;
            }
        }
        public bool EmailConfirm
        {
            get
            {
                return EditableTblStudents.EmailConfirm == true;
            }
            set
            {
                EditableTblStudents.EmailConfirm = value;
            }
        }


        public bool GetAlertByCell
        {
            get
            {
                return EditableTblStudents.GetAlertByCell == true;
            }
            set
            {
                EditableTblStudents.GetAlertByCell = value;
            }
        }
        public bool GetAlertByEmail
        {
            get
            {
                return EditableTblStudents.GetAlertByEmail == true;
            }
            set
            {
                EditableTblStudents.GetAlertByEmail = value;
            }
        }
        public bool paymentStatus
        {
            get
            {
                return EditableTblStudents.paymentStatus == true;
            }
            set
            {
                EditableTblStudents.paymentStatus = value;
            }
        }
        public bool subsidyStatus
        {
            get
            {
                return EditableTblStudents.subsidy== true;
            }
            set
            {
                EditableTblStudents.subsidy = value;
            }
        }
    }


}