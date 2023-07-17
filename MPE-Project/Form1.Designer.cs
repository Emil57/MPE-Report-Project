namespace MPE_Project
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            button1 = new Button();
            label1 = new Label();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            comboBox1 = new ComboBox();
            label3 = new Label();
            label4 = new Label();
            comboBox2 = new ComboBox();
            label5 = new Label();
            comboBox3 = new ComboBox();
            label6 = new Label();
            button5 = new Button();
            textBox2 = new TextBox();
            label7 = new Label();
            button7 = new Button();
            textBox3 = new TextBox();
            label2 = new Label();
            button2 = new Button();
            textBox1 = new TextBox();
            label8 = new Label();
            label9 = new Label();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            button1.Location = new Point(319, 242);
            button1.Margin = new Padding(3, 2, 3, 2);
            button1.Name = "button1";
            button1.Size = new Size(99, 46);
            button1.TabIndex = 0;
            button1.Text = "Run";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Calibri", 28.2F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(90, 28);
            label1.Name = "label1";
            label1.Size = new Size(328, 46);
            label1.TabIndex = 1;
            label1.Text = "MPE Report Project";
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Checked = true;
            radioButton1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            radioButton1.Location = new Point(33, 98);
            radioButton1.Margin = new Padding(3, 2, 3, 2);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(59, 19);
            radioButton1.TabIndex = 6;
            radioButton1.TabStop = true;
            radioButton1.Text = "Create";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Enabled = false;
            radioButton2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            radioButton2.Location = new Point(109, 98);
            radioButton2.Margin = new Padding(3, 2, 3, 2);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(66, 19);
            radioButton2.TabIndex = 7;
            radioButton2.Text = "Validate";
            radioButton2.UseVisualStyleBackColor = true;
            radioButton2.CheckedChanged += RadioButton2_CheckedChanged;
            // 
            // comboBox1
            // 
            comboBox1.DropDownHeight = 85;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.IntegralHeight = false;
            comboBox1.Items.AddRange(new object[] { "SKY53882-26", "SKY53884-13", "SKY53885-53", "SKY58440-11", "SKY58446-11", "SKY53903-13", "SKY53904-11", "SKY55507-11", "SKY58292-16", "SKY55210-16", "SKY59724-17", "SKY78268-17" });
            comboBox1.Location = new Point(115, 231);
            comboBox1.Margin = new Padding(3, 2, 3, 2);
            comboBox1.MaxDropDownItems = 4;
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(105, 23);
            comboBox1.TabIndex = 8;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(81, 232);
            label3.Name = "label3";
            label3.Size = new Size(23, 15);
            label3.TabIndex = 9;
            label3.Text = "PN";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label4.Location = new Point(66, 259);
            label4.Name = "label4";
            label4.Size = new Size(36, 15);
            label4.TabIndex = 11;
            label4.Text = "Week";
            // 
            // comboBox2
            // 
            comboBox2.DropDownHeight = 85;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.FormattingEnabled = true;
            comboBox2.IntegralHeight = false;
            comboBox2.Items.AddRange(new object[] { "Wk01", "Wk02", "Wk03", "Wk04", "Wk05", "Wk06", "Wk07", "Wk08", "Wk09", "Wk10", "Wk11", "Wk12", "Wk13", "Wk14", "Wk15", "Wk16", "Wk17", "Wk18", "Wk19", "Wk20", "Wk21", "Wk22", "Wk23", "Wk24", "Wk25", "Wk26", "Wk27", "Wk28", "Wk29", "Wk30", "Wk31", "Wk32", "Wk33", "Wk34", "Wk35", "Wk36", "Wk37", "Wk38", "Wk39", "Wk40", "Wk41", "Wk42", "Wk43", "Wk44", "Wk45", "Wk46", "Wk47", "Wk48", "Wk49", "Wk50", "Wk51", "Wk52" });
            comboBox2.Location = new Point(115, 257);
            comboBox2.Margin = new Padding(3, 2, 3, 2);
            comboBox2.MaxDropDownItems = 4;
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(105, 23);
            comboBox2.TabIndex = 10;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Enabled = false;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label5.Location = new Point(81, 284);
            label5.Name = "label5";
            label5.Size = new Size(20, 15);
            label5.TabIndex = 13;
            label5.Text = "FY";
            // 
            // comboBox3
            // 
            comboBox3.DropDownHeight = 85;
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.Enabled = false;
            comboBox3.FormattingEnabled = true;
            comboBox3.IntegralHeight = false;
            comboBox3.ItemHeight = 15;
            comboBox3.Items.AddRange(new object[] { "FY2022", "FY2023", "FY2024", "FY2025" });
            comboBox3.Location = new Point(115, 284);
            comboBox3.Margin = new Padding(3, 2, 3, 2);
            comboBox3.MaxDropDownItems = 4;
            comboBox3.Name = "comboBox3";
            comboBox3.Size = new Size(105, 23);
            comboBox3.TabIndex = 12;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label6.Location = new Point(20, 160);
            label6.Name = "label6";
            label6.Size = new Size(74, 15);
            label6.TabIndex = 17;
            label6.Text = "Power BI File";
            // 
            // button5
            // 
            button5.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            button5.Location = new Point(450, 156);
            button5.Margin = new Padding(3, 2, 3, 2);
            button5.Name = "button5";
            button5.Size = new Size(43, 22);
            button5.TabIndex = 15;
            button5.Text = "...";
            button5.UseVisualStyleBackColor = true;
            button5.Click += Button5_Click;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(104, 156);
            textBox2.Margin = new Padding(3, 2, 3, 2);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(342, 23);
            textBox2.TabIndex = 14;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label7.Location = new Point(42, 133);
            label7.Name = "label7";
            label7.Size = new Size(52, 15);
            label7.TabIndex = 21;
            label7.Text = "MPE File";
            // 
            // button7
            // 
            button7.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            button7.Location = new Point(450, 130);
            button7.Margin = new Padding(3, 2, 3, 2);
            button7.Name = "button7";
            button7.Size = new Size(43, 22);
            button7.TabIndex = 19;
            button7.Text = "...";
            button7.UseVisualStyleBackColor = true;
            button7.Click += Button7_Click;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(104, 130);
            textBox3.Margin = new Padding(3, 2, 3, 2);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(342, 23);
            textBox3.TabIndex = 18;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Enabled = false;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(22, 186);
            label2.Name = "label2";
            label2.Size = new Size(70, 30);
            label2.TabIndex = 24;
            label2.Text = "Ofshore File\r\n(Optional)";
            // 
            // button2
            // 
            button2.Enabled = false;
            button2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            button2.Location = new Point(450, 183);
            button2.Margin = new Padding(3, 2, 3, 2);
            button2.Name = "button2";
            button2.Size = new Size(43, 22);
            button2.TabIndex = 23;
            button2.Text = "...";
            button2.UseVisualStyleBackColor = true;
            button2.Click += Button2_Click;
            // 
            // textBox1
            // 
            textBox1.Enabled = false;
            textBox1.Location = new Point(104, 183);
            textBox1.Margin = new Padding(3, 2, 3, 2);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(342, 23);
            textBox1.TabIndex = 22;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI Semibold", 7.8F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point);
            label8.Location = new Point(7, 322);
            label8.Name = "label8";
            label8.Size = new Size(132, 13);
            label8.TabIndex = 25;
            label8.Text = "By: Dimas Emiliano Trejo";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI Semibold", 7.8F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point);
            label9.Location = new Point(475, 323);
            label9.Name = "label9";
            label9.Size = new Size(18, 13);
            label9.TabIndex = 26;
            label9.Text = "as";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(525, 343);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(label2);
            Controls.Add(button2);
            Controls.Add(textBox1);
            Controls.Add(label7);
            Controls.Add(button7);
            Controls.Add(textBox3);
            Controls.Add(label6);
            Controls.Add(button5);
            Controls.Add(textBox2);
            Controls.Add(label5);
            Controls.Add(comboBox3);
            Controls.Add(label4);
            Controls.Add(comboBox2);
            Controls.Add(label3);
            Controls.Add(comboBox1);
            Controls.Add(radioButton2);
            Controls.Add(radioButton1);
            Controls.Add(label1);
            Controls.Add(button1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            Text = "MPE Report Project";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label label1;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private ComboBox comboBox1;
        private Label label3;
        private Label label4;
        private ComboBox comboBox2;
        private Label label5;
        private ComboBox comboBox3;
        private Label label6;
        private Button button5;
        private TextBox textBox2;
        private Label label7;
        private Button button7;
        private TextBox textBox3;
        private Label label2;
        private Button button2;
        private TextBox textBox1;
        private Label label8;
        private Label label9;
    }
}