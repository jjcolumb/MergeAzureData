using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using MergeAzureData.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MergeAzureData.Module.Controllers
{
    public partial class DataMergeViewController : ViewController<DetailView>
    {
        public DataMergeViewController()
        {
            InitializeComponent();
            TargetObjectType = typeof(DataMerge);
            TargetViewType = ViewType.DetailView;


            SimpleAction mergeData = new SimpleAction(this, "MergeData", PredefinedCategory.Edit);
            mergeData.Caption = "Merge Data";
            mergeData.ImageName = "BO_Data";

            mergeData.Execute += MergeData_Execute;
        }

        private void MergeData_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            string queryString = string.Empty;

            var currentObject = View.CurrentObject as DataMerge;
            if(currentObject != null)
            {
                // 1. Loop databases
                int i = 1;
                foreach(var d in currentObject.Databases.OrderBy(db => db.DatabaseName))
                {
                    queryString += $"Declare @T{i} Table ({currentObject.ListOfFields});" + Environment.NewLine;
                    queryString += $"Insert @T{i} EXEC sp_execute_remote" + Environment.NewLine;
                    queryString += $"@data_source_name = N'{d.DataSourceName}'," + Environment.NewLine;
                    queryString += $"@stmt = N'{currentObject.SQLCode.Replace("DatabaseName", "''"+d.DatabaseName+"''")}'" +
                        Environment.NewLine +
                        Environment.NewLine;

                    i++;
                }

                i = 1;
                queryString += Environment.NewLine;
                foreach(var d in currentObject.Databases.OrderBy(db => db.DatabaseName))
                {
                    string tableName = $"@T{i}";
                    queryString += $"{ReplaceTableNameWithParameter(currentObject.SQLCode.Replace("DatabaseName", "'"+d.DatabaseName+"'"), tableName)}" +
                        Environment.NewLine;
                    ;
                    if(i < currentObject.Databases.Count)
                    {
                        queryString += "union " + Environment.NewLine;
                    }
                    i++;
                }

                StringBuilder resultString = new StringBuilder();
                SelectedData selectedData = ((XPObjectSpace)View.ObjectSpace).Session.ExecuteQuery(queryString);
                foreach(var result in selectedData.ResultSet)
                {
                    foreach(var row in result.Rows)
                    {
                        for(int l = 0; l < row.Values.Length; l++)
                        {
                            resultString.Append("'");
                            resultString.Append(row.Values[l].ToString());
                            resultString.Append("',");
                        }
                    }
                }

                File.WriteAllText(@"c:\spooler\sampledata.txt", resultString.ToString());
            }
        }

        protected override void OnActivated() { base.OnActivated(); }
        protected override void OnViewControlsCreated() { base.OnViewControlsCreated(); }
        protected override void OnDeactivated() { base.OnDeactivated(); }

        static string ReplaceTableNameWithParameter(string sqlString, string tableName)
        {
            string pattern = @"FROM\s+([^\s]+)";

            string replacement = $"FROM {tableName}";

            string parameterizedSql = Regex.Replace(sqlString, pattern, replacement, RegexOptions.IgnoreCase);

            return parameterizedSql;
        }
    }
}
