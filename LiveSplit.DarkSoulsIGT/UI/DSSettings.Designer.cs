namespace LiveSplit.DarkSoulsIGT
{
    partial class DSSettings
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.cbxInventoryReset = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbxStartTimer = new System.Windows.Forms.CheckBox();
            this.cbxUseIGT = new System.Windows.Forms.CheckBox();
            this.groupBoxAutosplitter = new System.Windows.Forms.GroupBox();
            this.cbxAutosplitterEnabled = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxAutosplitter.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbxInventoryReset
            // 
            this.cbxInventoryReset.AutoSize = true;
            this.cbxInventoryReset.Location = new System.Drawing.Point(7, 43);
            this.cbxInventoryReset.Name = "cbxInventoryReset";
            this.cbxInventoryReset.Size = new System.Drawing.Size(139, 17);
            this.cbxInventoryReset.TabIndex = 0;
            this.cbxInventoryReset.Text = "Reset inventory indexes";
            this.cbxInventoryReset.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxAutosplitter, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(321, 245);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.Controls.Add(this.cbxStartTimer);
            this.groupBox2.Controls.Add(this.cbxUseIGT);
            this.groupBox2.Controls.Add(this.cbxInventoryReset);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(315, 100);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Settings";
            // 
            // cbxStartTimer
            // 
            this.cbxStartTimer.AutoSize = true;
            this.cbxStartTimer.Location = new System.Drawing.Point(7, 67);
            this.cbxStartTimer.Name = "cbxStartTimer";
            this.cbxStartTimer.Size = new System.Drawing.Size(137, 17);
            this.cbxStartTimer.TabIndex = 2;
            this.cbxStartTimer.Text = "Start timer automatically";
            this.cbxStartTimer.UseVisualStyleBackColor = true;
            // 
            // cbxUseIGT
            // 
            this.cbxUseIGT.AutoSize = true;
            this.cbxUseIGT.Location = new System.Drawing.Point(7, 20);
            this.cbxUseIGT.Name = "cbxUseIGT";
            this.cbxUseIGT.Size = new System.Drawing.Size(96, 17);
            this.cbxUseIGT.TabIndex = 1;
            this.cbxUseIGT.Text = "Use game time";
            this.cbxUseIGT.UseVisualStyleBackColor = true;
            // 
            // groupBoxAutosplitter
            // 
            this.groupBoxAutosplitter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxAutosplitter.AutoSize = true;
            this.groupBoxAutosplitter.Controls.Add(this.cbxAutosplitterEnabled);
            this.groupBoxAutosplitter.Location = new System.Drawing.Point(3, 109);
            this.groupBoxAutosplitter.Name = "groupBoxAutosplitter";
            this.groupBoxAutosplitter.Size = new System.Drawing.Size(315, 133);
            this.groupBoxAutosplitter.TabIndex = 4;
            this.groupBoxAutosplitter.TabStop = false;
            this.groupBoxAutosplitter.Text = "Autosplitter";
            // 
            // cbxAutosplitterEnabled
            // 
            this.cbxAutosplitterEnabled.AutoSize = true;
            this.cbxAutosplitterEnabled.Location = new System.Drawing.Point(7, 20);
            this.cbxAutosplitterEnabled.Name = "cbxAutosplitterEnabled";
            this.cbxAutosplitterEnabled.Size = new System.Drawing.Size(59, 17);
            this.cbxAutosplitterEnabled.TabIndex = 0;
            this.cbxAutosplitterEnabled.Text = "Enable";
            this.cbxAutosplitterEnabled.UseVisualStyleBackColor = true;
            // 
            // DSSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DSSettings";
            this.Size = new System.Drawing.Size(321, 245);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBoxAutosplitter.ResumeLayout(false);
            this.groupBoxAutosplitter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox cbxInventoryReset;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbxStartTimer;
        private System.Windows.Forms.CheckBox cbxUseIGT;
        private System.Windows.Forms.GroupBox groupBoxAutosplitter;
        private System.Windows.Forms.CheckBox cbxAutosplitterEnabled;
    }
}
