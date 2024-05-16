using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.Linq;

namespace MergeAzureData.Module.BusinessObjects
{
    [DefaultClassOptions, NavigationItem("Tools"), ImageName("State_Priority_High")]
    public class DataMerge : XPObject
    {
        public DataMerge(Session session) : base(session)
        {
        }
        public override void AfterConstruction() { base.AfterConstruction(); }


        string sQLCode;
        string listOfFields;
        string processDescription;

        [Size(100)]
        public string ProcessDescription
        {
            get => processDescription;
            set => SetPropertyValue(nameof(ProcessDescription), ref processDescription, value);
        }


        [Size(4096)]
        [ModelDefault("RowCount", "8")]

        public string ListOfFields
        {
            get => listOfFields;
            set => SetPropertyValue(nameof(ListOfFields), ref listOfFields, value);
        }


        [Size(4096)]
        [ModelDefault("RowCount", "8")]

        public string SQLCode { get => sQLCode; set => SetPropertyValue(nameof(SQLCode), ref sQLCode, value); }

        #region Asociations
        [XafDisplayName("Databases")]
        [Association("DataMerge-DatabaseInfo")]
        public XPCollection<DatabaseInfo> Databases { get { return GetCollection<DatabaseInfo>(nameof(Databases)); } }
        #endregion
    }
}