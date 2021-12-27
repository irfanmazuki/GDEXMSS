using IronPython.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using GDEXMSS.Models;

namespace GDEXMSS.Controllers
{
    public class AnalysisController : Controller
    {
        public static string RunFromCmd(string rCodeFilePath, string args)
        {
            string file = rCodeFilePath;
            string result = string.Empty;

            try
            {

                var info = new ProcessStartInfo(@"C:\Users\Irfan Mazuki\AppData\Local\Programs\Python\Python37\python.exe");
                info.Arguments = rCodeFilePath + " " + "\"" + args + "\"";

                info.RedirectStandardInput = false;
                info.RedirectStandardOutput = true;
                info.UseShellExecute = false;
                info.CreateNoWindow = true;

                using (var proc = new Process())
                {
                    proc.StartInfo = info;
                    proc.Start();
                    proc.WaitForExit();
                    if (proc.ExitCode == 0)
                    {
                        result = proc.StandardOutput.ReadToEnd();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("R Script failed: " + result, ex);
            }
        }
        [HttpGet]
        public ActionResult Index()
        {
            searchAnalysi searchAnalysis = new searchAnalysi();
            return View(searchAnalysis);
        }
        [HttpPost]
        public ActionResult Index(searchAnalysi objAnalysis)
        {
            string res = RunFromCmd(@"C:\PythonScript\trendAnalysis.py", objAnalysis.searchKeyword);
            objAnalysis.searchResult = res;
            return View(objAnalysis);
        }
    }
}