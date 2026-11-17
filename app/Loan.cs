using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace App
{    public enum type
    {
        Teacher,
        Student,
        Guess
    }
    internal class Loan
    {
        public List<(string Book, int Count)> TopBooks { get; private set; } = new List<(string, int)>();
        public List<(string Id, string Name)> AtRiskReaders { get; private set; } = new List<(string, string)>();

        //reader_id,name,type,book,loan_day,return_day
        public string Id { get; set; }
        public string Name { get; set; }
        public type Usertype { get; set; }
        public string BookName { get; set; }
        public int Loan_day { get; set; }
       public int  Return_day { get; set; }
        public Loan (string id,string name,type typeUser,string bookName,int loan_day)
        {
            Id= id;
            Name= name;
            Usertype = typeUser;
            BookName = bookName;
            Loan_day= loan_day;
       
        }
        //בודק תקינות ומחליט אם לשלוח לשגיאות או לעקוף כי ריק
        private bool CurectData(string[] data)
        {
            if (data == null || data.Length != 6)
                return false;

            for (int i = 0; i < 5; i++)
            {
                if (string.IsNullOrWhiteSpace(data[i]))
                    return false;
            }

            if (!int.TryParse(data[4], out int loanDay))
                return false;

            if (!string.IsNullOrWhiteSpace(data[5]) &&
                (!int.TryParse(data[5], out int returnDay) || loanDay > returnDay))
                return false;

            string userType = data[2].Trim().ToLowerInvariant();
            return userType == "student" || userType == "teacher" || userType == "guest";
        }

        public bool TryCalculateLoan(string[] data, int reportDay, out decimal fine, out bool flagged)
        {
            fine = 0;
            flagged = false;

            if (!CurectData(data))
                return false;

            int loanDay = int.Parse(data[4]);
            bool returned = !string.IsNullOrWhiteSpace(data[5]);
            int returnDay = returned ? int.Parse(data[5]) : reportDay;
            int loanDays = returnDay - loanDay + 1;
            string userType = data[2].Trim().ToLowerInvariant();

            if (userType == "student")
            {
                int chargeableDays = Math.Max(0, loanDays - 15);
                fine = Math.Min(30m, chargeableDays);
            }
            else if (userType == "guest")
            {
                int chargeableDays = Math.Max(0, loanDays - 7);
                fine = decimal.Ceiling(chargeableDays * 2.5m);
            }
            else if (!returned && loanDays > 60)
            {
                flagged = true;
            }

            return true;
        }
        //אם זה יחזיר NULL הREPORT MONY לא יכניס לרשימה לכן לא בעיה שיחזיר NULL
        public List<string[,]> CalculateBuilds(List<string[]> dataOfStudentLoan,int ReportDay)
        {
            List<string[,]> answers = new List<string[,]>();
            Dictionary<string, int> bookCounts = new Dictionary<string, int>();
            Dictionary<string, (string Name, int PositiveLoans)> readerPositiveLoans = new Dictionary<string, (string, int)>();
            TopBooks = new List<(string, int)>();
            AtRiskReaders = new List<(string, string)>();

            if (dataOfStudentLoan == null)
                return answers;

            foreach (string[] data in dataOfStudentLoan)
            {
                if (!TryCalculateLoan(data, ReportDay, out decimal fine, out _))
                    continue;

                string[,] answer = new string[1, 3];
                answer[0, 0] = data[0];
                answer[0, 1] = data[1];
                answer[0, 2] = fine.ToString("0.##", CultureInfo.InvariantCulture);
                answers.Add(answer);

                bookCounts[data[3]] = bookCounts.TryGetValue(data[3], out int bookCount) ? bookCount + 1 : 1;

                if (readerPositiveLoans.TryGetValue(data[0], out (string Name, int PositiveLoans) reader))
                    readerPositiveLoans[data[0]] = (reader.Name, reader.PositiveLoans + (fine > 0 ? 1 : 0));
                else
                    readerPositiveLoans[data[0]] = (data[1], fine > 0 ? 1 : 0);
            }

            TopBooks = bookCounts
                .OrderByDescending(item => item.Value)
                .ThenBy(item => item.Key)
                .Take(3)
                .Select(item => (item.Key, item.Value))
                .ToList();

            AtRiskReaders = readerPositiveLoans
                .Where(reader => reader.Value.PositiveLoans > 2)
                .Select(reader => (reader.Key, reader.Value.Name))
                .ToList();

            return answers;
        }

    }
}
