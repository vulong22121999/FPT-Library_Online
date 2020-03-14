using Libol.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Libol.EntityResult;

namespace Libol.SupportClass
{
    public class ExportExcelWithManyRowSearchPatron
    {
        public void ExportDataToExcelFile(string fileName, List<CustomPatron> lstPatron)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/Content/Template1/" + fileName);
            var package = new ExcelPackage(new FileInfo(filePath));


            ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
            for (int i = workSheet.Dimension.End.Row; i >= 3; i--)
            {
                workSheet.DeleteRow(i);
            }

            int row = 3;
            foreach (CustomPatron item in lstPatron)
            {
                workSheet.Cells[row, 1].Value = item.strCode;
                workSheet.Cells[row, 2].Value = item.Name;
                workSheet.Cells[row, 3].Value = item.strDOB;
                workSheet.Cells[row, 4].Value = item.strLastIssuedDate;
                workSheet.Cells[row, 5].Value = item.strExpiredDate;
                workSheet.Cells[row, 6].Value = item.Sex;
                workSheet.Cells[row, 7].Value = item.intEthnicID;
                workSheet.Cells[row, 8].Value = item.intCollegeID;
                workSheet.Cells[row, 9].Value = item.intFacultyID;
                workSheet.Cells[row, 10].Value = item.strGrade;
                workSheet.Cells[row, 11].Value = item.strClass;
                workSheet.Cells[row, 12].Value = item.strAddress;
                workSheet.Cells[row, 13].Value = item.strTelephone;
                workSheet.Cells[row, 14].Value = item.strMobile;
                workSheet.Cells[row, 15].Value = item.strEmail;
                workSheet.Cells[row, 16].Value = item.strNote;
                workSheet.Cells[row, 17].Value = item.intOccupationID;
                workSheet.Cells[row, 18].Value = item.intPatronGroupID;
                row++;
            }
            package.Save();

        }
       
    }
}