using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business_Logic;


namespace ticonet
{
    public class familyViewModel
    {
        public tblFamily EditableTblFamily { get; set; }
        public List<tblStudent> students { get; set; }
        public bool parent1EmailConfirm
        {
            get
            {
                return EditableTblFamily.parent1EmailConfirm == true;
            }
            set
            {
                EditableTblFamily.parent1EmailConfirm = value;
            }
        }
        public bool parent1EmailGetAlert
        {
            get
            {
                return EditableTblFamily.parent1GetAlertByEmail == true;
            }
            set
            {
                EditableTblFamily.parent1GetAlertByEmail = value;
            }
        }


        public bool parent1CellConfirm
        {
            get
            {
                return EditableTblFamily.parent1CellConfirm == true;
            }
            set
            {
                EditableTblFamily.parent1CellConfirm = value;
            }
        }
        public bool parent1CellGetAlert
        {
            get
            {
                return EditableTblFamily.parent1GetAlertBycell == true;
            }
            set
            {
                EditableTblFamily.parent1GetAlertBycell = value;
            }
        }
        public bool parentAgree
        {
            get
            {
                return EditableTblFamily.iAgree == true;
            }
            set
            {
                EditableTblFamily.iAgree = value;
            }
        }
        public bool allredyUse
        {
            get
            {
                return EditableTblFamily.allredyUsed == true;
            }
            set
            {
                EditableTblFamily.allredyUsed = value;
            }
        }
    }

 
}