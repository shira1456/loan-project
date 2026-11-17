using System;
using System.Collections.Generic;
using System.Text;

namespace App
{    public enum type
    {
        Teacher,
        Student,
        Guess
    }
    internal class Loan
    {
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
            int count=0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == "")
                    count++;
                else if(int.Parse(data[4]) >int.Parse( data[5]))
                {
                    //עדכון בקובץ שגיאות
                    return false;
                }

            }
            if (count == 6)
            {
                // עוקף שורה זה שדה ריק
                return false;
            }
            else if(count ==0)
            {
                return true;
            }
            else { //חלק תוכן ריק יש לעדכן בשגיאות
                return false; }
        }
        //אם זה יחזיר NULL הREPORT MONY לא יכניס לרשימה לכן לא בעיה שיחזיר NULL
        public List<string[,]> CalculateBuilds(List<string[]> dataOfStudentLoan,int ReportDay)
        {
            foreach(string[] data in dataOfStudentLoan)
            {
              if(!CurectData(data))
                {
                    return null;
                }
                int Return = 0;
                if (data[5] == "")//no
                {
                    Return = ReportDay;
                }
                else
                {
                    Return = int.Parse(data[5]);
                }
                switch(data[2]) {
                    case 'Teacher':
                        {

                        }
                }
                if ( int.Parse(data[5]) - Return >=)
            }
        }

    }
}
