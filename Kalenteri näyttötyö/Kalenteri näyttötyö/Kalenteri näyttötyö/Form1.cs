using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Kalenteri_näyttötyö
{
    public partial class kalenteri : Form
    {
        private Timer timer; // Ajastin päivämäärän ja kellonajan päivittämiseen
        private DateTime currentDate; // Nykyinen päivämäärä
        private string[][] dayContents; // Taulukko päivien muistiinpanoille
        private string notesFilePath = "day_notes.txt"; // Tiedostopolku muistiinpanojen tallentamiseen

        public kalenteri()
        {
            InitializeComponent();
            currentDate = DateTime.Now; // Asetetaan nykyinen päivämäärä
            dayContents = new string[12][]; // Tehdään taulukko, joka tallentaa päivien muistiinpanot eri kuukausille

            // Alustetaan alitaulukot
            for (int i = 0; i < 12; i++)
            {
                dayContents[i] = new string[31]; // Alustetaan taulukko, joka tallentaa päivien muistiinpanot yhdelle kuukaudelle
            }

            // Ladataan päivän muistiinpanot tiedostosta
            LoadDayNotesFromFile();

            // Tehdään käyttöliittymä ja lisätään tapahtumankäsittelijät
            ShowDaysBasedOnCurrentMonth();
            HighlightCurrentDay();
            CenterAllRichTextBoxes();
            SetCurrentMonthText();
            CreateMonthNavigationButtons();

            // Tehdään ajastin päivämäärän ja kellonajan päivittämiseen
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();

            // Lisätään tapahtumankäsittelijä lomakkeen sulkemiseen
            this.FormClosing += kalenteri_FormClosing;
        }

        // Ajastimen tikin käsittelijä
        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateCurrentTime();
        }

        // Päivittää nykyisen ajan näytön
        private void UpdateCurrentTime()
        {
            currentTime.Text = DateTime.Now.ToString("H:mm");
        }

        // Näyttää päivät kuukauden mukaisesti
        private void ShowDaysBasedOnCurrentMonth()
        {
            int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

            int maxVisibleDay = Math.Min(daysInMonth, 31);

            // Piilottaa päivät, jotka eivät kuulu nykyiseen kuukauteen
            for (int i = 1; i <= 31; i++)
            {
                if (i <= maxVisibleDay)
                {
                    Controls["day" + i].Visible = true;
                }
                else
                {
                    Controls["day" + i].Visible = false;
                }
            }
        }

        // Korostaa nykyisen päivän
        private void HighlightCurrentDay()
        {
            ClearHighlight(); // Poistaa vanhan korostuksen

            int currentDay = DateTime.Now.Day;
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            // Korostaa nykyisen päivän
            if (Controls.ContainsKey("day" + currentDay) && currentDate.Month == currentMonth && currentDate.Year == currentYear)
            {
                RichTextBox currentRichTextBox = (RichTextBox)Controls["day" + currentDay];

                int padding = 3;
                int squareSize = currentRichTextBox.Height + padding;
                int left = currentRichTextBox.Left - padding / 2;
                int top = currentRichTextBox.Top - padding / 2;

                Panel highlightPanel = new Panel();
                highlightPanel.BackColor = Color.Yellow;
                highlightPanel.Location = new Point(left, top);
                highlightPanel.Size = new Size(squareSize, squareSize);
                this.Controls.Add(highlightPanel);
                highlightPanel.SendToBack();
            }
        }

        // Poistaa korostuksen
        private void ClearHighlight()
        {
            foreach (Control control in this.Controls)
            {
                if (control is Panel && control.BackColor == Color.Yellow)
                {
                    this.Controls.Remove(control);
                    control.Dispose();
                    break;
                }
            }
        }

        // Keskittää kaikki RichTextBoxit
        private void CenterAllRichTextBoxes()
        {
            for (int i = 1; i <= 31; i++)
            {
                if (Controls.ContainsKey("day" + i))
                {
                    RichTextBox richTextBox = (RichTextBox)Controls["day" + i];
                    richTextBox.SelectionAlignment = HorizontalAlignment.Center;
                    richTextBox.ReadOnly = true; // Estää muokkaamisen
                    richTextBox.Multiline = true; // Mahdollistaa usean rivin tekstinsyötön
                    richTextBox.MouseDoubleClick += day_MouseDoubleClick; // Lisää käsittelijän tuplaklikkaukselle
                }
            }
        }

        // Asettaa nykyisen kuukauden tekstin näytölle
        private void SetCurrentMonthText()
        {
            string currentMonthName = currentDate.ToString("yyyy MMMM");
            currentMonth.Text = currentMonthName;
        }

        // Luo navigointinapit edellisen ja seuraavan kuukauden vaihtamiseksi
        private void CreateMonthNavigationButtons()
        {
            // Create previous month arrow button
            Button previousMonthButton = new Button();
            previousMonthButton.Font = new Font("Arial", 26, FontStyle.Bold);
            previousMonthButton.Text = "←";
            previousMonthButton.FlatStyle = FlatStyle.Flat;
            previousMonthButton.FlatAppearance.BorderSize = 0;
            previousMonthButton.FlatAppearance.MouseOverBackColor = Color.Transparent; // Remove hovering highlight
            previousMonthButton.FlatAppearance.MouseDownBackColor = Color.Transparent; // Remove click highlight
            previousMonthButton.ForeColor = Color.White;
            previousMonthButton.BackColor = Color.Transparent;
            previousMonthButton.Size = new Size(40, 40);
            previousMonthButton.Location = new Point(currentMonth.Left - 45, currentMonth.Top);
            previousMonthButton.Click += PreviousMonthButton_Click;
            this.Controls.Add(previousMonthButton);

            // Create next month arrow button
            Button nextMonthButton = new Button();
            nextMonthButton.Font = new Font("Arial", 26, FontStyle.Bold);
            nextMonthButton.Text = "→";
            nextMonthButton.FlatStyle = FlatStyle.Flat;
            nextMonthButton.FlatAppearance.BorderSize = 0;
            nextMonthButton.FlatAppearance.MouseOverBackColor = Color.Transparent; // Remove hovering highlight
            nextMonthButton.FlatAppearance.MouseDownBackColor = Color.Transparent; // Remove click highlight
            nextMonthButton.ForeColor = Color.White;
            nextMonthButton.BackColor = Color.Transparent;
            nextMonthButton.Size = new Size(40, 40);
            nextMonthButton.Location = new Point(currentMonth.Right + 10, currentMonth.Top);
            nextMonthButton.Click += NextMonthButton_Click;
            this.Controls.Add(nextMonthButton);
        }

        // Edellisen kuukauden nappulan klikkaus käsittelijä
        private void PreviousMonthButton_Click(object sender, EventArgs e)
        {
            currentDate = currentDate.AddMonths(-1);
            SetMonth(currentDate);
        }

        // Seuraavan kuukauden nappulan klikkaus käsittelijä
        private void NextMonthButton_Click(object sender, EventArgs e)
        {
            currentDate = currentDate.AddMonths(1);
            SetMonth(currentDate);
        }

        // Asettaa kuukauden
        private void SetMonth(DateTime month)
        {
            SetCurrentMonthText();
            ShowDaysBasedOnCurrentMonth();
            HighlightCurrentDay();
        }

        // Tuplaklikkauksen käsittelijä päiville
        private void day_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RichTextBox clickedDay = sender as RichTextBox;
            if (clickedDay != null)
            {
                int day = int.Parse(clickedDay.Name.Replace("day", ""));
                DateTime selectedDate = new DateTime(currentDate.Year, currentDate.Month, day);

                // Avaa ikkuna muistion kirjoittamiseen ja välittää päivän tekstilaatikko parametrina
                string content = dayContents[currentDate.Month - 1][day - 1]; // Haetaan sisältö taulukosta nykyisen kuukauden osalta
                string newContent = Prompt.ShowDialog("Syötä muistiinpano", content, clickedDay);

                // Tallenna päivän sisältö taulukkoon nykyisen kuukauden osalta
                dayContents[currentDate.Month - 1][day - 1] = newContent;
            }
        }

        // Kirjoittaa päivän muistiinpanot tiedostoon ennen lomakkeen sulkemista
        private void kalenteri_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveDayNotesToFile();
        }

        // Luo muistion kirjoittamisen ikkunan
        public static class Prompt
        {
            public static string ShowDialog(string caption, string text, RichTextBox dayBox)
            {
                Form prompt = new Form();
                prompt.Width = 300;
                prompt.Height = 220; // Korotettu korkeus, jotta tekstin syöttöön on enemmän tilaa
                prompt.Text = caption;
                Label textLabel = new Label() { Left = 50, Top = 20, Text = "Syötä muistiinpano:" }; // Päivitetty teksti
                TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 200, Height = 80, Multiline = true }; // Lisätty Multiline ja korotettu korkeus
                textBox.Text = text;
                Button confirmation = new Button() { Text = "OK", Left = 50, Width = 100, Top = 140 }; // Päivitetty sijainti
                confirmation.Click += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(textBox.Text))
                    {
                        // Lisää merkki päivän tekstilaatikkoon, jos muistiinpanolaatikossa on tekstiä
                        if (!dayBox.Text.Contains("✔"))
                        {
                            // Insert the checkmark at the end of the existing text
                            dayBox.SelectionStart = dayBox.TextLength;
                            dayBox.SelectedText = "✔";
                        }
                    }
                    else
                    {
                        // Poistaa merkin päivän tekstilaatikosta, jos se on jo lisätty
                        if (dayBox.Text.Contains("✔"))
                        {
                            int index = dayBox.Text.IndexOf("✔");
                            dayBox.Select(index, 1);
                            dayBox.Text = dayBox.Text.Replace("✔", ""); // Poistaa merkin päivän tekstilaatikosta
                            dayBox.SelectionAlignment = HorizontalAlignment.Center;
                        }
                    }
                    prompt.Close();
                };
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(textBox);
                prompt.ShowDialog();
                return textBox.Text;
            }
        }

        // Lataa päivän muistiinpanot tiedostosta
        private void LoadDayNotesFromFile()
        {
            try
            {
                if (File.Exists(notesFilePath))
                {
                    using (StreamReader sr = new StreamReader(notesFilePath))
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            for (int j = 0; j < 31; j++)
                            {
                                string note = sr.ReadLine();
                                dayContents[i][j] = note;

                                // Add checkmark to the RichTextBox if note is not empty
                                if (!string.IsNullOrEmpty(note) && Controls.ContainsKey("day" + (j + 1)))
                                {
                                    RichTextBox richTextBox = (RichTextBox)Controls["day" + (j + 1)];
                                    richTextBox.Text = richTextBox.Text + " ✔";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading day notes: " + ex.Message);
            }
        }


        // Tallentaa päivän muistiinpanot tiedostoon
        private void SaveDayNotesToFile()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(notesFilePath))
                {
                    for (int i = 0; i < 12; i++)
                    {
                        for (int j = 0; j < 31; j++)
                        {
                            sw.WriteLine(dayContents[i][j]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving day notes: " + ex.Message);
            }
        }
    }
}
