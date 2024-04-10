using System;
using System.Drawing;
using System.Windows.Forms;

namespace Kalenteri_näyttötyö
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ShowDaysBasedOnCurrentMonth();
            HighlightCurrentDay();
            CenterAllRichTextBoxes();
            SetCurrentMonthText();
        }

        private void ShowDaysBasedOnCurrentMonth()
        {
            int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

            for (int i = 1; i <= 31; i++)
            {
                if (i <= daysInMonth)
                {
                    Controls["day" + i].Visible = true;
                }
                else
                {
                    Controls["day" + i].Visible = false;
                }
            }
        }

        private void HighlightCurrentDay()
        {
            int currentDay = DateTime.Now.Day;
            RichTextBox currentRichTextBox = (RichTextBox)Controls["day" + currentDay];
            currentRichTextBox.BackColor = Color.Yellow;
        }

        private void CenterAllRichTextBoxes()
        {
            for (int i = 1; i <= 31; i++)
            {
                RichTextBox richTextBox = (RichTextBox)Controls["day" + i];
                richTextBox.SelectionAlignment = HorizontalAlignment.Center;
            }
        }

        private void SetCurrentMonthText()
        {
            string currentMonthName = DateTime.Now.ToString("MMMM");
            currentMonth.Text = currentMonthName;
        }
    }
}
