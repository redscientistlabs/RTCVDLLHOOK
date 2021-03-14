
namespace XemuVanguardHook
{
    partial class TestForm
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
            this.btnClearBlastUnits = new System.Windows.Forms.Button();
            this.btnRestart = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnClearBlastUnits
            // 
            this.btnClearBlastUnits.Location = new System.Drawing.Point(2, 2);
            this.btnClearBlastUnits.Name = "btnClearBlastUnits";
            this.btnClearBlastUnits.Size = new System.Drawing.Size(100, 23);
            this.btnClearBlastUnits.TabIndex = 1;
            this.btnClearBlastUnits.Text = "Clear Blast Units";
            this.btnClearBlastUnits.UseVisualStyleBackColor = true;
            this.btnClearBlastUnits.Click += new System.EventHandler(this.btnClearBlastUnits_Click);
            // 
            // btnRestart
            // 
            this.btnRestart.Location = new System.Drawing.Point(108, 2);
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Size = new System.Drawing.Size(100, 23);
            this.btnRestart.TabIndex = 1;
            this.btnRestart.Text = "Restart Xemu";
            this.btnRestart.UseVisualStyleBackColor = true;
            this.btnRestart.Click += new System.EventHandler(this.btnRestart_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(214, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Restart Xemu";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnRestart_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(316, 93);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnRestart);
            this.Controls.Add(this.btnClearBlastUnits);
            this.Name = "TestForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClearBlastUnits;
        private System.Windows.Forms.Button btnRestart;
        private System.Windows.Forms.Button button1;
    }
}