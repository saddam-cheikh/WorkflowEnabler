using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using Microsoft.Xrm.Sdk.Messages;

namespace WorkflowEnabler
{
    public partial class MyPluginControl : PluginControlBase
    {
        private Settings mySettings;
        private Guid currentSolutionId = Guid.Empty;
        private Dictionary<Guid, string> workflowInfo = new Dictionary<Guid, string>();

        public MyPluginControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes plugin settings and displays an info notification.
        /// </summary>
        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            ShowInfoNotification("This plugin allows you to manage workflows and Power Automate flows. Feature requests and contributions are welcome via our GitHub repository.",
                new Uri("https://github.com/saddam-cheikh/WorkflowEnabler"));

            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();
                LogWarning("No settings found, creating a configuration file.");
            }
            else
            {
                LogInfo("Settings loaded successfully.");
            }
        }

        /// <summary>
        /// Applies filters when the search text changes.
        /// </summary>
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Applies filters when the process type selection changes.
        /// </summary>
        private void cmbFilterProcessType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Filters the workflow grid based on search text and process type.
        /// </summary>
        private void ApplyFilters()
        {
            string filterText = tstxtSearch.Text.Trim().ToLower();
            string selectedType = tscmbFilterProcessType.SelectedItem.ToString();

            foreach (DataGridViewRow row in dgvWorkflows.Rows)
            {
                if (row.IsNewRow)
                    continue;

                bool visible = true;

                if (!string.IsNullOrEmpty(filterText))
                {
                    visible = false;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value != null && cell.Value.ToString().ToLower().Contains(filterText))
                        {
                            visible = true;
                            break;
                        }
                    }
                }

                if (visible && selectedType != "All")
                {
                    string rowType = row.Cells["colType"].Value?.ToString() ?? "";
                    visible = selectedType == "Others"
                        ? !IsKnownType(rowType)
                        : rowType.Equals(selectedType, StringComparison.OrdinalIgnoreCase);
                }
                row.Visible = visible;
            }
        }

        /// <summary>
        /// Retrieves all solutions.
        /// </summary>
        private List<Entity> GetSolutions()
        {
            var query = new QueryExpression("solution")
            {
                ColumnSet = new ColumnSet("friendlyname", "solutionid")
            };

            var result = Service.RetrieveMultiple(query);
            return result.Entities.ToList();
        }

        /// <summary>
        /// Displays a dialog for solution selection and returns the selected solution ID.
        /// </summary>
        private Guid ShowSolutionSelectionDialog()
        {
            List<Entity> solutions = GetSolutions();

            if (solutions.Count == 0)
            {
                MessageBox.Show("No solution available.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return Guid.Empty;
            }

            using (Form form = new Form())
            {
                form.Text = "Select a solution";
                form.Width = 400;
                form.Height = 200;
                form.StartPosition = FormStartPosition.CenterScreen;

                ComboBox cbSolutions = new ComboBox { Left = 20, Top = 20, Width = 350 };
                Button btnOk = new Button { Text = "OK", Left = 150, Width = 100, Top = 70, DialogResult = DialogResult.OK };

                foreach (var sol in solutions)
                {
                    cbSolutions.Items.Add(new KeyValuePair<string, Guid>(sol.GetAttributeValue<string>("friendlyname"), sol.Id));
                }
                cbSolutions.DisplayMember = "Key";
                cbSolutions.ValueMember = "Value";
                cbSolutions.SelectedIndex = 0;

                form.Controls.Add(cbSolutions);
                form.Controls.Add(btnOk);
                form.AcceptButton = btnOk;

                if (form.ShowDialog() == DialogResult.OK)
                    return ((KeyValuePair<string, Guid>)cbSolutions.SelectedItem).Value;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Loads workflows for the currently selected solution.
        /// </summary>
        private void btnLoadWorkflows_Click(object sender, EventArgs e)
        {
            if (currentSolutionId == Guid.Empty)
            {
                currentSolutionId = ShowSolutionSelectionDialog();
                if (currentSolutionId == Guid.Empty)
                    return;
            }

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading workflows...",
                Work = (worker, args) =>
                {
                    var workflows = GetWorkflowsBySolution(currentSolutionId);
                    args.Result = workflows;
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        dgvWorkflows.Rows.Clear();
                        workflowInfo.Clear();
                        var workflows = args.Result as List<Entity>;
                        if (workflows != null)
                        {
                            foreach (var wf in workflows)
                            {
                                string status = wf.GetAttributeValue<OptionSetValue>("statecode")?.Value == 1 ? "Active" : "Draft";
                                int category = (int)(wf.GetAttributeValue<OptionSetValue>("category")?.Value);
                                string processType;
                                if (category == 0)
                                    processType = "Workflow";
                                else if (category == 1)
                                    processType = "Dialog";
                                else if (category == 2)
                                    processType = "Business Rule";
                                else if (category == 3)
                                    processType = "Action";
                                else if (category == 4)
                                    processType = "BPF (Business Process Flow)";
                                else if (category == 5)
                                    processType = "Cloud Flow (Modern Flow)";
                                else if (category == 6)
                                    processType = "Desktop Flow";
                                else if (category == 7)
                                    processType = "AI Flow";
                                else if (category == 9000)
                                    processType = "Web Client API Flow";
                                else
                                    processType = "Unknown";

                                DateTime createdOn = wf.GetAttributeValue<DateTime>("createdon");
                                DateTime modifiedOn = wf.GetAttributeValue<DateTime>("modifiedon");
                                EntityReference owner = wf.GetAttributeValue<EntityReference>("ownerid");
                                bool isManaged = wf.GetAttributeValue<bool>("ismanaged");

                                string createdOnStr = createdOn.ToString("g");
                                string modifiedOnStr = modifiedOn.ToString("g");
                                string managedStr = isManaged ? "Yes" : "No";
                                string wfName = wf.GetAttributeValue<string>("name");

                                dgvWorkflows.Rows.Add(
                                    wfName,
                                    wf.Id.ToString(),
                                    processType,
                                    status,
                                    createdOnStr,
                                    modifiedOnStr,
                                    owner != null ? owner.Name : "",
                                    managedStr
                                );
                                workflowInfo[wf.Id] = wfName;
                            }
                        }
                        ApplyFilters();
                    }
                }
            });
        }

        /// <summary>
        /// Formats the Status cell based on its value.
        /// </summary>
        private void dgvWorkflows_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvWorkflows.Columns[e.ColumnIndex].Name == "colStatut" && e.Value != null)
            {
                string status = e.Value.ToString().ToLower();
                if (status.Equals("active", StringComparison.OrdinalIgnoreCase))
                {
                    e.CellStyle.BackColor = Color.LightGreen;
                    e.CellStyle.ForeColor = Color.Black;
                }
                else if (status.Equals("draft", StringComparison.OrdinalIgnoreCase))
                {
                    e.CellStyle.BackColor = Color.LightCoral;
                    e.CellStyle.ForeColor = Color.White;
                }
            }
        }

        /// <summary>
        /// Determines if the given process type is known.
        /// </summary>
        private bool IsKnownType(string type)
        {
            return type == "Workflow" ||
                   type == "Dialog" ||
                   type == "Business Rule" ||
                   type == "Action" ||
                   type == "BPF (Business Process Flow)" ||
                   type == "Cloud Flow (Modern Flow)" ||
                   type == "Desktop Flow" ||
                   type == "AI Flow" ||
                   type == "Web Client API Flow";
        }

        /// <summary>
        /// Exports visible workflows to a CSV file.
        /// </summary>
        private void btnExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV Files (*.csv)|*.csv";
                sfd.FileName = "Workflows_Export.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                        {
                            sw.WriteLine("Name;ID;Type;Status;Created On;Modified On;Owner;Managed");

                            foreach (DataGridViewRow row in dgvWorkflows.Rows)
                            {
                                if (!row.Visible)
                                    continue;

                                string name = row.Cells["colNom"].Value?.ToString() ?? "";
                                string id = row.Cells["colID"].Value?.ToString() ?? "";
                                string type = row.Cells["colType"].Value?.ToString() ?? "";
                                string status = row.Cells["colStatut"].Value?.ToString() ?? "";
                                string createdOn = row.Cells["colCreatedOn"].Value?.ToString() ?? "";
                                string modifiedOn = row.Cells["colModifiedOn"].Value?.ToString() ?? "";
                                string owner = row.Cells["colOwner"].Value?.ToString() ?? "";
                                string managed = row.Cells["colIsManaged"].Value?.ToString() ?? "";
                                sw.WriteLine($"{name};{id};{type};{status};{createdOn};{modifiedOn};{owner};{managed}");
                            }
                        }
                        MessageBox.Show("Export successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error during export: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves workflows for the specified solution.
        /// </summary>
        private List<Entity> GetWorkflowsBySolution(Guid solutionId)
        {
            var query = new QueryExpression("solutioncomponent")
            {
                ColumnSet = new ColumnSet("objectid"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("solutionid", ConditionOperator.Equal, solutionId),
                        new ConditionExpression("componenttype", ConditionOperator.Equal, 29)
                    }
                }
            };

            var result = Service.RetrieveMultiple(query);
            List<Entity> workflows = new List<Entity>();
            foreach (var record in result.Entities)
            {
                Guid workflowId = record.GetAttributeValue<Guid>("objectid");
                Entity workflow = Service.Retrieve("workflow", workflowId,
                    new ColumnSet("name", "workflowid", "statecode", "statuscode", "category", "createdon", "modifiedon", "ownerid", "ismanaged"));
                workflows.Add(workflow);
            }
            return workflows;
        }

        /// <summary>
        /// Activates selected workflows.
        /// </summary>
        private void btnActivate_Click(object sender, EventArgs e)
        {
            ChangeWorkflowStatus(true);
        }

        /// <summary>
        /// Deactivates selected workflows.
        /// </summary>
        private void btnDeactivate_Click(object sender, EventArgs e)
        {
            ChangeWorkflowStatus(false);
        }

        /// <summary>
        /// Changes the status of selected workflows.
        /// </summary>
        /// <param name="activate">True to activate; false to deactivate.</param>
        private void ChangeWorkflowStatus(bool activate)
        {
            if (dgvWorkflows.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select one or more workflows.", "No selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Dictionary<Guid, string> selectedWorkflowInfo = new Dictionary<Guid, string>();
            foreach (DataGridViewRow row in dgvWorkflows.SelectedRows)
            {
                if (row.Cells["colID"].Value != null && row.Cells["colNom"].Value != null)
                {
                    Guid id = Guid.Parse(row.Cells["colID"].Value.ToString());
                    string name = row.Cells["colNom"].Value.ToString();
                    selectedWorkflowInfo[id] = name;
                }
            }

            if (selectedWorkflowInfo.Count == 0)
            {
                MessageBox.Show("No valid workflow selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<Guid> workflowIds = selectedWorkflowInfo.Keys.ToList();

            WorkAsync(new WorkAsyncInfo
            {
                Message = activate ? "Activating workflows..." : "Deactivating workflows...",
                Work = (worker, args) =>
                {
                    ExecuteMultipleRequest executeMultipleRequest = new ExecuteMultipleRequest
                    {
                        Settings = new ExecuteMultipleSettings()
                        {
                            ContinueOnError = true,
                            ReturnResponses = true
                        },
                        Requests = new OrganizationRequestCollection()
                    };

                    foreach (var workflowId in workflowIds)
                    {
                        var updateRequest = new UpdateRequest
                        {
                            Target = new Entity("workflow", workflowId)
                            {
                                ["statecode"] = new OptionSetValue(activate ? 1 : 0),
                                ["statuscode"] = new OptionSetValue(activate ? 2 : 1)
                            }
                        };
                        executeMultipleRequest.Requests.Add(updateRequest);
                    }

                    var response = Service.Execute(executeMultipleRequest) as ExecuteMultipleResponse;
                    args.Result = response;
                },
                PostWorkCallBack = (args) =>
                {
                    StringBuilder logBuilder = new StringBuilder();

                    if (args.Error != null)
                    {
                        logBuilder.AppendLine("Overall error: " + args.Error.ToString());
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        var response = args.Result as ExecuteMultipleResponse;
                        logBuilder.AppendLine(activate ? "Activation results:" : "Deactivation results:");

                        int successCount = 0;
                        int failureCount = 0;

                        for (int i = 0; i < response.Responses.Count; i++)
                        {
                            var responseItem = response.Responses[i];
                            Guid workflowId = workflowIds[i];
                            string workflowName = selectedWorkflowInfo.ContainsKey(workflowId) ? selectedWorkflowInfo[workflowId] : "Unknown";

                            if (responseItem.Fault != null)
                            {
                                failureCount++;
                                logBuilder.AppendLine($"Flow: {workflowName} (ID: {workflowId}) - Error: {responseItem.Fault.Message}");
                            }
                            else
                            {
                                successCount++;
                                logBuilder.AppendLine($"Flow: {workflowName} (ID: {workflowId}) - {(activate ? "Activated" : "Deactivated")} successfully.");
                            }
                        }

                        string logText = logBuilder.ToString();
                        LogInfo(logText);
                        txtLog.AppendText(logText + Environment.NewLine);

                        if (failureCount > 0 && successCount > 0)
                        {
                            MessageBox.Show($"{successCount} workflows succeeded and {failureCount} workflows failed. Please check the log for details.",
                                "Partial Success", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else if (failureCount > 0)
                        {
                            MessageBox.Show("All workflows failed. Please check the log for details.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show($"Workflows {(activate ? "activated" : "deactivated")} successfully!",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            RefreshWorkflowList(reapplyFilters: true);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Refreshes the workflow list. If reapplyFilters is false, resets the search and filter controls.
        /// </summary>
        /// <param name="reapplyFilters">True to keep current filters; false to clear them.</param>
        private void RefreshWorkflowList(bool reapplyFilters)
        {
            if (!reapplyFilters)
            {
                tstxtSearch.Text = string.Empty;
                tscmbFilterProcessType.SelectedIndex = 0;
            }
            btnLoadWorkflows_Click(null, null);
        }

        private void dgvWorkflows_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
    }
}
