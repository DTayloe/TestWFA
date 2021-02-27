namespace TestWFA
{
     partial class TaskViewSettingsView
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

          #region Windows Form Designer generated code

          /// <summary>
          /// Required method for Designer support - do not modify
          /// the contents of this method with the code editor.
          /// </summary>
          private void InitializeComponent()
          {
               this.cbEnableParallelTasks = new System.Windows.Forms.CheckBox();
               this.clbSettings = new System.Windows.Forms.CheckedListBox();
               this.SuspendLayout();
               // 
               // cbEnableParallelTasks
               // 
               this.cbEnableParallelTasks.AutoSize = true;
               this.cbEnableParallelTasks.Location = new System.Drawing.Point(40, 45);
               this.cbEnableParallelTasks.Name = "cbEnableParallelTasks";
               this.cbEnableParallelTasks.Size = new System.Drawing.Size(163, 17);
               this.cbEnableParallelTasks.TabIndex = 2;
               this.cbEnableParallelTasks.Text = "Enable multiple running tasks";
               this.cbEnableParallelTasks.UseVisualStyleBackColor = true;
               // 
               // clbSettings
               // 
               this.clbSettings.CheckOnClick = true;
               this.clbSettings.FormattingEnabled = true;
               this.clbSettings.Items.AddRange(new object[] {
            "Blank",
            "Blank",
            "Blank"});
               this.clbSettings.Location = new System.Drawing.Point(103, 82);
               this.clbSettings.Name = "clbSettings";
               this.clbSettings.Size = new System.Drawing.Size(420, 274);
               this.clbSettings.TabIndex = 3;
               // 
               // TaskViewSettingsView
               // 
               this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
               this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
               this.ClientSize = new System.Drawing.Size(800, 450);
               this.Controls.Add(this.clbSettings);
               this.Controls.Add(this.cbEnableParallelTasks);
               this.Name = "TaskViewSettingsView";
               this.Text = "TaskViewSettingsView";
               this.ResumeLayout(false);
               this.PerformLayout();

          }

          #endregion
          private System.Windows.Forms.CheckBox cbEnableParallelTasks;
          private System.Windows.Forms.CheckedListBox clbSettings;
     }
}