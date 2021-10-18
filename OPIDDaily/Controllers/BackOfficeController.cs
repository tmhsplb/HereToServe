﻿using DataTables.Mvc;
using OPIDDaily.DAL;
using OPIDDaily.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using OPIDDaily.Utils;
using OPIDDaily.DataContexts;
using OpidDailyEntities;
using log4net;

namespace OPIDDaily.Controllers
{
    [Authorize(Roles = "BackOffice")]
    public class BackOfficeController : SharedController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(BackOfficeController));
       
        public ActionResult Home()
        {
            // Log.Debug("Return view Home");
            return View("Home");
        }

        public ActionResult BackOfficeServiceTicket()
        {
            int nowServing = NowServing();

            if (nowServing == 0)
            {
                ViewBag.Warning = "Please first select a client from the Dashboard.";
                return View("Warning");
            }

            Client client = Clients.GetClient(nowServing, null);

            if (client == null)
            {
                ViewBag.Warning = "Could not find selected client.";
                return View("Warning");
            }

            if (CheckManager.HasHistory(client.Id))
            {
                return RedirectToAction("ExistingClientServiceTicket");
            }

            return RedirectToAction("ExpressClientServiceTicket");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StoreExpressClientServiceRequest(RequestedServicesViewModel rsvm)
        {
            // Called when 
            //   ~/Views/BackOffice/ExpressClientServiceRequest.cshtml
            // posts to server. rsvm will contain both requested services
            // and supporting documents.
            int nowServing = NowServing();

            if (nowServing == 0)
            {
                ViewBag.Warning = "You waited too long before acting! Session timeout. Go back to Dashboard and try again.";
                return View("Warning");
            }

            Client client = Clients.GetClient(nowServing, null);  // pass null so the supporting documents won't be erased
            string serviceRequestError = ServiceRequestError(rsvm);

            if (!string.IsNullOrEmpty(serviceRequestError))
            {
                ViewBag.ClientName = Clients.ClientBeingServed(client);
                ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
                ViewBag.Age = client.Age;
                ModelState.AddModelError("ServiceRequestError", serviceRequestError);
                rsvm.Agencies = Agencies.GetAgenciesSelectList(client.AgencyId);
                rsvm.MBVDS = MBVDS.GetMBVDSelectList();
                return View("ExpressClientServiceRequest", rsvm);
            }

            Clients.StoreRequestedServicesAndSupportingDocuments(client.Id, rsvm);
            PrepareClientNotes(client, rsvm);
            return RedirectToAction("ManageDashboard", "BackOffice");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StoreExistingClientServiceRequest(RequestedServicesViewModel rsvm)
        {
            // Called when          
            //    ~/Views/BackOffice/ExistingClientServiceRequest.cshtml
            // posts to server. rsvm will contain both requested services
            // and supporting documents.
            int nowServing = NowServing();

            if (nowServing == 0)
            {
                ViewBag.Warning = "You waited too long before acting! Session timeout. Go back to Clients list and try again.";
                return View("Warning");
            }

            Client client = Clients.GetClient(nowServing, null);  // pass null so the supporting documents won't be erased
            string serviceRequestError = ServiceRequestError(rsvm);

            if (!string.IsNullOrEmpty(serviceRequestError))
            {
                ViewBag.ClientName = Clients.ClientBeingServed(client);
                ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
                ViewBag.Age = client.Age;
                ModelState.AddModelError("ServiceRequestError", serviceRequestError);
                rsvm.Agencies = Agencies.GetAgenciesSelectList(client.AgencyId);
                rsvm.MBVDS = MBVDS.GetMBVDSelectList();
                return View("ExistingClientServiceRequest", rsvm);
            }

            Clients.StoreRequestedServicesAndSupportingDocuments(client.Id, rsvm);
            PrepareClientNotes(client, rsvm);
            return RedirectToAction("ManageDashboard", "BackOffice");
        }

        public ActionResult PocketChecksReport()
        {
            return View("PocketChecksReport");
        }

        public JsonResult GetDependentPocketChecks(int id, int page)
        {
            List<PocketCheckViewModel> dependentPocketChecks = PocketChecks.GetDependentPocketChecks(id);

            var jsonData = new
            {
                total = 1,
                page = page,
                records = dependentPocketChecks.Count,
                rows = dependentPocketChecks
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public string EditPCRPocketCheck(PocketCheckViewModel pcvm)
        {
            PocketChecks.EditPocketCheck(pcvm);
            return "Success";
        }

        public ActionResult Resolved()
        {
            return View();
        }

        public JsonResult GetResolvedChecks([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IQueryable<CheckViewModel> query = CheckManager.GetResolvedChecksAsQueryable();
            var totalCount = query.Count();

            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                query = query.Where(p => p.Name.Contains(value) ||
                                         p.sRecordID.Contains(value) ||
                                         p.sInterviewRecordID.Contains(value) ||
                                         p.sNum.Contains(value) ||
                                         p.sDate.Contains(value) ||
                                         p.Service.Contains(value) ||
                                         p.Disposition.Contains(value)
                                   );
            }

            var filteredCount = query.Count();

            query = query.OrderBy("Name asc");

            // Paging
            query = query.Skip(requestModel.Start).Take(requestModel.Length);

            var data = query.Select(rcheck => new
            {
                sRecordID = rcheck.sRecordID,
                sInterviewRecordID = rcheck.sInterviewRecordID,
                Name = rcheck.Name,
                sNum = rcheck.sNum,
                sDate = rcheck.sDate,
                Service = rcheck.Service,
                Disposition = rcheck.Disposition
            }).ToList();

            return Json(new DataTablesResponse(requestModel.Draw, data, filteredCount, totalCount), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Merge()
        {
            TempData["UploadedFile"] = "";
            return View();
        }

        [HttpPost]
        public ActionResult UploadOpidDailyFile(FileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postedFile = Request.Files["File"];

                string fname = postedFile.FileName;

                if (!fname.EndsWith("xlsx"))
                {
                    ModelState.AddModelError("OPIDDailyError", "This is not an Excel xlsx file.");
                    return View("Merge", model);
                }

                List<string> docfiles = FileUploader.UploadFile(postedFile);
                TempData["UploadedFile"] = fname;
                TempData["FileType"] = "OPIDDaily";
                ViewData["UploadedOPIDDailyFile"] = string.Format("Uploaded File: {0}", fname);

                return View("Merge", model);
            }

            ModelState.AddModelError("OPIDDailyError", "Please supply a file name.");
            return View("Merge", model);
        }

        [HttpPost]
        public ActionResult UploadOpidDailyTrackingFile(FileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postedFile = Request.Files["File"];

                string fname = postedFile.FileName;

                if (!fname.EndsWith("xlsx"))
                {
                    ModelState.AddModelError("OPIDDailyError", "This is not an Excel xlsx file.");
                    return View("Merge", model);
                }

                List<string> docfiles = FileUploader.UploadFile(postedFile);
                TempData["UploadedFile"] = fname;
                TempData["FileType"] = "OPIDDailyTracking";
                ViewData["UploadedOPIDDailyTrackingFile"] = string.Format("Uploaded File: {0}", fname);

                return View("Merge", model);
            }

            ModelState.AddModelError("OPIDDailyError", "Please supply a file name.");
            return View("Merge", model);
        }

        [HttpPost]
        public ActionResult UploadNewClientsFile(FileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postedFile = Request.Files["File"];

                string fname = postedFile.FileName;

                if (!fname.EndsWith("xlsx"))
                {
                    ModelState.AddModelError("OPIDDailyError", "This is not an Excel xlsx file.");
                    return View("Merge", model);
                }

                List<string> docfiles = FileUploader.UploadFile(postedFile);
                TempData["UploadedFile"] = fname;
                TempData["FileType"] = "NewClients";
                ViewData["UploadedNewClientsFile"] = string.Format("Uploaded File: {0}", fname);

                return View("Merge", model);
            }

            ModelState.AddModelError("OPIDDailyError", "Please supply a file name.");
            return View("Merge", model);
        }

        [HttpPost]
        public ActionResult UploadVoidedChecksFile(FileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postedFile = Request.Files["File"];

                string fname = postedFile.FileName;

                if (!fname.EndsWith("xlsx"))
                {
                    ModelState.AddModelError("VoidedChecksError", "This is not an Excel xlsx file.");
                    return View("Merge", model);
                }

                List<string> docfiles = FileUploader.UploadFile(postedFile);
                TempData["UploadedFile"] = fname;
                TempData["FileType"] = "VoidedChecks";
                ViewData["UploadedVCFile"] = string.Format("Uploaded File: {0}", fname);

                return View("Merge", model);
            }

            ModelState.AddModelError("VoidedChecksError", "Please supply a file name.");
            return View("Merge", model);
        }

        [HttpPost]
        public ActionResult UploadVoidedOrigenChecksFile(FileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postedFile = Request.Files["File"];

                string fname = postedFile.FileName;

                if (!fname.EndsWith("xlsx"))
                {
                    ModelState.AddModelError("VoidedChecksError", "This is not an Excel xlsx file.");
                    return View("Merge", model);
                }

                List<string> docfiles = FileUploader.UploadFile(postedFile);
                TempData["UploadedFile"] = fname;
                TempData["FileType"] = "VoidedChecks";
                ViewData["UploadedVCFile"] = string.Format("Uploaded File: {0}", fname);

                return View("Merge", model);
            }

            ModelState.AddModelError("VoidedChecksError", "Please supply a file name.");
            return View("Merge", model);
        }

        [HttpPost]
        public ActionResult UploadClearedChecksFile(FileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postedFile = Request.Files["File"];

                string fname = postedFile.FileName;

                if (!fname.EndsWith("xlsx"))
                {
                    ModelState.AddModelError("ClearedChecksError", "This is not an Excel xlsx file.");
                    return View("Merge", model);
                }

                List<string> docfiles = FileUploader.UploadFile(postedFile);
                TempData["UploadedFile"] = fname;
                TempData["FileType"] = "ClearedChecks";
                ViewData["UploadedCCFile"] = string.Format("Uploaded File: {0}", fname);

                return View("Merge", model);
            }

            ModelState.AddModelError("ClearedChecksError", "Please supply a file name.");
            return View("Merge", model);
        }

        [HttpPost]
        public ActionResult UploadClearedOrigenChecksFile(FileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postedFile = Request.Files["File"];

                string fname = postedFile.FileName;

                if (!fname.EndsWith("xlsx"))
                {
                    ModelState.AddModelError("ClearedChecksError", "This is not an Excel xlsx file.");
                    return View("Merge", model);
                }

                List<string> docfiles = FileUploader.UploadFile(postedFile);
                TempData["UploadedFile"] = fname;
                TempData["FileType"] = "ClearedChecks";
                ViewData["UploadedCCFile"] = string.Format("Uploaded File: {0}", fname);

                return View("Merge", model);
            }

            ModelState.AddModelError("ClearedChecksError", "Please supply a file name.");
            return View("Merge", model);
        }

        [HttpPost]
        public ActionResult PerformMerge()
        {
            string uploadedFile = TempData["UploadedFile"] as string;
            string fileType = TempData["FileType"] as string;

            if (string.IsNullOrEmpty(uploadedFile))
            {
                ViewData["MergeStatus"] = "Please choose a file to merge";
                return View("Merge");
            }

            string filePath = string.Format("~/Uploads/{0}", uploadedFile);
            string mappedPath = HttpContext.Server.MapPath(filePath);

            Merger.PerformMerge(mappedPath, fileType);

            ViewData["MergeStatus"] = "Merge Complete";
            return View("Merge");
        }

        public ActionResult ChangePassword()
        {
            return View("ChangePassword");
        }
    }
}