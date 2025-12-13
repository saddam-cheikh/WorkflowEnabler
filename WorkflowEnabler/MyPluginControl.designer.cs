using System;
using System.Drawing;
using System.Windows.Forms;
using static ScintillaNET.Style;

namespace WorkflowEnabler
{
    partial class MyPluginControl
    {
        private System.ComponentModel.IContainer components = null;

        // Declaration of ToolStrip elements (ribbon)
        private ToolStrip toolStrip;
        private ToolStripButton tsbLoadWorkflows;
        private ToolStripButton tsbActivate;
        private ToolStripButton tsbDeactivate;
        private ToolStripButton tsbExport;
        private ToolStripSeparator toolStripSeparator;
        private ToolStripLabel tslSearch;
        private ToolStripTextBox tstxtSearch;
        private ToolStripLabel tslFilter;
        private ToolStripComboBox tscmbFilterProcessType;

        // Declaration of DataGridView and its columns
        private DataGridView dgvWorkflows;
        private DataGridViewTextBoxColumn colNom;
        private DataGridViewTextBoxColumn colID;
        private DataGridViewTextBoxColumn colType;
        private DataGridViewTextBoxColumn colStatut;
        private DataGridViewTextBoxColumn colCreatedOn;
        private DataGridViewTextBoxColumn colModifiedOn;
        private DataGridViewTextBoxColumn colOwner;
        private DataGridViewTextBoxColumn colIsManaged;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }


        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.tsbLoadWorkflows = new System.Windows.Forms.ToolStripButton();
            this.tsbActivate = new System.Windows.Forms.ToolStripButton();
            this.tsbDeactivate = new System.Windows.Forms.ToolStripButton();
            this.tsbExport = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.tslSearch = new System.Windows.Forms.ToolStripLabel();
            this.tstxtSearch = new System.Windows.Forms.ToolStripTextBox();
            this.tslFilter = new System.Windows.Forms.ToolStripLabel();
            this.tscmbFilterProcessType = new System.Windows.Forms.ToolStripComboBox();
            this.dgvWorkflows = new System.Windows.Forms.DataGridView();
            this.colNom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatut = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCreatedOn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colModifiedOn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOwner = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIsManaged = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWorkflows)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Font = new System.Drawing.Font("Calibri", 11F);
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbLoadWorkflows,
            this.tsbActivate,
            this.tsbDeactivate,
            this.tsbExport,
            this.toolStripSeparator,
            this.tslSearch,
            this.tstxtSearch,
            this.tslFilter,
            this.tscmbFilterProcessType});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(900, 30);
            this.toolStrip.TabIndex = 1;
            // 
            // tsbLoadWorkflows
            // 
            this.tsbLoadWorkflows.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbLoadWorkflows.Name = "tsbLoadWorkflows";
            this.tsbLoadWorkflows.Size = new System.Drawing.Size(127, 27);
            this.tsbLoadWorkflows.Text = "🔄 Load Workflows";
            this.tsbLoadWorkflows.Click += new System.EventHandler(this.btnLoadWorkflows_Click);
            // 
            // tsbActivate
            // 
            this.tsbActivate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbActivate.Name = "tsbActivate";
            this.tsbActivate.Size = new System.Drawing.Size(80, 27);
            this.tsbActivate.Text = "✅ Activate";
            this.tsbActivate.Click += new System.EventHandler(this.btnActivate_Click);
            // 
            // tsbDeactivate
            // 
            this.tsbDeactivate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbDeactivate.Name = "tsbDeactivate";
            this.tsbDeactivate.Size = new System.Drawing.Size(95, 27);
            this.tsbDeactivate.Text = "⛔ Deactivate";
            this.tsbDeactivate.Click += new System.EventHandler(this.btnDeactivate_Click);
            // 
            // tsbExport
            // 
            this.tsbExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbExport.Name = "tsbExport";
            this.tsbExport.Size = new System.Drawing.Size(79, 27);
            this.tsbExport.Text = "Export CSV";
            this.tsbExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 30);
            // 
            // tslSearch
            // 
            this.tslSearch.Font = new System.Drawing.Font("Calibri", 11F);
            this.tslSearch.Name = "tslSearch";
            this.tslSearch.Size = new System.Drawing.Size(71, 27);
            this.tslSearch.Text = "🔍 Search:";
            // 
            // tstxtSearch
            // 
            this.tstxtSearch.Font = new System.Drawing.Font("Calibri", 11F);
            this.tstxtSearch.Name = "tstxtSearch";
            this.tstxtSearch.Size = new System.Drawing.Size(200, 30);
            this.tstxtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // tslFilter
            // 
            this.tslFilter.Font = new System.Drawing.Font("Calibri", 11F);
            this.tslFilter.Name = "tslFilter";
            this.tslFilter.Size = new System.Drawing.Size(94, 27);
            this.tslFilter.Text = "Filter by type:";
            // 
            // tscmbFilterProcessType
            // 
            this.tscmbFilterProcessType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscmbFilterProcessType.Font = new System.Drawing.Font("Calibri", 11F);
            this.tscmbFilterProcessType.Items.AddRange(new object[] {
            "All",
            "Workflow",
            "Dialog",
            "Business Rule",
            "Action",
            "BPF (Business Process Flow)",
            "Cloud Flow (Modern Flow)",
            "Desktop Flow",
            "AI Flow",
            "Web Client API Flow"});
            this.tscmbFilterProcessType.Name = "tscmbFilterProcessType";
            this.tscmbFilterProcessType.Size = new System.Drawing.Size(180, 26);
            this.tscmbFilterProcessType.Text = "All";
            this.tscmbFilterProcessType.SelectedIndexChanged += new System.EventHandler(this.cmbFilterProcessType_SelectedIndexChanged);
            // 
            // dgvWorkflows
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.dgvWorkflows.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvWorkflows.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvWorkflows.BackgroundColor = System.Drawing.Color.White;
            this.dgvWorkflows.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvWorkflows.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            this.dgvWorkflows.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvWorkflows.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvWorkflows.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNom,
            this.colID,
            this.colType,
            this.colStatut,
            this.colCreatedOn,
            this.colModifiedOn,
            this.colOwner,
            this.colIsManaged});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.LightSteelBlue;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvWorkflows.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvWorkflows.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvWorkflows.EnableHeadersVisualStyles = false;
            this.dgvWorkflows.GridColor = System.Drawing.Color.LightGray;
            this.dgvWorkflows.Location = new System.Drawing.Point(0, 30);
            this.dgvWorkflows.Name = "dgvWorkflows";
            this.dgvWorkflows.RowHeadersVisible = false;
            this.dgvWorkflows.RowTemplate.Height = 30;
            this.dgvWorkflows.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvWorkflows.Size = new System.Drawing.Size(900, 570);
            this.dgvWorkflows.TabIndex = 0;
            this.dgvWorkflows.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvWorkflows_CellFormatting);
            // 
            // colNom
            // 
            this.colNom.HeaderText = "Name";
            this.colNom.Name = "colNom";
            this.colNom.ReadOnly = true;
            // 
            // colID
            // 
            this.colID.HeaderText = "ID";
            this.colID.Name = "colID";
            this.colID.ReadOnly = true;
            this.colID.Visible = false;
            // 
            // colType
            // 
            this.colType.HeaderText = "Type";
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            // 
            // colStatut
            // 
            this.colStatut.HeaderText = "Status";
            this.colStatut.Name = "colStatut";
            this.colStatut.ReadOnly = true;
            // 
            // colCreatedOn
            // 
            this.colCreatedOn.HeaderText = "Created On";
            this.colCreatedOn.Name = "colCreatedOn";
            this.colCreatedOn.ReadOnly = true;
            // 
            // colModifiedOn
            // 
            this.colModifiedOn.HeaderText = "Modified On";
            this.colModifiedOn.Name = "colModifiedOn";
            this.colModifiedOn.ReadOnly = true;
            // 
            // colOwner
            // 
            this.colOwner.HeaderText = "Owner";
            this.colOwner.Name = "colOwner";
            this.colOwner.ReadOnly = true;
            // 
            // colIsManaged
            // 
            this.colIsManaged.HeaderText = "Managed";
            this.colIsManaged.Name = "colIsManaged";
            this.colIsManaged.ReadOnly = true;
           
            // 
            // MyPluginControl
            // 
            this.Controls.Add(this.dgvWorkflows);
            this.Controls.Add(this.toolStrip);
            this.Name = "MyPluginControl";
            this.Size = new System.Drawing.Size(900, 600);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWorkflows)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    }
}
