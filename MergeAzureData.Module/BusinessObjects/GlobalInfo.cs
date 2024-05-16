using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Linq;

namespace MergeAzureData.Module.BusinessObjects
{
    [DefaultClassOptions, NavigationItem("Settings")]

    [Appearance(
        "GlobalInfoAction",
        AppearanceItemType.Action,
        "true",
        TargetItems = "New,Delete,SaveAndClose",
        Criteria = "1=1",
        Enabled = false,
        Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]

    public class GlobalInfo : XPObject
    {
        public GlobalInfo(Session session) : base(session)
        {
        }
        public override void AfterConstruction() { base.AfterConstruction(); }

        protected override void OnSaving()
        {
            try
            {
                string sqlQuery = @$"
                OPEN MASTER KEY DECRYPTION BY PASSWORD = 'xari@2024#';

                if not exists (select * from sys.database_credentials where name = '{ScopedCredential}')
                begin
                   CREATE DATABASE SCOPED CREDENTIAL {ScopedCredential}
                   WITH IDENTITY = '{Identity}',
                   SECRET = '{Secret}';
                end 
                ";
                Session.ExecuteNonQuery(sqlQuery);
                var xyz = Session.ExecuteQuery(sqlQuery);
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            base.OnSaving();
        }


        string secret;
        string identity;
        string scopedCredential;

        [RuleRequiredField(DefaultContexts.Save)]
        [Size(100)]
        public string ScopedCredential
        {
            get => scopedCredential;
            set => SetPropertyValue(nameof(ScopedCredential), ref scopedCredential, value);
        }

        [RuleRequiredField(DefaultContexts.Save)]
        [Size(100)]
        public string Identity { get => identity; set => SetPropertyValue(nameof(Identity), ref identity, value); }

        [RuleRequiredField(DefaultContexts.Save)]
        [Size(100)]
        public string Secret { get => secret; set => SetPropertyValue(nameof(Secret), ref secret, value); }
    }
}