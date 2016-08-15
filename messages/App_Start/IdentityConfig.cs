//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Threading.Tasks;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.Owin;
//using System.ComponentModel.DataAnnotations;


//namespace ticonet
//{
//    public class IdentityConfig
//    {
//        public class MyRequiredAttribute : System.ComponentModel.DataAnnotations.RequiredAttribute
//        {
//            public override string FormatErrorMessage(string name)
//            {
//                return base.FormatErrorMessage(string.Format("This is my error for {0}", name));
//            }
//        }
//        public class CustomRequiredAttribute : RequiredAttribute
//        {
//            public override string FormatErrorMessage(string name)
//            {
//                return "oo";
//            }
//        }
//        public class GenericRequired : RequiredAttribute
//        {
//            public GenericRequired()
//            {
//                this.ErrorMessage = "{0} Blah blah";
//            }
//        }
//        public class MyRequiredAttributeq : System.ComponentModel.DataAnnotations.RequiredAttribute
//        {
//            public override string FormatErrorMessage(string name)
//            {
//                return base.FormatErrorMessage(string.Format("This is my error for {0}", name));
//            }
//        }
//    }
//}

