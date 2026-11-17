using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace App
{
    internal class ReportOfMoney
    {
        public int day { get; set; }
        public int SumOf { get; set; }
        public ReportOfMoney() { }

        public void CreateReport(List<string[]> Data, int reportDay)
        {
            List<string> rawRows = Data == null
                ? new List<string>()
                : Data.Select(row => string.Join(",", row)).ToList();
            CreateReport(Data, rawRows, reportDay);
        }

        public void CreateReport(List<string[]> data, List<string> rawRows, int reportDay)
        {
            Loan loanCalculator = new Loan("", "", type.Student, "", 0);
            loanCalculator.CalculateBuilds(data, reportDay);

            Dictionary<string, (string Name, decimal Fine)> fines = new Dictionary<string, (string, decimal)>();
            List<string> readerOrder = new List<string>();
            List<(string Id, string Name)> flagged = new List<(string, string)>();
            HashSet<string> flaggedIds = new HashSet<string>();
            List<string> errors = new List<string>();

            for (int i = 0; i < data.Count; i++)
            {
                string[] row = data[i];
                if (!loanCalculator.TryCalculateLoan(row, reportDay, out decimal fine, out bool isFlagged))
                {
                    errors.Add(rawRows[i]);
                    continue;
                }

                if (!fines.ContainsKey(row[0]))
                {
                    fines[row[0]] = (row[1], 0);
                    readerOrder.Add(row[0]);
                }

                fines[row[0]] = (fines[row[0]].Name, fines[row[0]].Fine + fine);

                if (isFlagged && flaggedIds.Add(row[0]))
                    flagged.Add((row[0], row[1]));
            }

            decimal total = fines.Values.Sum(value => value.Fine);
            SumOf = (int)total;
            day = reportDay;

            Console.WriteLine("FINES");
            Console.WriteLine("reader_id,name,fine");
            foreach (string readerId in readerOrder)
            {
                (string name, decimal fine) = fines[readerId];
                Console.WriteLine(readerId + "," + name + "," + FormatMoney(fine));
            }

            Console.WriteLine("FLAGGED");
            Console.WriteLine("reader_id,name");
            foreach ((string id, string name) in flagged)
                Console.WriteLine(id + "," + name);

            Console.WriteLine("ERRORS");
            foreach (string error in errors)
                Console.WriteLine(error);

            Console.WriteLine("TOTAL," + FormatMoney(total));

            Console.WriteLine("TOP_BOOKS");
            Console.WriteLine("book,count");
            foreach ((string book, int count) in loanCalculator.TopBooks)
                Console.WriteLine(book + "," + count);

            Console.WriteLine("AT_RISK");
            Console.WriteLine("reader_id,name");
            foreach ((string id, string name) in loanCalculator.AtRiskReaders)
                Console.WriteLine(id + "," + name);
        }

        private static string FormatMoney(decimal amount)
        {
            return amount.ToString("0.##", CultureInfo.InvariantCulture);
        }
    }
}
