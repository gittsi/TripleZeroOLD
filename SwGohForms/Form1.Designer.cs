namespace SwGohForms
{
    partial class Form1
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.butGuildLoad = new System.Windows.Forms.Button();
            this.textGuildFixed = new System.Windows.Forms.TextBox();
            this.textGuildID = new System.Windows.Forms.TextBox();
            this.textGuildName = new System.Windows.Forms.TextBox();
            this.labFixed = new System.Windows.Forms.Label();
            this.labID = new System.Windows.Forms.Label();
            this.labRealName = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.butDelFromQ = new System.Windows.Forms.Button();
            this.textDelFromQ = new System.Windows.Forms.TextBox();
            this.labDelFromQ = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.butDelFromP = new System.Windows.Forms.Button();
            this.textDelFromP = new System.Windows.Forms.TextBox();
            this.labDelFromP = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.butGuildLoad);
            this.groupBox1.Controls.Add(this.textGuildFixed);
            this.groupBox1.Controls.Add(this.textGuildID);
            this.groupBox1.Controls.Add(this.textGuildName);
            this.groupBox1.Controls.Add(this.labFixed);
            this.groupBox1.Controls.Add(this.labID);
            this.groupBox1.Controls.Add(this.labRealName);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(232, 117);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "TW opponent Guild Launch";
            // 
            // butGuildLoad
            // 
            this.butGuildLoad.Location = new System.Drawing.Point(150, 82);
            this.butGuildLoad.Name = "butGuildLoad";
            this.butGuildLoad.Size = new System.Drawing.Size(75, 23);
            this.butGuildLoad.TabIndex = 7;
            this.butGuildLoad.Text = "Guild Load";
            this.butGuildLoad.UseVisualStyleBackColor = true;
            this.butGuildLoad.Click += new System.EventHandler(this.butGuildLoad_Click);
            // 
            // textGuildFixed
            // 
            this.textGuildFixed.Location = new System.Drawing.Point(83, 56);
            this.textGuildFixed.Name = "textGuildFixed";
            this.textGuildFixed.Size = new System.Drawing.Size(142, 20);
            this.textGuildFixed.TabIndex = 6;
            this.textGuildFixed.Text = "order-66-501st-division";
            // 
            // textGuildID
            // 
            this.textGuildID.Location = new System.Drawing.Point(83, 35);
            this.textGuildID.Name = "textGuildID";
            this.textGuildID.Size = new System.Drawing.Size(142, 20);
            this.textGuildID.TabIndex = 5;
            this.textGuildID.Text = "32";
            // 
            // textGuildName
            // 
            this.textGuildName.Location = new System.Drawing.Point(83, 13);
            this.textGuildName.Name = "textGuildName";
            this.textGuildName.Size = new System.Drawing.Size(142, 20);
            this.textGuildName.TabIndex = 4;
            this.textGuildName.Text = "Order 66 501st Division";
            // 
            // labFixed
            // 
            this.labFixed.AutoSize = true;
            this.labFixed.Location = new System.Drawing.Point(6, 59);
            this.labFixed.Name = "labFixed";
            this.labFixed.Size = new System.Drawing.Size(72, 13);
            this.labFixed.TabIndex = 3;
            this.labFixed.Text = "Fixed Name : ";
            // 
            // labID
            // 
            this.labID.AutoSize = true;
            this.labID.Location = new System.Drawing.Point(6, 38);
            this.labID.Name = "labID";
            this.labID.Size = new System.Drawing.Size(27, 13);
            this.labID.TabIndex = 2;
            this.labID.Text = "ID : ";
            // 
            // labRealName
            // 
            this.labRealName.AutoSize = true;
            this.labRealName.Location = new System.Drawing.Point(6, 16);
            this.labRealName.Name = "labRealName";
            this.labRealName.Size = new System.Drawing.Size(71, 13);
            this.labRealName.TabIndex = 1;
            this.labRealName.Text = "Guild Name : ";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.butDelFromQ);
            this.groupBox2.Controls.Add(this.textDelFromQ);
            this.groupBox2.Controls.Add(this.labDelFromQ);
            this.groupBox2.Location = new System.Drawing.Point(12, 135);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(232, 80);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Delete Guild From Queue";
            // 
            // butDelFromQ
            // 
            this.butDelFromQ.Location = new System.Drawing.Point(117, 45);
            this.butDelFromQ.Name = "butDelFromQ";
            this.butDelFromQ.Size = new System.Drawing.Size(108, 23);
            this.butDelFromQ.TabIndex = 7;
            this.butDelFromQ.Text = "Delete From Queue";
            this.butDelFromQ.UseVisualStyleBackColor = true;
            this.butDelFromQ.Click += new System.EventHandler(this.butDelFromQ_Click);
            // 
            // textDelFromQ
            // 
            this.textDelFromQ.Location = new System.Drawing.Point(83, 19);
            this.textDelFromQ.Name = "textDelFromQ";
            this.textDelFromQ.Size = new System.Drawing.Size(142, 20);
            this.textDelFromQ.TabIndex = 6;
            this.textDelFromQ.Text = "Order 66 501st Division";
            // 
            // labDelFromQ
            // 
            this.labDelFromQ.AutoSize = true;
            this.labDelFromQ.Location = new System.Drawing.Point(6, 22);
            this.labDelFromQ.Name = "labDelFromQ";
            this.labDelFromQ.Size = new System.Drawing.Size(71, 13);
            this.labDelFromQ.TabIndex = 5;
            this.labDelFromQ.Text = "Guild Name : ";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.butDelFromP);
            this.groupBox3.Controls.Add(this.textDelFromP);
            this.groupBox3.Controls.Add(this.labDelFromP);
            this.groupBox3.Location = new System.Drawing.Point(12, 221);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(232, 82);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Delete Guild From Player";
            // 
            // butDelFromP
            // 
            this.butDelFromP.Location = new System.Drawing.Point(117, 45);
            this.butDelFromP.Name = "butDelFromP";
            this.butDelFromP.Size = new System.Drawing.Size(108, 23);
            this.butDelFromP.TabIndex = 10;
            this.butDelFromP.Text = "Delete From Player";
            this.butDelFromP.UseVisualStyleBackColor = true;
            this.butDelFromP.Click += new System.EventHandler(this.butDelFromP_Click);
            // 
            // textDelFromP
            // 
            this.textDelFromP.Location = new System.Drawing.Point(83, 19);
            this.textDelFromP.Name = "textDelFromP";
            this.textDelFromP.Size = new System.Drawing.Size(142, 20);
            this.textDelFromP.TabIndex = 9;
            this.textDelFromP.Text = "Order 66 501st Division";
            // 
            // labDelFromP
            // 
            this.labDelFromP.AutoSize = true;
            this.labDelFromP.Location = new System.Drawing.Point(6, 22);
            this.labDelFromP.Name = "labDelFromP";
            this.labDelFromP.Size = new System.Drawing.Size(71, 13);
            this.labDelFromP.TabIndex = 8;
            this.labDelFromP.Text = "Guild Name : ";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 316);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.Text = "SwGoh";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textGuildFixed;
        private System.Windows.Forms.TextBox textGuildID;
        private System.Windows.Forms.TextBox textGuildName;
        private System.Windows.Forms.Label labFixed;
        private System.Windows.Forms.Label labID;
        private System.Windows.Forms.Label labRealName;
        private System.Windows.Forms.Button butGuildLoad;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button butDelFromQ;
        private System.Windows.Forms.TextBox textDelFromQ;
        private System.Windows.Forms.Label labDelFromQ;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button butDelFromP;
        private System.Windows.Forms.TextBox textDelFromP;
        private System.Windows.Forms.Label labDelFromP;
    }
}

