using System.Collections.Generic;

namespace ReddStats.Core
{
    using System.IO;

    using bsparser;

    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    using ReddStats.Core.VO;

    public static class ExcelReports
    {
        public static void ExportToExcel(string filename, List<Summary> history, RelatedAccounts relatedAccounts)
        {
            using (var file = File.Open(filename, FileMode.OpenOrCreate))
            {
                var ef = new ExcelPackage(file);
                for (var i = 0; i < history.Count; i += 2)
                {
                    var excel = ef.Workbook.Worksheets.Add("Week " + ((i / 2) + 1));

                    excel.Cells["A1:C1"].Merge = true;
                    excel.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    excel.Cells["A11:C11"].Merge = true;
                    excel.Cells["A11:C11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    excel.Cells[1, 1].Value = string.Format("Week {0} (Blocks {1} - {2})", (i / 2) + 1, history[i].StartBlock, history[i].EndBlock);

                    excel.Cells["F1:H1"].Merge = true;
                    excel.Cells["F1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    excel.Cells["F11:H11"].Merge = true;
                    excel.Cells["F11:H11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    excel.Cells[1, 6].Value = string.Format("Week {0} (Total)", ((i / 2) + 1));

                    WriteSummary(excel, 2, history[i], relatedAccounts);
                    WriteSummary(excel, 7, history[i + 1], relatedAccounts);
                }

                ef.Save();
            }
        }

        private static void WriteSummary(ExcelWorksheet excel, int left, Summary history, RelatedAccounts relatedAccounts)
        {
            for (var m = 2; m < 11; m++)
            {
                excel.Cells[m, left - 1, m, left].Merge = true;
            }

            excel.Cells[2, left - 1].Value = "Total Redds";
            excel.Cells[2, left + 1].Value = history.TotalMoney;
            excel.Cells[2, left + 1].Style.Numberformat.Format = "#,##0.00;-#,##0.00";

            excel.Cells[3, left - 1].Value = "# Accounts";
            excel.Cells[3, left + 1].Value = history.TotalAddresses;
            excel.Cells[3, left + 1].Style.Numberformat.Format = "#,##0";

            excel.Cells[4, left - 1].Value = "# Active Accounts";
            excel.Cells[4, left + 1].Value = history.TotalNonZeroAddresses;
            excel.Cells[4, left + 1].Style.Numberformat.Format = "#,##0";

            excel.Cells[5, left - 1].Value = "Transactions";
            excel.Cells[5, left + 1].Value = history.TotalTransactions;
            excel.Cells[5, left + 1].Style.Numberformat.Format = "#,##0";

            excel.Cells[6, left - 1].Value = "Transactions/Block";
            excel.Cells[6, left + 1].Value = history.TotalTransactions / (history.EndBlock - history.StartBlock + 1M);
            excel.Cells[6, left + 1].Style.Numberformat.Format = "#,##0.00";

            excel.Cells[7, left - 1].Value = "Top 10 (%)";
            excel.Cells[7, left + 1].Value = string.Format("{0} ({1}%)", history.Top10Total.ToString("N0"), (history.Top10Total / history.TotalMoney * 100).ToString("N2"));
            excel.Cells[7, left + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            excel.Cells[8, left - 1].Value = "Top 100 (%)";
            excel.Cells[8, left + 1].Value = string.Format("{0} ({1}%)", history.Top100Total.ToString("N0"), (history.Top100Total / history.TotalMoney * 100).ToString("N2"));
            excel.Cells[8, left + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            excel.Cells[9, left - 1].Value = "Theil Index";
            if (history.TheilIndex > 0)
            {
                excel.Cells[9, left + 1].Value = history.TheilIndex;
                excel.Cells[9, left + 1].Style.Numberformat.Format = "#,##0.00";
            }
            else
            {
                excel.Cells[9, left + 1].Value = "N/A";
                excel.Cells[9, left + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            }

            excel.Cells[10, left - 1].Value = "Gini Index";
            excel.Cells[10, left + 1].Value = history.GniIndex;
            excel.Cells[10, left + 1].Style.Numberformat.Format = "#,##0.00";

            excel.Cells[11, left - 1].Value = "Accounts";
            excel.Cells[12, left - 1].Value = "Rank";
            excel.Cells[12, left].Value = "Address";
            excel.Cells[12, left + 1].Value = "Balance";

            var index = 0;
            for (var a = 0; a < history.TopAccounts.Count; a++)
            {
                excel.Cells[13 + index, left].Value = history.TopAccounts[a].Address;
                if (!relatedAccounts.LinkedAccountList.ContainsKey(history.TopAccounts[a].Address))
                {
                    excel.Cells[13 + index, left + 1].Value = history.TopAccounts[a].Balance;
                    excel.Cells[13 + index, left - 1].Value = a + 1;
                }
                else
                {
                    var begin = index;
                    foreach (var secondary in relatedAccounts.LinkedAccountList[history.TopAccounts[a].Address])
                    {
                        // do not list insignificant addresses
                        if (history.NonZero[secondary] >= 100)
                        {
                            index++;
                            excel.Cells[13 + index, left].Value = secondary;
                        }
                    }

                    excel.Cells[13 + begin, left + 1, 13 + index, left + 1].Merge = true;
                    excel.Cells[13 + begin, left + 1].Value = history.TopAccounts[a].Balance;

                    excel.Cells[13 + begin, left - 1, 13 + index, left - 1].Merge = true;
                    excel.Cells[13 + begin, left - 1].Value = a + 1;
                }

                index++;
            }


            excel.Cells[13, left - 1, 12 + index, left - 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            excel.Cells[13, left + 1, 12 + index, left + 1].Style.Numberformat.Format = "#,##0";
            excel.Cells[13, left + 1, 12 + index, left + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            excel.Column(left).AutoFit();
            excel.Column(left + 1).AutoFit();
        }
    }
}
