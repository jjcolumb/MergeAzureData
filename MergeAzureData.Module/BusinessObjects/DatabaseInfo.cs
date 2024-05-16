using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.Linq;

namespace MergeAzureData.Module.BusinessObjects
{
    [DefaultClassOptions, NavigationItem("Settings")]
    public class DatabaseInfo : XPObject
    {
        public DatabaseInfo(Session session) : base(session)
        {
        }
        public override void AfterConstruction() { base.AfterConstruction(); }

        protected override void OnSaving()
        {
            try
            {
                GlobalInfo globalInfo = this.Session.FindObject<GlobalInfo>(null);

                if(globalInfo != null)
                {
                    string sqlQuery = @$"
                                IF  NOT EXISTS (
                                SELECT  name
                                FROM sys.external_data_sources
                                WHERE [name] =  '{DataSourceName}'
                                )
                                BEGIN
                                CREATE EXTERNAL DATA SOURCE {DataSourceName} WITH
                                    (TYPE = RDBMS,
                                    LOCATION = '{Location}',
                                    DATABASE_NAME = '{DatabaseName}',
                                    CREDENTIAL = {globalInfo.ScopedCredential},
                                ) ;
                                END
                                ";
                    Session.ExecuteNonQuery(sqlQuery);
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            base.OnSaving();
        }


        string location;
        string databaseName;

        [Size(100)]
        public string DatabaseName
        {
            get => databaseName;
            set => SetPropertyValue(nameof(DatabaseName), ref databaseName, value);
        }


        [Size(300)]
        public string Location { get => location; set => SetPropertyValue(nameof(Location), ref location, value); }


        public string DataSourceName => $"DataSource_{DatabaseName}";


        #region Asociations


        DataMerge dataMerge;
        [Association("DataMerge-DatabaseInfo")]
        [VisibleInListView(false), VisibleInDetailView(false)]
        public XPCollection<DataMerge> DataMerges
        {
            get { return GetCollection<DataMerge>(nameof(DataMerges)); }
        }


        #endregion


    }
}