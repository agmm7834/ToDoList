using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TodoListApp
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Task { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; }

        public TodoItem(int id, string task)
        {
            Id = id;
            Task = task;
            IsCompleted = false;
            CreatedDate = DateTime.Now;
        }
    }

    public class TodoListForm : Form
    {
        private List<TodoItem> todoItems;
        private int nextId = 1;

        private TextBox txtNewTask;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnEdit;
        private Button btnToggleComplete;
        private ListBox lstTasks;
        private Label lblTitle;
        private Label lblStats;
        private CheckBox chkShowCompleted;

        public TodoListForm()
        {
            todoItems = new List<TodoItem>();
            InitializeComponents();
            UpdateStats();
        }

        private void InitializeComponents()
        {
            this.Text = "Todo List Ilovasi";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 245);

            lblTitle = new Label
            {
                Text = "📝 Mening Vazifalarim",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            this.Controls.Add(lblTitle);

            txtNewTask = new TextBox
            {
                Location = new Point(20, 70),
                Size = new Size(440, 30),
                Font = new Font("Segoe UI", 12),
                PlaceholderText = "Yangi vazifa kiriting..."
            };
            this.Controls.Add(txtNewTask);

            btnAdd = new Button
            {
                Text = "➕ Qo'shish",
                Location = new Point(470, 70),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            lstTasks = new ListBox
            {
                Location = new Point(20, 110),
                Size = new Size(550, 250),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            lstTasks.DrawMode = DrawMode.OwnerDrawFixed;
            lstTasks.ItemHeight = 35;
            lstTasks.DrawItem += LstTasks_DrawItem;
            lstTasks.DoubleClick += LstTasks_DoubleClick;
            this.Controls.Add(lstTasks);

            chkShowCompleted = new CheckBox
            {
                Text = "Bajarilganlarni ko'rsatish",
                Location = new Point(20, 370),
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                Checked = true
            };
            chkShowCompleted.CheckedChanged += ChkShowCompleted_CheckedChanged;
            this.Controls.Add(chkShowCompleted);

            btnToggleComplete = new Button
            {
                Text = "✓ Bajarildi",
                Location = new Point(20, 400),
                Size = new Size(130, 35),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnToggleComplete.FlatAppearance.BorderSize = 0;
            btnToggleComplete.Click += BtnToggleComplete_Click;
            this.Controls.Add(btnToggleComplete);

            btnEdit = new Button
            {
                Text = "✏️ Tahrirlash",
                Location = new Point(160, 400),
                Size = new Size(130, 35),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(btnEdit);

            btnDelete = new Button
            {
                Text = "🗑️ O'chirish",
                Location = new Point(300, 400),
                Size = new Size(130, 35),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            lblStats = new Label
            {
                Location = new Point(440, 405),
                Size = new Size(130, 25),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(127, 140, 141),
                TextAlign = ContentAlignment.MiddleRight
            };
            this.Controls.Add(lblStats);

            txtNewTask.KeyPress += TxtNewTask_KeyPress;
        }

        private void TxtNewTask_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnAdd_Click(sender, e);
                e.Handled = true;
            }
        }

        private void LstTasks_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var item = lstTasks.Items[e.Index] as TodoItem;
            if (item == null) return;

            e.DrawBackground();

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(52, 152, 219)), e.Bounds);
            }
            else if (item.IsCompleted)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(220, 255, 220)), e.Bounds);
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);
            }

            var textColor = (e.State & DrawItemState.Selected) == DrawItemState.Selected 
                ? Color.White 
                : (item.IsCompleted ? Color.Gray : Color.Black);

            var font = item.IsCompleted 
                ? new Font(lstTasks.Font, FontStyle.Strikeout) 
                : lstTasks.Font;

            var status = item.IsCompleted ? "✓ " : "○ ";
            e.Graphics.DrawString(
                status + item.Task,
                font,
                new SolidBrush(textColor),
                e.Bounds.X + 5,
                e.Bounds.Y + 8
            );

            e.DrawFocusRectangle();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string task = txtNewTask.Text.Trim();
            if (string.IsNullOrEmpty(task))
            {
                MessageBox.Show("Iltimos, vazifa nomini kiriting!", "Ogohlantirish", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newItem = new TodoItem(nextId++, task);
            todoItems.Add(newItem);
            txtNewTask.Clear();
            txtNewTask.Focus();
            RefreshTaskList();
            UpdateStats();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (lstTasks.SelectedItem == null)
            {
                MessageBox.Show("Iltimos, o'chirish uchun vazifa tanlang!", "Ogohlantirish",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Haqiqatan ham o'chirmoqchimisiz?", "Tasdiqlash",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var item = lstTasks.SelectedItem as TodoItem;
                todoItems.Remove(item);
                RefreshTaskList();
                UpdateStats();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (lstTasks.SelectedItem == null)
            {
                MessageBox.Show("Iltimos, tahrirlash uchun vazifa tanlang!", "Ogohlantirish",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var item = lstTasks.SelectedItem as TodoItem;
            string newTask = Microsoft.VisualBasic.Interaction.InputBox(
                "Yangi matnni kiriting:",
                "Vazifani tahrirlash",
                item.Task
            );

            if (!string.IsNullOrWhiteSpace(newTask))
            {
                item.Task = newTask;
                RefreshTaskList();
            }
        }

        private void BtnToggleComplete_Click(object sender, EventArgs e)
        {
            if (lstTasks.SelectedItem == null)
            {
                MessageBox.Show("Iltimos, vazifa tanlang!", "Ogohlantirish",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var item = lstTasks.SelectedItem as TodoItem;
            item.IsCompleted = !item.IsCompleted;
            RefreshTaskList();
            UpdateStats();
        }

        private void LstTasks_DoubleClick(object sender, EventArgs e)
        {
            BtnToggleComplete_Click(sender, e);
        }

        private void ChkShowCompleted_CheckedChanged(object sender, EventArgs e)
        {
            RefreshTaskList();
        }

        private void RefreshTaskList()
        {
            lstTasks.Items.Clear();
            var itemsToShow = chkShowCompleted.Checked 
                ? todoItems 
                : todoItems.Where(t => !t.IsCompleted);

            foreach (var item in itemsToShow)
            {
                lstTasks.Items.Add(item);
            }
        }

        private void UpdateStats()
        {
            int total = todoItems.Count;
            int completed = todoItems.Count(t => t.IsCompleted);
            lblStats.Text = $"Jami: {total} | Bajarildi: {completed}";
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TodoListForm());
        }
    }
}
