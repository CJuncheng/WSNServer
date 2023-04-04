namespace WSNServer.Views
{
    partial class MainView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainView));
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.城市设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.设置城市个数ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.设置城市节点ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblMessageTitle = new System.Windows.Forms.Label();
            this.lvMessage = new System.Windows.Forms.ListView();
            this.Image = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Index = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Message = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NowTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageListIcon = new System.Windows.Forms.ImageList(this.components);
            this.tabMain = new System.Windows.Forms.TabControl();
            this.btnClear = new System.Windows.Forms.Button();
            this.menuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuMain
            // 
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.城市设置ToolStripMenuItem});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new System.Drawing.Size(669, 25);
            this.menuMain.TabIndex = 2;
            this.menuMain.Text = "menuStrip1";
            // 
            // 城市设置ToolStripMenuItem
            // 
            this.城市设置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.设置城市个数ToolStripMenuItem,
            this.设置城市节点ToolStripMenuItem});
            this.城市设置ToolStripMenuItem.Name = "城市设置ToolStripMenuItem";
            this.城市设置ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.城市设置ToolStripMenuItem.Text = "城市设置";
            // 
            // 设置城市个数ToolStripMenuItem
            // 
            this.设置城市个数ToolStripMenuItem.Name = "设置城市个数ToolStripMenuItem";
            this.设置城市个数ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.设置城市个数ToolStripMenuItem.Text = "设置城市个数";
            // 
            // 设置城市节点ToolStripMenuItem
            // 
            this.设置城市节点ToolStripMenuItem.Name = "设置城市节点ToolStripMenuItem";
            this.设置城市节点ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.设置城市节点ToolStripMenuItem.Text = "设置城市节点";
            // 
            // lblMessageTitle
            // 
            this.lblMessageTitle.AutoSize = true;
            this.lblMessageTitle.Location = new System.Drawing.Point(5, 236);
            this.lblMessageTitle.Name = "lblMessageTitle";
            this.lblMessageTitle.Size = new System.Drawing.Size(65, 12);
            this.lblMessageTitle.TabIndex = 3;
            this.lblMessageTitle.Text = "服务器消息";
            // 
            // lvMessage
            // 
            this.lvMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvMessage.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Image,
            this.Index,
            this.Message,
            this.NowTime});
            this.lvMessage.GridLines = true;
            this.lvMessage.LargeImageList = this.imageListIcon;
            this.lvMessage.Location = new System.Drawing.Point(0, 261);
            this.lvMessage.Name = "lvMessage";
            this.lvMessage.Size = new System.Drawing.Size(669, 181);
            this.lvMessage.SmallImageList = this.imageListIcon;
            this.lvMessage.TabIndex = 4;
            this.lvMessage.UseCompatibleStateImageBehavior = false;
            this.lvMessage.View = System.Windows.Forms.View.Details;
            // 
            // Image
            // 
            this.Image.Text = "图标";
            this.Image.Width = 40;
            // 
            // Index
            // 
            this.Index.Text = "序号";
            this.Index.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Index.Width = 40;
            // 
            // Message
            // 
            this.Message.Text = "说明";
            this.Message.Width = 430;
            // 
            // NowTime
            // 
            this.NowTime.Text = "时间";
            this.NowTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.NowTime.Width = 162;
            // 
            // imageListIcon
            // 
            this.imageListIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListIcon.ImageStream")));
            this.imageListIcon.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListIcon.Images.SetKeyName(0, "error.png");
            this.imageListIcon.Images.SetKeyName(1, "message.png");
            // 
            // tabMain
            // 
            this.tabMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabMain.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabMain.Location = new System.Drawing.Point(0, 28);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(669, 200);
            this.tabMain.TabIndex = 5;
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.BackColor = System.Drawing.Color.Transparent;
            this.btnClear.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnClear.BackgroundImage")));
            this.btnClear.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnClear.FlatAppearance.BorderSize = 0;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Location = new System.Drawing.Point(635, 227);
            this.btnClear.Margin = new System.Windows.Forms.Padding(0);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(25, 31);
            this.btnClear.TabIndex = 12;
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 444);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.lvMessage);
            this.Controls.Add(this.lblMessageTitle);
            this.Controls.Add(this.menuMain);
            this.MainMenuStrip = this.menuMain;
            this.Name = "MainView";
            this.Text = "服务器";
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem 城市设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 设置城市个数ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 设置城市节点ToolStripMenuItem;
        private System.Windows.Forms.Label lblMessageTitle;
        private System.Windows.Forms.ListView lvMessage;
        private System.Windows.Forms.ColumnHeader Image;
        private System.Windows.Forms.ColumnHeader Message;
        private System.Windows.Forms.ColumnHeader NowTime;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ColumnHeader Index;
        public System.Windows.Forms.ImageList imageListIcon;
    }
}