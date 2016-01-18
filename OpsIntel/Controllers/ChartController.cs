using OpsIntel.Models;
using OpsIntel.ViewModel;
using OpsIntelLibrary;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;

namespace OpsIntel.Controllers
{
    public class ChartController : ApiController
    {
        private OpsIntel1Entities1 db = new OpsIntel1Entities1();

        //public List<Navigation> GetNavigationData()
        //{
        //    List<Navigation> navigationModel = new List<Navigation>();
        //    using (OpsIntelEntities3 context = new OpsIntelEntities3())
        //    {
        //        navigationModel = (from item in context.VMThresholds
        //                           select item).ToList();
        //    }
        //    return navigationModel;
        //}



        [System.Web.Http.HttpGet]
        public List<VMThreshold> GetVmThresholdData(int id)
        {
            List<VMThreshold> _vmThreshold = new List<VMThreshold>();

            using (OpsIntel1Entities1 context = new OpsIntel1Entities1())
            {
                if (id != 0)
                {
                    _vmThreshold = (from item in context.VMThresholds
                                    where item.Id == id
                                    select item).ToList();
                }
                else
                {
                    _vmThreshold = (from item in context.VMThresholds
                                    select item).ToList();
                }

            }

            return _vmThreshold;
        }



        [System.Web.Http.HttpGet]
        public dynamic GetVmNames()
        {
            using (OpsIntel1Entities1 context = new OpsIntel1Entities1())
            {
                return context.VMThresholds.Select(item => new { item.Id, item.VMName }).ToList();
            }


        }

        [System.Web.Http.HttpGet]
        public List<VMUtilization> GetVmUtilizationDataBasedOnId(int id)
        {
            List<VMUtilization> _vmUtilization = new List<VMUtilization>();

            using (OpsIntel1Entities1 context = new OpsIntel1Entities1())
            {
                _vmUtilization = (from item in context.VMUtilizations
                                  where item.VMId == id
                                  select item).ToList();
            }

            return _vmUtilization;
        }

        [System.Web.Http.HttpGet]
        public List<VMAutomationRule> GetVmAutomationRuleDataBasedOnId(int id)
        {
            List<VMAutomationRule> _vmAutomationRule = new List<VMAutomationRule>();

            using (OpsIntel1Entities1 context = new OpsIntel1Entities1())
            {
                if (id != 0)
                {
                    _vmAutomationRule = (from item in context.VMAutomationRules
                                         where item.VMID == id
                                         select item).ToList();
                }
                else
                {
                    _vmAutomationRule = (from item in context.VMAutomationRules
                                         select item).ToList();
                }

            }

            return _vmAutomationRule;
        }



        [System.Web.Http.HttpGet]
        public List<VMAction> GetVmActions()
        {
            List<VMAction> _vmAction = new List<VMAction>();

            using (OpsIntel1Entities1 context = new OpsIntel1Entities1())
            {
                _vmAction = (from item in context.VMActions
                             select item).ToList();
            }

            return _vmAction;
        }
        [System.Web.Http.HttpGet]
        public List<VMValue> GetVmValues()
        {
            List<VMValue> _vmValue = new List<VMValue>();

            using (OpsIntel1Entities1 context = new OpsIntel1Entities1())
            {
                _vmValue = (from item in context.VMValues
                            select item).ToList();
            }

            return _vmValue;
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage PutData(List<VMAutomationRule> data)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in data)
                {
                    db.Entry<VMAutomationRule>(item).State = System.Data.Entity.EntityState.Modified;
                }
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [System.Web.Http.HttpPost]
        public bool AddVM(VMDetails VMData)
        {

            var FP = VMData.UtilFileLocation;
            var FU = new FileUploader();
            var blob = FU.UploadFileToStorage(FP, true);
            var comm = new MLCommunicator();
            var result = comm.GetUtilDataFromFile(blob);
            var d = result.Result;
            var UtilData = new UtilizationCalculator();
            var UD = UtilData.getUtilPattern(d);

            AddVMInDB(VMData, blob, UD);

            // now we need to insert this data in SQL Azure for the given VM

            foreach (var u in UD)
            {
                Console.WriteLine(u.UtilizationType);
                foreach (var tr in u.TimeRanges)
                {
                    Console.WriteLine("{0}, {1}", tr.StartTime, tr.StopTime);
                }
            }
            //  populateTreeView();
            return true;

            // return Json(true, JsonRequestBehavior.AllowGet);

        }
        public bool AddVMInDB(VMDetails VMData, string BlobLocation, List<SummaryUtil> UtilPattern)
        {
            using (OpsIntel1Entities1 context = new OpsIntel1Entities1())
            {
                VMThreshold details = new VMThreshold
                {

                    VMName = VMData.Name,
                    Location = VMData.Location,
                    Description = VMData.Description,
                    UtilFileLocation = VMData.UtilFileLocation,
                    Status = "Created"

                };

                context.VMThresholds.Add(details);
                context.SaveChanges();
                var id = details.Id;


                foreach (var u in UtilPattern)
                {

                    // Console.WriteLine(u.UtilizationType);
                    foreach (var tr in u.TimeRanges)
                    {
                        //string InsertUtilPatternQuery = "INSERT into VMAutomationRules (VMID, UtilizationType, StartTime, EndTime) ";
                        //InsertUtilPatternQuery += " VALUES(@VMID, @UtilizationType, @StartTime, @EndTime);";
                        //SqlCommand InsertUtilData = new SqlCommand(InsertUtilPatternQuery, conn);

                        double Start = tr.StartTime ?? 0;
                        double Stop = tr.StopTime ?? 0;

                        TimeSpan StartTime = TimeSpan.FromHours(Start);
                        TimeSpan StopTime = TimeSpan.FromHours(Stop);

                        VMAutomationRule InsertUtilData = new VMAutomationRule
                        {
                            VMID = id,
                            UtilizationType = u.UtilizationType,
                            StartTime = StartTime,
                            EndTime = StopTime
                        };

                        context.VMAutomationRules.Add(InsertUtilData);
                        context.SaveChanges();
                        //InsertUtilData.Parameters.AddWithValue("@VMID", id);
                        //InsertUtilData.Parameters.AddWithValue("@UtilizationType", u.UtilizationType);
                        //InsertUtilData.Parameters.AddWithValue("@StartTime", StartTime);
                        //InsertUtilData.Parameters.AddWithValue("@EndTime", StopTime);

                        //var res = InsertUtilData.ExecuteNonQuery();
                        //Console.WriteLine("{0}, {1}", tr.StartTime, tr.StopTime);
                    }
                }


            }
            //using (var conn = new SqlConnection(ConnString))
            //{
            //    conn.Open();

            //    string InsertVMQuery = "Insert into VMThreshold (VMName, CloudServiceName, Description, UtilFileLocation, Status)";
            //    InsertVMQuery += "values (@VMName, @Location, @Description, @UtilFileLocation, @Status); select SCOPE_IDENTITY();";
            //    //InsertVMQuery += "Select @Id = Scope_Identity()";

            //    SqlCommand InsertVMCommand = new SqlCommand(InsertVMQuery, conn);
            //    InsertVMCommand.Parameters.AddWithValue("@VMName", VMData.Name);
            //    InsertVMCommand.Parameters.AddWithValue("@Location", VMData.Location);
            //    InsertVMCommand.Parameters.AddWithValue("@Description", VMData.Description);
            //    InsertVMCommand.Parameters.AddWithValue("@UtilFileLocation", BlobLocation);
            //    InsertVMCommand.Parameters.AddWithValue("@Status", "Created");
            //    //myCommand.Parameters.Add("@Id", SqlDbType.Int);
            //    var id = InsertVMCommand.ExecuteScalar();

            //    foreach (var u in UtilPattern)
            //    {

            //        Console.WriteLine(u.UtilizationType);
            //        foreach (var tr in u.TimeRanges)
            //        {
            //            string InsertUtilPatternQuery = "INSERT into VMAutomationRules (VMID, UtilizationType, StartTime, EndTime) ";
            //            InsertUtilPatternQuery += " VALUES(@VMID, @UtilizationType, @StartTime, @EndTime);";
            //            SqlCommand InsertUtilData = new SqlCommand(InsertUtilPatternQuery, conn);

            //            double Start = tr.StartTime ?? 0;
            //            double Stop = tr.StopTime ?? 0;

            //            TimeSpan StartTime = TimeSpan.FromHours(Start);
            //            TimeSpan StopTime = TimeSpan.FromHours(Stop);

            //            InsertUtilData.Parameters.AddWithValue("@VMID", id);
            //            InsertUtilData.Parameters.AddWithValue("@UtilizationType", u.UtilizationType);
            //            InsertUtilData.Parameters.AddWithValue("@StartTime", StartTime);
            //            InsertUtilData.Parameters.AddWithValue("@EndTime", StopTime);

            //            var res = InsertUtilData.ExecuteNonQuery();
            //            Console.WriteLine("{0}, {1}", tr.StartTime, tr.StopTime);
            //        }
            //    }

            //}

            return true;
        }
        //[System.Web.Http.HttpPost]
        //public HttpResponseMessage PutData(VMDetails VMData)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        try
        //        {
        //            var FP = VMData.UtilFileLocation;
        //            var FU = new FileUploader();
        //            var blob = FU.UploadFileToStorage(FP, true);
        //            var comm = new MLCommunicator();
        //            var result = comm.GetUtilDataFromFile(blob);
        //            var d = result.Result;
        //            var UtilData = new UtilizationCalculator();
        //            var UD = UtilData.getUtilPattern(d);

        //            AddVMInDB(VMData, blob, UD);

        //            // now we need to insert this data in SQL Azure for the given VM

        //            foreach (var u in UD)
        //            {
        //                Console.WriteLine(u.UtilizationType);
        //                foreach (var tr in u.TimeRanges)
        //                {
        //                    Console.WriteLine("{0}, {1}", tr.StartTime, tr.StopTime);
        //                }
        //            }
        //        }
        //        catch (DbUpdateConcurrencyException ex)
        //        {
        //            return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
        //        }
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}
        //[System.Web.Http.HttpPost]
        //public HttpResponseMessage PutData(VMDetails vmDetails)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        db.Entry<VMDetails>(vmDetails).State = System.Data.Entity.EntityState.Modified;

        //        try
        //        {
        //            db.SaveChanges();
        //        }
        //        catch (DbUpdateConcurrencyException ex)
        //        {
        //            return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
        //        }
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}
    }
}
