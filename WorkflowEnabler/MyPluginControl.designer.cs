using System;
using System.Drawing;
using System.Windows.Forms;

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

        // Declaration of textbox for log
        private TextBox txtLog;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // -----------------------------
            // Configuration of the ToolStrip (ribbon)
            // -----------------------------
            this.toolStrip = new ToolStrip();
            this.toolStrip.Dock = DockStyle.Top;
            // Using Calibri for a more user friendly look
            this.toolStrip.Font = new Font("Calibri", 11F, FontStyle.Regular);
            this.toolStrip.GripStyle = ToolStripGripStyle.Hidden;

            // "Load Workflows" button
            this.tsbLoadWorkflows = new ToolStripButton();
            this.tsbLoadWorkflows.Text = "🔄 Load Workflows";
            this.tsbLoadWorkflows.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.tsbLoadWorkflows.Click += new EventHandler(this.btnLoadWorkflows_Click);

            // "Activate" button
            this.tsbActivate = new ToolStripButton();
            this.tsbActivate.Text = "✅ Activate";
            this.tsbActivate.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.tsbActivate.Click += new EventHandler(this.btnActivate_Click);

            // "Deactivate" button
            this.tsbDeactivate = new ToolStripButton();
            this.tsbDeactivate.Text = "⛔ Deactivate";
            this.tsbDeactivate.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.tsbDeactivate.Click += new EventHandler(this.btnDeactivate_Click);

            // "Export CSV" button
            this.tsbExport = new ToolStripButton();
            this.tsbExport.Text = "Export CSV";
            this.tsbExport.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.tsbExport.Click += new EventHandler(this.btnExport_Click);

            // Separator to separate actions from filters/search
            this.toolStripSeparator = new ToolStripSeparator();

            // Search element
            this.tslSearch = new ToolStripLabel();
            this.tslSearch.Text = "🔍 Search:";
            this.tslSearch.Font = new Font("Calibri", 11F, FontStyle.Regular);

            this.tstxtSearch = new ToolStripTextBox();
            this.tstxtSearch.Size = new Size(200, 25);
            this.tstxtSearch.Font = new Font("Calibri", 11F, FontStyle.Regular);
            this.tstxtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);

            // Filter element
            this.tslFilter = new ToolStripLabel();
            this.tslFilter.Text = "Filter by type:";
            this.tslFilter.Font = new Font("Calibri", 11F, FontStyle.Regular);

            this.tscmbFilterProcessType = new ToolStripComboBox();
            this.tscmbFilterProcessType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.tscmbFilterProcessType.Font = new Font("Calibri", 11F, FontStyle.Regular);
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
                "Web Client API Flow"
            });
            this.tscmbFilterProcessType.SelectedIndex = 0;
            this.tscmbFilterProcessType.Size = new Size(180, 25);
            this.tscmbFilterProcessType.SelectedIndexChanged += new EventHandler(this.cmbFilterProcessType_SelectedIndexChanged);

            // Add items to the ToolStrip in the desired order
            this.toolStrip.Items.Add(this.tsbLoadWorkflows);
            this.toolStrip.Items.Add(this.tsbActivate);
            this.toolStrip.Items.Add(this.tsbDeactivate);
            this.toolStrip.Items.Add(this.tsbExport);
            this.toolStrip.Items.Add(this.toolStripSeparator);
            this.toolStrip.Items.Add(this.tslSearch);
            this.toolStrip.Items.Add(this.tstxtSearch);
            this.toolStrip.Items.Add(this.tslFilter);
            this.toolStrip.Items.Add(this.tscmbFilterProcessType);

            // -----------------------------
            // Configuration of the DataGridView
            // -----------------------------
            this.dgvWorkflows = new DataGridView();
            this.dgvWorkflows.Dock = DockStyle.Fill;
            this.dgvWorkflows.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvWorkflows.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvWorkflows.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvWorkflows.BackgroundColor = Color.White;
            this.dgvWorkflows.EnableHeadersVisualStyles = false;


            // Souscription à l'événement CellFormatting
            this.dgvWorkflows.CellFormatting += new DataGridViewCellFormattingEventHandler(this.dgvWorkflows_CellFormatting);


            // Personnalisation des en-têtes
            DataGridViewCellStyle headerStyle = new DataGridViewCellStyle();
            headerStyle.BackColor = Color.FromArgb(240, 240, 240);
            headerStyle.Font = new Font("Calibri", 11F, FontStyle.Bold);
            headerStyle.ForeColor = Color.Black;
            headerStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvWorkflows.ColumnHeadersDefaultCellStyle = headerStyle;

            // Alternating colors for lines
            DataGridViewCellStyle alternatingCellStyle = new DataGridViewCellStyle();
            alternatingCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            this.dgvWorkflows.AlternatingRowsDefaultCellStyle = alternatingCellStyle;

            // Customize selection style (replace default blue)
            this.dgvWorkflows.DefaultCellStyle.SelectionBackColor = Color.LightSteelBlue;
            this.dgvWorkflows.DefaultCellStyle.SelectionForeColor = Color.Black;


            // Delete line headers
            this.dgvWorkflows.RowHeadersVisible = false;

            // borders and grid style
            this.dgvWorkflows.BorderStyle = BorderStyle.None;
            this.dgvWorkflows.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvWorkflows.GridColor = Color.LightGray;

            // Other customizations  : change line heights
            this.dgvWorkflows.RowTemplate.Height = 30;

            // Create read-only columns
            this.colNom = new DataGridViewTextBoxColumn();
            this.colNom.HeaderText = "Name";
            this.colNom.Name = "colNom";
            this.colNom.ReadOnly = true;

            this.colID = new DataGridViewTextBoxColumn();
            this.colID.HeaderText = "ID";
            this.colID.Name = "colID";
            this.colID.ReadOnly = true;
            this.colID.Visible = false; // Hidden, used for internal processing

            this.colType = new DataGridViewTextBoxColumn();
            this.colType.HeaderText = "Type";
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            this.colType.Width = 150;

            this.colStatut = new DataGridViewTextBoxColumn();
            this.colStatut.HeaderText = "Status";
            this.colStatut.Name = "colStatut";
            this.colStatut.ReadOnly = true;

            this.colCreatedOn = new DataGridViewTextBoxColumn();
            this.colCreatedOn.HeaderText = "Created On";
            this.colCreatedOn.Name = "colCreatedOn";
            this.colCreatedOn.ReadOnly = true;

            this.colModifiedOn = new DataGridViewTextBoxColumn();
            this.colModifiedOn.HeaderText = "Modified On";
            this.colModifiedOn.Name = "colModifiedOn";
            this.colModifiedOn.ReadOnly = true;

            this.colOwner = new DataGridViewTextBoxColumn();
            this.colOwner.HeaderText = "Owner";
            this.colOwner.Name = "colOwner";
            this.colOwner.ReadOnly = true;

            this.colIsManaged = new DataGridViewTextBoxColumn();
            this.colIsManaged.HeaderText = "Managed";
            this.colIsManaged.Name = "colIsManaged";
            this.colIsManaged.ReadOnly = true;

            // Add columns to the DataGridView (order matters)
            this.dgvWorkflows.Columns.AddRange(new DataGridViewColumn[] {
                this.colNom,
                this.colID,
                this.colType,
                this.colStatut,
                this.colCreatedOn,
                this.colModifiedOn,
                this.colOwner,
                this.colIsManaged
            });

            // -----------------------------
            // Initialize the log TextBox (simulating a CMD-like console)
            // -----------------------------
            this.txtLog = new TextBox();
            this.txtLog.Multiline = true;
            this.txtLog.ScrollBars = ScrollBars.Both;
            this.txtLog.Font = new Font("Calibri", 10F);
            this.txtLog.Dock = DockStyle.Bottom;
            this.txtLog.Height = 150; // Adjustable height
            this.txtLog.ReadOnly = true;
            this.txtLog.BackColor = Color.Black;
            this.txtLog.ForeColor = Color.LightGreen;

            // -----------------------------
            // Add controls to the UserControl
            // -----------------------------
            this.Controls.Add(this.dgvWorkflows);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.txtLog);

            // UserControl properties
            this.Name = "MyPluginControl";
            this.Size = new Size(900, 600);
            this.Load += new EventHandler(this.MyPluginControl_Load);
        }
    }
}
