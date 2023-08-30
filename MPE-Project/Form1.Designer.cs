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
            goButton = new Button();
            label1 = new Label();
            createRadioButton = new RadioButton();
            validateRadioButton = new RadioButton();
            partNumberComboBox = new ComboBox();
            label3 = new Label();
            label4 = new Label();
            weekNumberComboBox = new ComboBox();
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
            dateTimePicker1 = new DateTimePicker();
            offshoreCheckBox = new CheckBox();
            mxliCheckBox = new CheckBox();
            dateTimePicker2 = new DateTimePicker();
            label5 = new Label();
            label10 = new Label();
            SuspendLayout();
            // 
            // button1
            // 
            goButton.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            goButton.Location = new Point(449, 321);
            goButton.Name = "button1";
            goButton.Size = new Size(113, 61);
            goButton.TabIndex = 0;
            goButton.Text = "Run";
            goButton.UseVisualStyleBackColor = true;
            goButton.Click += Button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Calibri", 28.2F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(103, 37);
            label1.Name = "label1";
            label1.Size = new Size(406, 58);
            label1.TabIndex = 1;
            label1.Text = "MPE Report Project";
            // 
            // createButton
            // 
            createRadioButton.AutoSize = true;
            createRadioButton.Checked = true;
            createRadioButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            createRadioButton.Location = new Point(38, 131);
            createRadioButton.Name = "createButton";
            createRadioButton.Size = new Size(73, 24);
            createRadioButton.TabIndex = 6;
            createRadioButton.TabStop = true;
            createRadioButton.Text = "Create";
            createRadioButton.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            validateRadioButton.AutoSize = true;
            validateRadioButton.Enabled = false;
            validateRadioButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            validateRadioButton.Location = new Point(125, 131);
            validateRadioButton.Name = "radioButton2";
            validateRadioButton.Size = new Size(84, 24);
            validateRadioButton.TabIndex = 7;
            validateRadioButton.Text = "Validate";
            validateRadioButton.UseVisualStyleBackColor = true;
            validateRadioButton.CheckedChanged += RadioButton2_CheckedChanged;
            // 
            // comboBox1
            // 
            partNumberComboBox.DropDownHeight = 85;
            partNumberComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            partNumberComboBox.FormattingEnabled = true;
            partNumberComboBox.IntegralHeight = false;
            partNumberComboBox.Items.AddRange(new object[] { "SKY53882-26", "SKY53884-13", "SKY53885-53", "SKY58440-11", "SKY58446-11", "SKY53903-13", "SKY53904-11", "SKY55507-11", "SKY58292-16", "SKY55210-16", "SKY59724-17", "SKY78268-17" });
            partNumberComboBox.Location = new Point(84, 321);
            partNumberComboBox.MaxDropDownItems = 4;
            partNumberComboBox.Name = "comboBox1";
            partNumberComboBox.Size = new Size(119, 28);
            partNumberComboBox.TabIndex = 8;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(46, 322);
            label3.Name = "label3";
            label3.Size = new Size(28, 20);
            label3.TabIndex = 9;
            label3.Text = "PN";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label4.Location = new Point(28, 358);
            label4.Name = "label4";
            label4.Size = new Size(45, 20);
            label4.TabIndex = 11;
            label4.Text = "Week";
            // 
            // comboBox2
            // 
            weekNumberComboBox.DropDownHeight = 85;
            weekNumberComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            weekNumberComboBox.FormattingEnabled = true;
            weekNumberComboBox.IntegralHeight = false;
            weekNumberComboBox.Items.AddRange(new object[] { "Wk01", "Wk02", "Wk03", "Wk04", "Wk05", "Wk06", "Wk07", "Wk08", "Wk09", "Wk10", "Wk11", "Wk12", "Wk13", "Wk14", "Wk15", "Wk16", "Wk17", "Wk18", "Wk19", "Wk20", "Wk21", "Wk22", "Wk23", "Wk24", "Wk25", "Wk26", "Wk27", "Wk28", "Wk29", "Wk30", "Wk31", "Wk32", "Wk33", "Wk34", "Wk35", "Wk36", "Wk37", "Wk38", "Wk39", "Wk40", "Wk41", "Wk42", "Wk43", "Wk44", "Wk45", "Wk46", "Wk47", "Wk48", "Wk49", "Wk50", "Wk51", "Wk52" });
            weekNumberComboBox.Location = new Point(84, 356);
            weekNumberComboBox.MaxDropDownItems = 4;
            weekNumberComboBox.Name = "comboBox2";
            weekNumberComboBox.Size = new Size(119, 28);
            weekNumberComboBox.TabIndex = 10;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label6.Location = new Point(21, 231);
            label6.Name = "label6";
            label6.Size = new Size(93, 20);
            label6.TabIndex = 17;
            label6.Text = "Power BI File";
            // 
            // button5
            // 
            button5.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            button5.Location = new Point(512, 226);
            button5.Name = "button5";
            button5.Size = new Size(50, 29);
            button5.TabIndex = 15;
            button5.Text = "...";
            button5.UseVisualStyleBackColor = true;
            button5.Click += Button5_Click;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(117, 226);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(391, 27);
            textBox2.TabIndex = 14;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label7.Location = new Point(46, 195);
            label7.Name = "label7";
            label7.Size = new Size(65, 20);
            label7.TabIndex = 21;
            label7.Text = "MPE File";
            // 
            // button7
            // 
            button7.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            button7.Location = new Point(512, 191);
            button7.Name = "button7";
            button7.Size = new Size(50, 29);
            button7.TabIndex = 19;
            button7.Text = "...";
            button7.UseVisualStyleBackColor = true;
            button7.Click += Button7_Click;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(117, 191);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(391, 27);
            textBox3.TabIndex = 18;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Enabled = false;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(23, 266);
            label2.Name = "label2";
            label2.Size = new Size(93, 40);
            label2.TabIndex = 24;
            label2.Text = "Offshore File\r\n(Optional)";
            // 
            // button2
            // 
            button2.Enabled = false;
            button2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            button2.Location = new Point(512, 262);
            button2.Name = "button2";
            button2.Size = new Size(50, 29);
            button2.TabIndex = 23;
            button2.Text = "...";
            button2.UseVisualStyleBackColor = true;
            button2.Click += Button2_Click;
            // 
            // textBox1
            // 
            textBox1.Enabled = false;
            textBox1.Location = new Point(117, 262);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(391, 27);
            textBox1.TabIndex = 22;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI Semibold", 7.8F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point);
            label8.Location = new Point(11, 444);
            label8.Name = "label8";
            label8.Size = new Size(156, 17);
            label8.TabIndex = 25;
            label8.Text = "By: Dimas Emiliano Trejo";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI Semibold", 7.8F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point);
            label9.Location = new Point(546, 446);
            label9.Name = "label9";
            label9.Size = new Size(20, 17);
            label9.TabIndex = 26;
            label9.Text = "as";
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Enabled = false;
            dateTimePicker1.Format = DateTimePickerFormat.Short;
            dateTimePicker1.Location = new Point(285, 322);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(122, 27);
            dateTimePicker1.TabIndex = 27;
            dateTimePicker1.Visible = false;
            // 
            // offshoreCheckBox1
            // 
            offshoreCheckBox.AutoSize = true;
            offshoreCheckBox.Location = new Point(409, 161);
            offshoreCheckBox.Name = "offshoreCheckBox1";
            offshoreCheckBox.Size = new Size(88, 24);
            offshoreCheckBox.TabIndex = 28;
            offshoreCheckBox.Text = "Offshore";
            offshoreCheckBox.UseVisualStyleBackColor = true;
            offshoreCheckBox.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // mxliCheckBox2
            // 
            mxliCheckBox.AutoSize = true;
            mxliCheckBox.Checked = true;
            mxliCheckBox.CheckState = CheckState.Checked;
            mxliCheckBox.Location = new Point(503, 161);
            mxliCheckBox.Name = "mxliCheckBox2";
            mxliCheckBox.Size = new Size(59, 24);
            mxliCheckBox.TabIndex = 29;
            mxliCheckBox.Text = "Mxli";
            mxliCheckBox.UseVisualStyleBackColor = true;
            mxliCheckBox.CheckedChanged += checkBox2_CheckedChanged;
            // 
            // dateTimePicker2
            // 
            dateTimePicker2.Enabled = false;
            dateTimePicker2.Format = DateTimePickerFormat.Short;
            dateTimePicker2.Location = new Point(285, 355);
            dateTimePicker2.Name = "dateTimePicker2";
            dateTimePicker2.Size = new Size(122, 27);
            dateTimePicker2.TabIndex = 30;
            dateTimePicker2.Visible = false;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Enabled = false;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label5.Location = new Point(236, 324);
            label5.Name = "label5";
            label5.Size = new Size(46, 20);
            label5.TabIndex = 31;
            label5.Text = "From:";
            label5.Visible = false;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Enabled = false;
            label10.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label10.Location = new Point(254, 358);
            label10.Name = "label10";
            label10.Size = new Size(28, 20);
            label10.TabIndex = 32;
            label10.Text = "To:";
            label10.Visible = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(595, 472);
            Controls.Add(label10);
            Controls.Add(label5);
            Controls.Add(dateTimePicker2);
            Controls.Add(mxliCheckBox);
            Controls.Add(offshoreCheckBox);
            Controls.Add(dateTimePicker1);
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
            Controls.Add(label4);
            Controls.Add(weekNumberComboBox);
            Controls.Add(label3);
            Controls.Add(partNumberComboBox);
            Controls.Add(validateRadioButton);
            Controls.Add(createRadioButton);
            Controls.Add(label1);
            Controls.Add(goButton);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "MPE Report Project";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private Label label3;
        private Label label4;
        private Label label6;
        private Label label7;
        private Label label2;
        private Label label8;
        private Label label9;
        private DateTimePicker dateTimePicker1;
        private DateTimePicker dateTimePicker2;
        private Label label5;
        private Label label10;

        //used for access outside this class
        public static RadioButton validateRadioButton;
        public static RadioButton createRadioButton;
        public static CheckBox offshoreCheckBox;
        public static CheckBox mxliCheckBox;
        public static Button goButton;
        public static ComboBox partNumberComboBox;
        public static ComboBox weekNumberComboBox;
        public static Button button5;
        public static TextBox textBox2;
        public static Button button7;
        public static TextBox textBox3;
        public static Button button2;
        public static TextBox textBox1;
    }
}