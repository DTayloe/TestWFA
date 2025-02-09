namespace TestWFA
{
     partial class TaskTimeControl
     {
          /// <summary> 
          /// Required designer variable.
          /// </summary>
          private System.ComponentModel.IContainer components = null;

          /// <summary> 
          /// Clean up any resources being used.
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

          #region Component Designer generated code

          /// <summary> 
          /// Required method for Designer support - do not modify 
          /// the contents of this method with the code editor.
          /// </summary>
          private void InitializeComponent()
          {
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.cbTaskTimeMode = new System.Windows.Forms.ComboBox();
            this.tblTimer = new System.Windows.Forms.TableLayoutPanel();
            this.linkLabel9 = new System.Windows.Forms.LinkLabel();
            this.linkLabel5 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.lblDigitHours = new System.Windows.Forms.LinkLabel();
            this.lblDigitDays = new System.Windows.Forms.LinkLabel();
            this.lblDigitUnit = new System.Windows.Forms.LinkLabel();
            this.lblDigitSeconds = new System.Windows.Forms.LinkLabel();
            this.lblDigitMinutes = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel5.SuspendLayout();
            this.tblTimer.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.AutoSize = true;
            this.tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.cbTaskTimeMode, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.tblTimer, 0, 0);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.Size = new System.Drawing.Size(257, 88);
            this.tableLayoutPanel5.TabIndex = 19;
            // 
            // cbTaskTimeMode
            // 
            this.cbTaskTimeMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbTaskTimeMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTaskTimeMode.FormattingEnabled = true;
            this.cbTaskTimeMode.Location = new System.Drawing.Point(3, 64);
            this.cbTaskTimeMode.Name = "cbTaskTimeMode";
            this.cbTaskTimeMode.Size = new System.Drawing.Size(251, 21);
            this.cbTaskTimeMode.TabIndex = 0;
            // 
            // tblTimer
            // 
            this.tblTimer.AutoSize = true;
            this.tblTimer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblTimer.ColumnCount = 4;
            this.tblTimer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblTimer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblTimer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblTimer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblTimer.Controls.Add(this.linkLabel9, 2, 1);
            this.tblTimer.Controls.Add(this.linkLabel5, 1, 1);
            this.tblTimer.Controls.Add(this.linkLabel3, 0, 1);
            this.tblTimer.Controls.Add(this.lblDigitHours, 1, 0);
            this.tblTimer.Controls.Add(this.lblDigitDays, 0, 0);
            this.tblTimer.Controls.Add(this.lblDigitUnit, 3, 1);
            this.tblTimer.Controls.Add(this.lblDigitSeconds, 3, 0);
            this.tblTimer.Controls.Add(this.lblDigitMinutes, 2, 0);
            this.tblTimer.Location = new System.Drawing.Point(3, 3);
            this.tblTimer.Name = "tblTimer";
            this.tblTimer.RowCount = 2;
            this.tblTimer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblTimer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblTimer.Size = new System.Drawing.Size(250, 55);
            this.tblTimer.TabIndex = 17;
            // 
            // linkLabel9
            // 
            this.linkLabel9.AutoSize = true;
            this.linkLabel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkLabel9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel9.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabel9.LinkColor = System.Drawing.Color.Black;
            this.linkLabel9.Location = new System.Drawing.Point(125, 39);
            this.linkLabel9.Name = "linkLabel9";
            this.linkLabel9.Size = new System.Drawing.Size(55, 16);
            this.linkLabel9.TabIndex = 6;
            this.linkLabel9.TabStop = true;
            this.linkLabel9.Text = "Minutes";
            this.linkLabel9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // linkLabel5
            // 
            this.linkLabel5.AutoSize = true;
            this.linkLabel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkLabel5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel5.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabel5.LinkColor = System.Drawing.Color.Black;
            this.linkLabel5.Location = new System.Drawing.Point(64, 39);
            this.linkLabel5.Name = "linkLabel5";
            this.linkLabel5.Size = new System.Drawing.Size(55, 16);
            this.linkLabel5.TabIndex = 6;
            this.linkLabel5.TabStop = true;
            this.linkLabel5.Text = "Hours";
            this.linkLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel3.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabel3.LinkColor = System.Drawing.Color.Black;
            this.linkLabel3.Location = new System.Drawing.Point(3, 39);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(55, 16);
            this.linkLabel3.TabIndex = 6;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "Days";
            this.linkLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDigitHours
            // 
            this.lblDigitHours.AutoSize = true;
            this.lblDigitHours.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDigitHours.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDigitHours.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lblDigitHours.LinkColor = System.Drawing.Color.Black;
            this.lblDigitHours.Location = new System.Drawing.Point(64, 0);
            this.lblDigitHours.Name = "lblDigitHours";
            this.lblDigitHours.Size = new System.Drawing.Size(55, 39);
            this.lblDigitHours.TabIndex = 3;
            this.lblDigitHours.TabStop = true;
            this.lblDigitHours.Text = "99";
            this.lblDigitHours.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDigitDays
            // 
            this.lblDigitDays.AutoSize = true;
            this.lblDigitDays.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDigitDays.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDigitDays.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lblDigitDays.LinkColor = System.Drawing.Color.Black;
            this.lblDigitDays.Location = new System.Drawing.Point(3, 0);
            this.lblDigitDays.Name = "lblDigitDays";
            this.lblDigitDays.Size = new System.Drawing.Size(55, 39);
            this.lblDigitDays.TabIndex = 3;
            this.lblDigitDays.TabStop = true;
            this.lblDigitDays.Text = "99";
            this.lblDigitDays.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDigitUnit
            // 
            this.lblDigitUnit.AutoSize = true;
            this.lblDigitUnit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDigitUnit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDigitUnit.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lblDigitUnit.LinkColor = System.Drawing.Color.Black;
            this.lblDigitUnit.Location = new System.Drawing.Point(186, 39);
            this.lblDigitUnit.Name = "lblDigitUnit";
            this.lblDigitUnit.Size = new System.Drawing.Size(61, 16);
            this.lblDigitUnit.TabIndex = 6;
            this.lblDigitUnit.TabStop = true;
            this.lblDigitUnit.Text = "Seconds";
            this.lblDigitUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDigitSeconds
            // 
            this.lblDigitSeconds.AutoSize = true;
            this.lblDigitSeconds.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDigitSeconds.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDigitSeconds.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lblDigitSeconds.LinkColor = System.Drawing.Color.Black;
            this.lblDigitSeconds.Location = new System.Drawing.Point(186, 0);
            this.lblDigitSeconds.Name = "lblDigitSeconds";
            this.lblDigitSeconds.Size = new System.Drawing.Size(61, 39);
            this.lblDigitSeconds.TabIndex = 3;
            this.lblDigitSeconds.TabStop = true;
            this.lblDigitSeconds.Text = "99";
            this.lblDigitSeconds.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDigitMinutes
            // 
            this.lblDigitMinutes.AutoSize = true;
            this.lblDigitMinutes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDigitMinutes.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDigitMinutes.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lblDigitMinutes.LinkColor = System.Drawing.Color.Black;
            this.lblDigitMinutes.Location = new System.Drawing.Point(125, 0);
            this.lblDigitMinutes.Name = "lblDigitMinutes";
            this.lblDigitMinutes.Size = new System.Drawing.Size(55, 39);
            this.lblDigitMinutes.TabIndex = 3;
            this.lblDigitMinutes.TabStop = true;
            this.lblDigitMinutes.Text = "99";
            this.lblDigitMinutes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TaskTimeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel5);
            this.Name = "TaskTimeControl";
            this.Size = new System.Drawing.Size(260, 91);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tblTimer.ResumeLayout(false);
            this.tblTimer.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

          }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.ComboBox cbTaskTimeMode;
        private System.Windows.Forms.TableLayoutPanel tblTimer;
        private System.Windows.Forms.LinkLabel linkLabel9;
        private System.Windows.Forms.LinkLabel linkLabel5;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.LinkLabel lblDigitHours;
        private System.Windows.Forms.LinkLabel lblDigitDays;
        private System.Windows.Forms.LinkLabel lblDigitUnit;
        private System.Windows.Forms.LinkLabel lblDigitSeconds;
        private System.Windows.Forms.LinkLabel lblDigitMinutes;
    }
}
