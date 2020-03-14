using Libol.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;


namespace Libol.SupportClass
{
	public class ExportExcelWithManyRow
	{
		public void ExportDataToExcelFile(string fileName, List<GET_PATRON_LOANINFOR_Result> lstPatron)
		{
			string filePath = HttpContext.Current.Server.MapPath("~/Content/Template1/" + fileName);
			var package = new ExcelPackage(new FileInfo(filePath));


			ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
			for (int i = workSheet.Dimension.End.Row; i >= 3; i--)
			{
				workSheet.DeleteRow(i);
			}

			int row = 3;
			foreach (GET_PATRON_LOANINFOR_Result item in lstPatron)
			{
				workSheet.Cells[row, 1].Value = item.Content;
				workSheet.Cells[row, 2].Value = item.CopyNumber;
				workSheet.Cells[row, 3].Value = item.FullName;
				workSheet.Cells[row, 4].Value = Convert.ToDateTime(item.CheckOutDate).ToString("dd/MM/yyyy");
				workSheet.Cells[row, 5].Value = Convert.ToDateTime(item.CheckInDate).ToString("dd/MM/yyyy");
				workSheet.Cells[row, 6].Value = item.OverdueDays;
				workSheet.Cells[row, 7].Value = item.OverdueFine;
				workSheet.Cells[row, 8].Value = item.Price;
				row++;
			}
			package.Save();

		}
		public void ExportDataToExcelFileRenews(string fileName, List<GET_PATRON_RENEW_LOAN_INFOR_Result> lstPatron)
		{
			string filePath = HttpContext.Current.Server.MapPath("~/Content/Template1/" + fileName);
			var package = new ExcelPackage(new FileInfo(filePath));


			ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
			for (int i = workSheet.Dimension.End.Row; i >= 3; i--)
			{
				workSheet.DeleteRow(i);
			}

			int row = 3;
			foreach (GET_PATRON_RENEW_LOAN_INFOR_Result item in lstPatron)
			{
				workSheet.Cells[row, 1].Value = item.Content;
				workSheet.Cells[row, 2].Value = item.CopyNumber;
				workSheet.Cells[row, 3].Value = item.FullName;
				workSheet.Cells[row, 4].Value = Convert.ToDateTime(item.CheckOutDate).ToString("dd/MM/yyyy");
				workSheet.Cells[row, 5].Value = Convert.ToDateTime(item.RenewDate).ToString("dd/MM/yyyy");
				workSheet.Cells[row, 6].Value = Convert.ToDateTime(item.OverDueDateNew).ToString("dd/MM/yyyy");
				workSheet.Cells[row, 7].Value = Convert.ToDateTime(item.CheckInDate).ToString("dd/MM/yyyy");
				workSheet.Cells[row, 8].Value = item.OverdueDays;
				workSheet.Cells[row, 9].Value = item.OverdueFine;
				workSheet.Cells[row, 10].Value = item.Price;
				row++;
			}
			package.Save();

		}
	}
}