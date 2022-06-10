using PlumbingShopBusinessLogic.BusinessLogics;
using PlumbingShopBusinessLogic.MailWorker;
using PlumbingShopBusinessLogic.OfficePackage;
using PlumbingShopBusinessLogic.OfficePackage.Implements;
using PlumbingShopContracts.BindingModels;
using PlumbingShopContracts.BusinessLogicsContracts;
using PlumbingShopContracts.StoragesContracts;
using PlumbingShopDatabaseImplement.Implements;
using System;
using System.Configuration;
using System.Threading;
using System.Windows.Forms;
using Unity;
using Unity.Lifetime;
using PlumbingShopContracts.Attributes;
using PlumbingShopContracts.ViewModels;
using System.Collections.Generic;

namespace PlumbingShopView
{
    static class Program
    {
        private static IUnityContainer container = null;

        public static IUnityContainer Container { get { if (container == null) container = BuildUnityContainer(); return container; } }
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            var mailSender = Container.Resolve<AbstractMailWorker>();
            mailSender.MailConfig(new MailConfigBindingModel
            {
                MailLogin = ConfigurationManager.AppSettings["MailLogin"],
                MailPassword = ConfigurationManager.AppSettings["MailPassword"],
                SmtpClientHost = ConfigurationManager.AppSettings["SmtpClientHost"],
                SmtpClientPort = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpClientPort"]),
                PopHost = ConfigurationManager.AppSettings["PopHost"],
                PopPort = Convert.ToInt32(ConfigurationManager.AppSettings["PopPort"])
            });
            // ������� ������
            var timer = new System.Threading.Timer(new TimerCallback(MailCheck), null, 0, 100000);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(Container.Resolve<FormMain>());
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var currentContainer = new UnityContainer();

            currentContainer.RegisterType<IComponentStorage, ComponentStorage>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<IOrderStorage, OrderStorage>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<ISanitaryEngineeringStorage, SanitaryEngineeringStorage>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<IClientStorage, ClientStorage>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<IImplementerStorage, ImplementerStorage>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<IMessageInfoStorage, MessageInfoStorage>(new HierarchicalLifetimeManager());

            currentContainer.RegisterType<IComponentLogic, ComponentLogic>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<IOrderLogic, OrderLogic>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<ISanitaryEngineeringLogic, SanitaryEngineeringLogic>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<IReportLogic, ReportLogic>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<AbstractSaveToExcel, SaveToExcel>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<AbstractSaveToWord, SaveToWord>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<AbstractSaveToPdf, SaveToPdf>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<IClientLogic, ClientLogic>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<IImplementerLogic, ImplementerLogic>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<IWorkProcess, WorkModeling>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<IMessageInfoLogic, MessageInfoLogic>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<AbstractMailWorker, MailKitWorker>(new SingletonLifetimeManager());
            currentContainer.RegisterType<IBackUpInfo, BackUpInfo>(new HierarchicalLifetimeManager());
            currentContainer.RegisterType<IBackUpLogic, BackUpLogic>(new HierarchicalLifetimeManager());
            return currentContainer;
        }
        private static void MailCheck(object obj) => Container.Resolve<AbstractMailWorker>().MailCheck();

        public static void ConfigGrid<T>(List<T> data, DataGridView grid)
        {
            var type = typeof(T);
            var config = new List<string>();
            grid.Columns.Clear();
            foreach (var prop in type.GetProperties())
            {
                var attributes = prop.GetCustomAttributes(typeof(ColumnAttribute), true);
                if (attributes != null && attributes.Length > 0)
                {
                    foreach (var attr in attributes)
                    {
                        if (attr is ColumnAttribute columnAttr)
                        {
                            config.Add(prop.Name);
                            var column = new DataGridViewTextBoxColumn
                            {
                                Name = prop.Name,
                                ReadOnly = true,
                                HeaderText = columnAttr.Title,
                                Visible = columnAttr.Visible,
                                Width = columnAttr.Width
                            };
                            if (columnAttr.GridViewAutoSize != GridViewAutoSize.None)
                            {
                                column.AutoSizeMode = (DataGridViewAutoSizeColumnMode)Enum.Parse(typeof(DataGridViewAutoSizeColumnMode),
                                columnAttr.GridViewAutoSize.ToString());
                            }
                            grid.Columns.Add(column);
                        }
                    }
                }
            }

            foreach (var elem in data)
            {
                var objs = new List<object>();
                foreach (var conf in config)
                {
                    var value = elem.GetType().GetProperty(conf).GetValue(elem);
                    if (value is Dictionary<int, (string, int)>)
                    {
                        objs.Add(((SanitaryEngineeringViewModel)(object)elem).GetComponents());
                    }
                    objs.Add(value);
                }
                grid.Rows.Add(objs.ToArray());
            }
        }
    }
}
