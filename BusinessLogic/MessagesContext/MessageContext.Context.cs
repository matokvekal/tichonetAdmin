﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Business_Logic.MessagesContext
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MessageContext : DbContext
    {
        public MessageContext()
            : base("name=MessageContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tblFilter> tblFilters { get; set; }
        public virtual DbSet<tblRecepientFilter> tblRecepientFilters { get; set; }
        public virtual DbSet<tblRecepientFilterTableName> tblRecepientFilterTableNames { get; set; }
        public virtual DbSet<tblTemplate> tblTemplates { get; set; }
        public virtual DbSet<tblWildcard> tblWildcards { get; set; }
        public virtual DbSet<tblRecepientCard> tblRecepientCards { get; set; }
    }
}
