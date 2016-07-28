using Business_Logic;
using Business_Logic.Helpers;
using System;

namespace ticonet.Models
{
    public class FamilyModel
    {
        public FamilyModel(tblFamily data)
        {
            familyId = data.familyId;
            ParentId = data.ParentId;
            parent1Type = data.parent1Type;
            parent1FirstName = data.parent1FirstName;
            parent1LastName = data.parent1LastName;
            parent1Email = data.parent1Email;
            parent1EmailConfirm = data.parent1EmailConfirm;
            parent1GetAlertByEmail = data.parent1GetAlertByEmail;
            parent1CellPhone = data.parent1CellPhone;
            parent1CellConfirm = data.parent1CellConfirm;
            parent1GetAlertBycell = data.parent1GetAlertBycell;
            parent2Type = data.parent2Type;
            parent2FirstName = data.parent2FirstName;
            parent2LastName = data.parent2LastName;
            parent2Email = data.parent2Email;
            parent2EmailConfirm = data.parent2EmailConfirm;
            parent2GetAlertByEmail = data.parent2GetAlertByEmail;
            parent2CellPhone = data.parent2CellPhone;
            parent2CellConfirm = data.parent2CellConfirm;
            parent2GetAlertBycell = data.parent2GetAlertBycell;
            date = data.date;
            LastUpdate = data.LastUpdate;
            paymentOk = data.paymentOk;
            paymentDateConfirm = data.paymentDateConfirm;
            iAgree = data.iAgree;
            subsidy = data.subsidy;
            allredyUsed = data.allredyUsed;
            oneParentOnly = data.oneParentOnly;
            PaymentPlanID = data.PaymentPlanID;
            PaymentRequestID = data.PaymentRequestID;
            registrationStatus = data.registrationStatus;
        }

        public FamilyModel() { }

        public int familyId { get; set; }
        public string ParentId { get; set; }
        public string parent1Type { get; set; }
        public string parent1FirstName { get; set; }
        public string parent1LastName { get; set; }
        public string parent1Email { get; set; }
        public bool parent1EmailConfirm { get; set; }
        public bool parent1GetAlertByEmail { get; set; }
        public string parent1CellPhone { get; set; }
        public bool parent1CellConfirm { get; set; }
        public bool parent1GetAlertBycell { get; set; }
        public string parent2Type { get; set; }
        public string parent2FirstName { get; set; }
        public string parent2LastName { get; set; }
        public string parent2Email { get; set; }
        public Nullable<bool> parent2EmailConfirm { get; set; }
        public Nullable<bool> parent2GetAlertByEmail { get; set; }
        public string parent2CellPhone { get; set; }
        public Nullable<bool> parent2CellConfirm { get; set; }
        public Nullable<bool> parent2GetAlertBycell { get; set; }
        public System.DateTime date { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
        public Nullable<bool> paymentOk { get; set; }
        public Nullable<System.DateTime> paymentDateConfirm { get; set; }
        public bool iAgree { get; set; }
        public Nullable<bool> subsidy { get; set; }
        public Nullable<bool> allredyUsed { get; set; }
        public bool oneParentOnly { get; set; }
        public string PaymentPlanID { get; set; }
        public string PaymentRequestID { get; set; }
        public bool registrationStatus { get; set; }


        public tblFamily ToDbModel()
        {
            return new tblFamily
            {
                familyId = this.familyId,
                ParentId = this.ParentId,
                parent1Type = this.parent1Type,
                parent1FirstName = this.parent1FirstName,
                parent1LastName = this.parent1LastName,
                parent1Email = this.parent1Email,
                parent1EmailConfirm = this.parent1EmailConfirm,
                parent1GetAlertByEmail = this.parent1GetAlertByEmail,
                parent1CellPhone = this.parent1CellPhone,
                parent1CellConfirm = this.parent1CellConfirm,
                parent1GetAlertBycell = this.parent1GetAlertBycell,
                parent2Type = this.parent2Type,
                parent2FirstName = this.parent2FirstName,
                parent2LastName = this.parent2LastName,
                parent2Email = this.parent2Email,
                parent2EmailConfirm = this.parent2EmailConfirm,
                parent2GetAlertByEmail = this.parent2GetAlertByEmail,
                parent2CellPhone = this.parent2CellPhone,
                parent2CellConfirm = this.parent2CellConfirm,
                parent2GetAlertBycell = this.parent2GetAlertBycell,
                date = this.date,
                LastUpdate = this.LastUpdate,
                paymentOk = this.paymentOk,
                paymentDateConfirm = this.paymentDateConfirm,
                iAgree = this.iAgree,
                subsidy = this.subsidy,
                allredyUsed = this.allredyUsed,
                oneParentOnly = this.oneParentOnly,
                PaymentPlanID = this.PaymentPlanID,
                PaymentRequestID = this.PaymentRequestID,
                registrationStatus = this.registrationStatus,
            };
        }
    }
}