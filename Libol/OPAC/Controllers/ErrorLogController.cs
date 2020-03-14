using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Runtime.Remoting.Services;

namespace OPAC.Controllers
{
    public class ErrorLogController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [HttpPost]
        public void Log(string Path, string Error, string Message, string Track)
        {
            //log the error!
            log.Info("");
            log.Info("");
            log.Info("------------------------------------------------------------------------------------------------------");
            log.Info("Time: " + DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"));
            log.Info(Path);
            log.Info(Error);
            log.Info(Message);
            log.Info(Track);

            string errorLogFilePath = Server.MapPath("~/ErrorLog/ErrorLog.txt");
            string errorContent = "Time: " + DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy") + Environment.NewLine +
                                  "Path: " + Path + Environment.NewLine + 
                                  "Error: " + Error + Environment.NewLine + 
                                  "Message: " + Message + Environment.NewLine + 
                                  "Track: " + FormatTrack(Track);
            System.IO.File.WriteAllText(errorLogFilePath, errorContent);
        }

        private string FormatTrack(string track)
        {
            string newTrack = "";
            int count = 0;
            var trackList = track.Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim())
                .ToList();

            foreach (var t in trackList)
            {
                count++;
                newTrack += t;
                if (count < trackList.Count)
                {
                    newTrack += Environment.NewLine + "\t";
                }
            }

            return newTrack;
        }
    }
}