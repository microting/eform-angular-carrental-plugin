﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Runtime.InteropServices;
using Microting.eFormRentableItemBase.Infrastructure.Data;


namespace RentableItems.Pn.Tests
{
    [TestFixture]
    public abstract class DbTestFixture
    {

        protected eFormRentableItemPnDbContext DbContext;
        protected string ConnectionString;


        private static string userName = "__USER_NAME__";
        private static string password = "__PASSWORD__";
        private static string databaseName = "__DBNAME__";
        private static string databaseServerId = "__DB_SERVER_ID__";
        private static string directoryId = "__DIRECTORY_ID__";
        private static string applicationId = "__APPLICATION_ID__";

        //public RentableItemsPnDbAnySql db;

        public void GetContext(string connectionStr)
        {

            DbContextOptionsBuilder<eFormRentableItemPnDbContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<eFormRentableItemPnDbContext>();

            if (ConnectionString.ToLower().Contains("convert zero datetime"))
            {
                dbContextOptionsBuilder.UseMySql(connectionStr);
            }
            else
            {
                dbContextOptionsBuilder.UseSqlServer(connectionStr);
            }
            dbContextOptionsBuilder.UseLazyLoadingProxies(true);
            DbContext = new eFormRentableItemPnDbContext(dbContextOptionsBuilder.Options);

            DbContext.Database.Migrate();
            DbContext.Database.EnsureCreated();
            //return db;

        }

        [SetUp]
        public void Setup()
        {
            //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //{
            //    ConnectionString = @"data source=(LocalDb)\SharedInstance;Initial catalog=rentable-items-pn-tests;Integrated Security=True";
            //}
            //else
            //{
                ConnectionString = @"Server = localhost; port = 3306; Database = rentable-items-pn-tests; user = root; Convert Zero Datetime = true;";
            //}


            GetContext(ConnectionString);


            DbContext.Database.SetCommandTimeout(300);

            try
            {
                ClearDb();
            }
            catch
            {
                DbContext.Database.Migrate();
            }

            DoSetup();
        }

        [TearDown]
        public void TearDown()
        {

            ClearDb();

            ClearFile();

            DbContext.Dispose();
        }

        public void ClearDb()
        {
            List<string> modelNames = new List<string>();
            modelNames.Add("RentableItemsContractVersions");
            modelNames.Add("RentableItemContract");
            modelNames.Add("ContractInspectionVersion");
            modelNames.Add("ContractInspection");
            modelNames.Add("ContractVersions");
            modelNames.Add("Contract");
            modelNames.Add("RentableItemsSettingsVersions");
            modelNames.Add("RentableItemsSettings");
            modelNames.Add("RentableItemsVersion");
            modelNames.Add("RentableItem");


            foreach (var modelName in modelNames)
            {
                try
                {
                    string sqlCmd = string.Empty;
                    if (DbContext.Database.IsMySql())
                    {
                        sqlCmd = string.Format("DELETE FROM `{0}`.`{1}`", "rentable-items-pn-tests", modelName);
                    }
                    else
                    {
                        sqlCmd = string.Format("DELETE FROM [{0}]", modelName);
                    }
                    DbContext.Database.ExecuteSqlCommand(sqlCmd);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        private string path;

        public void ClearFile()
        {
            path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            path = System.IO.Path.GetDirectoryName(path).Replace(@"file:\", "");

            string picturePath = path + @"\output\dataFolder\picture\Deleted";

            DirectoryInfo diPic = new DirectoryInfo(picturePath);

            try
            {
                foreach (FileInfo file in diPic.GetFiles())
                {
                    file.Delete();
                }
            }
            catch { }


        }
        public virtual void DoSetup() { }

    }
}
