using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using WorkflowEnabler.Forms;

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

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            //ShowInfoNotification("This plugin allows you to manage workflows and Power Automate flows. Feature requests and contributions are welcome via our GitHub repository.",
            //    new Uri("https://github.com/saddam-cheikh/WorkflowEnabler"));

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

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void cmbFilterProcessType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

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

        private List<Entity> GetSolutions()
        {
            var query = new QueryExpression("solution")
            {
                ColumnSet = new ColumnSet("friendlyname", "solutionid")
            };

            var result = Service.RetrieveMultiple(query);
            return result.Entities.ToList();
        }

        private Guid ShowSolutionSelectionDialog()
        {
            using (var sPicker = new SolutionPicker(Service))
            {
                if (sPicker.ShowDialog() != DialogResult.OK || sPicker.SelectedSolution == null || !sPicker.SelectedSolution.Any())
                {
                    return Guid.Empty;
                }

                return sPicker.SelectedSolution.First().Id;
            }
        }

        private void btnLoadWorkflows_Click(object sender, EventArgs e)
        {
            currentSolutionId = ShowSolutionSelectionDialog();
            if (currentSolutionId == Guid.Empty)
                return;

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
                        return;
                    }

                    dgvWorkflows.Rows.Clear();
                    workflowInfo.Clear();

                    var workflows = args.Result as List<Entity>;
                    if (workflows == null)
                        return;

                    foreach (var wf in workflows)
                    {
                        string status = wf.GetAttributeValue<OptionSetValue>("statecode")?.Value == 1 ? "Active" : "Draft";
                        int category = (int)(wf.GetAttributeValue<OptionSetValue>("category")?.Value);

                        string processType;
                        if (category == 0) processType = "Workflow";
                        else if (category == 1) processType = "Dialog";
                        else if (category == 2) processType = "Business Rule";
                        else if (category == 3) processType = "Action";
                        else if (category == 4) processType = "BPF (Business Process Flow)";
                        else if (category == 5) processType = "Cloud Flow (Modern Flow)";
                        else if (category == 6) processType = "Desktop Flow";
                        else if (category == 7) processType = "AI Flow";
                        else if (category == 9000) processType = "Web Client API Flow";
                        else processType = "Unknown";

                        DateTime createdOn = wf.GetAttributeValue<DateTime>("createdon");
                        DateTime modifiedOn = wf.GetAttributeValue<DateTime>("modifiedon");
                        EntityReference owner = wf.GetAttributeValue<EntityReference>("ownerid");
                        bool isManaged = wf.GetAttributeValue<bool>("ismanaged");

                        dgvWorkflows.Rows.Add(
                            wf.GetAttributeValue<string>("name"),
                            wf.Id.ToString(),
                            processType,
                            status,
                            createdOn.ToString("g"),
                            modifiedOn.ToString("g"),
                            owner != null ? owner.Name : "",
                            isManaged ? "Yes" : "No"
                        );

                        workflowInfo[wf.Id] = wf.GetAttributeValue<string>("name");
                    }

                    ApplyFilters();
                }
            });
        }

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

        private void btnExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV Files (*.csv)|*.csv";
                sfd.FileName = "Workflows_Export.csv";

                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

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

            var workflows = new List<Entity>();
            foreach (var record in result.Entities)
            {
                Guid workflowId = record.GetAttributeValue<Guid>("objectid");
                Entity workflow = Service.Retrieve("workflow", workflowId,
                    new ColumnSet("name", "workflowid", "statecode", "statuscode", "category", "createdon", "modifiedon", "ownerid", "ismanaged"));
                workflows.Add(workflow);
            }

            return workflows;
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            ChangeWorkflowStatus(true);
        }

        private void btnDeactivate_Click(object sender, EventArgs e)
        {
            ChangeWorkflowStatus(false);
        }

        private void ChangeWorkflowStatus(bool activate)
        {
            if (dgvWorkflows.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select one or more workflows.", "No selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Build targets + skipped (based on current UI status)
            var targets = new List<(Guid Id, string Name)>();
            var skipped = new List<string>();

            foreach (DataGridViewRow row in dgvWorkflows.SelectedRows)
            {
                var idStr = row.Cells["colID"].Value?.ToString();
                var name = row.Cells["colNom"].Value?.ToString();
                var status = row.Cells["colStatut"].Value?.ToString(); // "Active"/"Draft"

                if (!Guid.TryParse(idStr, out var id) || string.IsNullOrWhiteSpace(name))
                    continue;

                bool alreadyInDesiredState =
                    activate
                        ? status?.Equals("Active", StringComparison.OrdinalIgnoreCase) == true
                        : status?.Equals("Draft", StringComparison.OrdinalIgnoreCase) == true;

                if (alreadyInDesiredState)
                    skipped.Add(name);
                else
                    targets.Add((id, name));
            }

            if (targets.Count == 0)
            {
                MessageBox.Show(
                    activate ? "All selected workflows are already Active." : "All selected workflows are already Draft.",
                    "Nothing to do",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // Confirmation
            var confirm = MessageBox.Show(
                $"You are about to {(activate ? "activate" : "deactivate")} {targets.Count} workflow(s).\nDo you want to continue?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            WorkAsync(new WorkAsyncInfo
            {
                Message = activate ? "Activating workflows..." : "Deactivating workflows...",
                Work = (worker, args) =>
                {
                    var req = new ExecuteMultipleRequest
                    {
                        Settings = new ExecuteMultipleSettings
                        {
                            ContinueOnError = true,
                            ReturnResponses = true
                        },
                        Requests = new OrganizationRequestCollection()
                    };

                    foreach (var t in targets)
                    {
                        req.Requests.Add(new UpdateRequest
                        {
                            Target = new Entity("workflow", t.Id)
                            {
                                ["statecode"] = new OptionSetValue(activate ? 1 : 0),
                                ["statuscode"] = new OptionSetValue(activate ? 2 : 1)
                            }
                        });
                    }

                    var resp = (ExecuteMultipleResponse)Service.Execute(req);
                    args.Result = new Tuple<ExecuteMultipleResponse, List<(Guid Id, string Name)>>(resp, targets);
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var tuple = (Tuple<ExecuteMultipleResponse, List<(Guid Id, string Name)>>)args.Result;
                    var response = tuple.Item1;
                    var sentTargets = tuple.Item2;

                    var succeeded = new List<string>();
                    var succeededIds = new HashSet<Guid>();
                    var failed = new List<(string Name, string Error)>();

                    foreach (var item in response.Responses)
                    {
                        var idx = item.RequestIndex;
                        var wf = (idx >= 0 && idx < sentTargets.Count) ? sentTargets[idx] : (Guid.Empty, "Unknown");

                        if (item.Fault != null)
                        {
                            failed.Add((wf.Item2, item.Fault.Message));
                        }
                        else
                        {
                            succeeded.Add(wf.Item2);
                            if (wf.Item1 != Guid.Empty)
                                succeededIds.Add(wf.Item1);
                        }
                    }

                    // Best-effort accounting for missing responses (safety)
                    if (succeeded.Count + failed.Count < sentTargets.Count)
                    {
                        var accounted = new HashSet<string>(succeeded.Concat(failed.Select(f => f.Name)));
                        foreach (var wf in sentTargets)
                        {
                            if (!accounted.Contains(wf.Name))
                            {
                                succeeded.Add(wf.Name);
                                succeededIds.Add(wf.Id);
                            }
                        }
                    }

                    // Update UI ONLY for succeeded (no reload)
                    UpdateGridStatus(succeededIds, activate);

                    // Summary + details
                    ShowOperationSummary(
                        activate ? "Activation" : "Deactivation",
                        succeeded,
                        skipped,
                        failed
                    );
                }
            });
        }

        private void UpdateGridStatus(HashSet<Guid> succeededIds, bool activate)
        {
            if (succeededIds == null || succeededIds.Count == 0)
                return;

            foreach (DataGridViewRow row in dgvWorkflows.Rows)
            {
                if (row.IsNewRow) continue;

                var idStr = row.Cells["colID"].Value?.ToString();
                if (!Guid.TryParse(idStr, out var id)) continue;

                if (!succeededIds.Contains(id)) continue;

                row.Cells["colStatut"].Value = activate ? "Active" : "Draft";
            }

            // Keep filters and repaint
            ApplyFilters();
            dgvWorkflows.Refresh();
        }

        private void ShowOperationSummary(
            string operation,
            List<string> succeeded,
            List<string> skipped,
            List<(string Name, string Error)> failed)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{operation} completed");
            sb.AppendLine();
            sb.AppendLine($"✅ Succeeded: {succeeded?.Count ?? 0}");
            sb.AppendLine($"🟡 Skipped: {skipped?.Count ?? 0}");
            sb.AppendLine($"❌ Errors: {failed?.Count ?? 0}");

            var icon = (failed != null && failed.Count > 0) ? MessageBoxIcon.Warning : MessageBoxIcon.Information;

            MessageBox.Show(sb.ToString(), $"{operation} result", MessageBoxButtons.OK, icon);

            // Show details only if something happened (or if you prefer: only if failed/skipped)
            if ((succeeded?.Count ?? 0) + (skipped?.Count ?? 0) + (failed?.Count ?? 0) > 0)
            {
                using (var f = new ResultsForm(operation, succeeded ?? new List<string>(), skipped ?? new List<string>(), failed ?? new List<(string Name, string Error)>()))
                {
                    f.ShowDialog();
                }
            }
        }

        // Kept in case you still use it elsewhere, but not called after Activate/Deactivate anymore.
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
