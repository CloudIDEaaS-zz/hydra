// file:	MarketingControls\SocialMedia\ctrlSocialMediaList.cs
//
// summary:	Implements the control social media list class

using AbstraX.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace AbstraX.MarketingControls.SocialMedia
{
    /// <summary>   List of control social medias. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/7/2021. </remarks>

    public partial class ctrlSocialMediaList : UserControl, IPropertyOwner
    {
        /// <summary>   Gets or sets a list of social medias. </summary>
        ///
        /// <value> A list of social medias. </value>

        private SocialMediaList socialMediaList;
        private SocialMediaList socialMediaListOriginal;
        private List<SocialMediaEntryViewable> socialMediaListViewable;
        private bool deferCellFormatting;
        private frmUrlEditPopup frmUrlEditPopup;

        /// <summary>   Event queue for all listeners interested in UpdateEntry events. </summary>
        public event EventHandlerT<SocialMediaEntry> UpdateEntry;

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/7/2021. </remarks>

        public ctrlSocialMediaList()
        {
            InitializeComponent();
        }

        /// <summary>   Gets or sets a list of social medias. </summary>
        ///
        /// <value> A list of social medias. </value> 

        public SocialMediaList SocialMediaList
        {
            get
            {
                return socialMediaList;
            }

            set
            {
                socialMediaList = value;
                FillGrid();
            }
        }

        /// <summary>   Gets or sets the social media list original. </summary>
        ///
        /// <value> The social media list original. </value>

        public SocialMediaList SocialMediaListOriginal
        {
            get
            {
                return socialMediaListOriginal;
            }

            set
            {
                socialMediaListOriginal = value;
            }
        }

        private void FillGrid()
        {
            if (this.SocialMediaList != null)
            {
                socialMediaListViewable = this.SocialMediaList.SocialMedia.Select(m => new SocialMediaEntryViewable
                {
                    SmallLogoImage = m.GetSmallLogoImage(),
                    SmallLogo = m.SmallLogo,
                    TinyLogo = m.TinyLogo,
                    LargeLogo = m.LargeLogo,
                    Name = m.Name,
                    AccountUrl = m.AccountUrl,
                    ShareUrl = m.ShareUrl,
                    VisitCallToAction = m.VisitCallToAction,
                    ShareCallToAction = m.ShareCallToAction,
                    SiteUrl = m.SiteUrl,
                    WorkingDirectory = m.WorkingDirectory,
                    Enable = m.Enable
                }).ToList();

                socialMediaListBindingSource.DataSource = socialMediaListViewable;
            }
        }

        private void socialMediaGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (!deferCellFormatting)
            {
                var rowIndex = e.RowIndex;
                var row = socialMediaGridView.Rows[rowIndex];
                var entry = socialMediaListViewable[rowIndex];

                if (entry.Enable)
                {
                    row.DefaultCellStyle.ForeColor = Color.Black;

                    foreach (var cell in row.Cells.Cast<DataGridViewCell>().Where(c => c.GetColumn().IsOneOf(visitCallToActionDataGridViewTextBoxColumn, shareCallToActionDataGridViewTextBoxColumn)))
                    {
                        cell.ReadOnly = false;
                    }

                    if (socialMediaGridView.CurrentRow.Index == rowIndex)
                    {
                        foreach (var cell in row.Cells.Cast<DataGridViewCell>().Where(c => c.GetColumn().IsOneOf(accountUrlDataGridViewTextBoxColumn, shareUrlDataGridViewTextBoxColumn)))
                        {
                            Rectangle cellRect;
                            ctrlSplitCheckbox ctrlSplitCheckbox = null;
                            var column = cell.GetColumn();

                            deferCellFormatting = true;

                            cellRect = cell.GetDisplayRectangle();

                            if (column == accountUrlDataGridViewTextBoxColumn)
                            {
                                ctrlSplitCheckbox = ctrlSplitCheckboxAccountUrl;
                            }
                            else if (column == shareUrlDataGridViewTextBoxColumn)
                            {
                                ctrlSplitCheckbox = ctrlSplitCheckboxShareUrl;
                            }

                            if (ctrlSplitCheckbox != null)
                            {
                                if (!socialMediaGridView.Controls.Contains(ctrlSplitCheckbox))
                                {
                                    socialMediaGridView.Controls.Add(ctrlSplitCheckbox);
                                }

                                ctrlSplitCheckbox.Left = cellRect.X + (cellRect.Width - ctrlSplitCheckbox.Width);
                                ctrlSplitCheckbox.Top = cellRect.Y;
                                ctrlSplitCheckbox.Height = cellRect.Height;
                                ctrlSplitCheckbox.Tag = cell;

                                ctrlSplitCheckbox.Visible = true;
                            }

                            deferCellFormatting = false;
                        }
                    }
                }
                else
                {
                    row.DefaultCellStyle.ForeColor = Color.Gray;

                    foreach (var cell in row.Cells.Cast<DataGridViewCell>().Where(c => c.GetColumn().IsOneOf(visitCallToActionDataGridViewTextBoxColumn, shareCallToActionDataGridViewTextBoxColumn)))
                    {
                        cell.ReadOnly = true;
                    }
                }
            }
        }

        private void socialMediaGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var rowIndex = e.RowIndex;

            if (rowIndex != -1)
            {
                var columnIndex = e.ColumnIndex;
                var row = socialMediaGridView.Rows[rowIndex];
                var cell = row.Cells[columnIndex];

                if (cell is DataGridViewCheckBoxCell dataGridViewCheckBoxCell)
                {
                    var entry = socialMediaListViewable[rowIndex];

                    socialMediaGridView.Update();

                    UpdateEntry.Raise(this, entry);
                }
            }
        }

        private void socialMediaGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var rowIndex = e.RowIndex;

            if (rowIndex != -1)
            {
                var columnIndex = e.ColumnIndex;
                var row = socialMediaGridView.Rows[rowIndex];
                var cell = row.Cells[columnIndex];

                if (frmUrlEditPopup != null)
                {
                    frmUrlEditPopup.Dispose();
                }

                if (cell is DataGridViewCheckBoxCell dataGridViewCheckBoxCell)
                {
                    var entry = socialMediaListViewable[rowIndex];

                    ctrlSplitCheckboxAccountUrl.Visible = false;
                    ctrlSplitCheckboxShareUrl.Visible = false;

                    socialMediaGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
        }

        private void socialMediaGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            var rowIndex = e.RowIndex;

            if (rowIndex != -1)
            {
                var columnIndex = e.ColumnIndex;
                var row = socialMediaGridView.Rows[rowIndex];
                var cell = row.Cells[columnIndex];

                ctrlSplitCheckboxAccountUrl.Visible = false;
                ctrlSplitCheckboxShareUrl.Visible = false;

                if (cell is DataGridViewCheckBoxCell dataGridViewCheckBoxCell)
                {
                    var entry = socialMediaListViewable[rowIndex];

                    socialMediaGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
        }

        private void ctrlSplitCheckboxAccountUrl_CheckedChanged(object sender, EventArgs e)
        {
            if (ctrlSplitCheckboxAccountUrl.Checked)
            {
                var cell = (DataGridViewCell)ctrlSplitCheckboxAccountUrl.Tag;
                var cellRect = cell.GetDisplayRectangle();
                var entry = this.SocialMediaList.SocialMedia[cell.RowIndex];
                var entryOriginal = this.SocialMediaListOriginal.SocialMedia[cell.RowIndex];
                var entryViewable = socialMediaListViewable[cell.RowIndex];
                var propertyInfo = entryViewable.GetProperty("AccountUrl");

                Debug.WriteLine("Checked");

                frmUrlEditPopup = new frmUrlEditPopup(this, entry, propertyInfo, entryOriginal.AccountUrl, cell.Value);

                frmUrlEditPopup.Deactivate += (sender2, e2) =>
                {
                    frmUrlEditPopup.Dispose();
                };

                frmUrlEditPopup.Disposed += (sender2, e2) =>
                {
                    if (ctrlSplitCheckboxAccountUrl.Checked)
                    {
                        var focusControl = this.GetFocus();
                        var clickControl = this.GetControlAtCursor();

                        if (focusControl != ctrlSplitCheckboxAccountUrl || clickControl != ctrlSplitCheckboxAccountUrl)
                        {
                            Debug.WriteLine("Unchecking in code");

                            ctrlSplitCheckboxAccountUrl.Checked = false;
                        }
                    }
                };

                cellRect = socialMediaGridView.RectangleToScreen(cellRect);
                cellRect = this.Parent.RectangleToClient(cellRect);

                frmUrlEditPopup.Location = new Point(cellRect.Left, cellRect.Bottom);

                frmUrlEditPopup.BringToFront();

                frmUrlEditPopup.Show();
            }
            else
            {
                Debug.WriteLine("Unchecked");
                frmUrlEditPopup.Dispose();
            }
        }

        private void ctrlSplitCheckboxShareUrl_CheckedChanged(object sender, EventArgs e)
        {
            if (ctrlSplitCheckboxShareUrl.Checked)
            {
                var cell = (DataGridViewCell)ctrlSplitCheckboxShareUrl.Tag;
                var cellRect = cell.GetDisplayRectangle();
                var entry = this.SocialMediaList.SocialMedia[cell.RowIndex];
                var entryOriginal = this.SocialMediaListOriginal.SocialMedia[cell.RowIndex];
                var entryViewable = socialMediaListViewable[cell.RowIndex];
                var propertyInfo = entryViewable.GetProperty("ShareUrl");

                Debug.WriteLine("Checked");

                frmUrlEditPopup = new frmUrlEditPopup(this, entry, propertyInfo, entryOriginal.ShareUrl, cell.Value);

                frmUrlEditPopup.Deactivate += (sender2, e2) =>
                {
                    frmUrlEditPopup.Dispose();
                };

                frmUrlEditPopup.Disposed += (sender2, e2) =>
                {
                    if (ctrlSplitCheckboxShareUrl.Checked)
                    {
                        var focusControl = this.GetFocus();
                        var clickControl = this.GetControlAtCursor();

                        if (focusControl != ctrlSplitCheckboxShareUrl || clickControl != ctrlSplitCheckboxShareUrl)
                        {
                            Debug.WriteLine("Unchecking in code");

                            ctrlSplitCheckboxShareUrl.Checked = false;
                        }
                    }
                };

                cellRect = socialMediaGridView.RectangleToScreen(cellRect);
                cellRect = this.Parent.RectangleToClient(cellRect);

                frmUrlEditPopup.Location = new Point(cellRect.Left, cellRect.Bottom);

                frmUrlEditPopup.BringToFront();

                frmUrlEditPopup.Show();
            }
            else
            {
                Debug.WriteLine("Unchecked");
                frmUrlEditPopup.Dispose();
            }
        }

        private void socialMediaGridView_Click(object sender, EventArgs e)
        {
            if (frmUrlEditPopup != null)
            {
                frmUrlEditPopup.Dispose();
            }
        }

        private void socialMediaGridView_Leave(object sender, EventArgs e)
        {
            if (frmUrlEditPopup != null)
            {
                frmUrlEditPopup.Dispose();
            }
        }

        /// <summary>   Property changed. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2021. </remarks>
        ///
        /// <param name="sender">           Source of the event. </param>
        /// <param name="socialMediaEntry"> The social media entry. </param>

        public void PropertyChanged(object sender, SocialMediaEntry socialMediaEntry)
        {
            var socialMediaEntryViewable = socialMediaListViewable.Single(e => e.Name == socialMediaEntry.Name);
            var dataSource = (BindingSource) socialMediaGridView.DataSource;

            socialMediaEntryViewable.AccountUrl = socialMediaEntry.AccountUrl;
            socialMediaEntryViewable.ShareUrl = socialMediaEntry.ShareUrl;
            socialMediaEntryViewable.VisitCallToAction = socialMediaEntry.VisitCallToAction;
            socialMediaEntryViewable.ShareCallToAction = socialMediaEntry.ShareCallToAction;

            dataSource.ResetBindings(false);
            socialMediaGridView.Update();

            UpdateEntry.Raise(this, socialMediaEntry);
        }

        /// <summary>   Property changed. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/11/2021. </remarks>
        ///
        /// <param name="sender">       Source of the event. </param>
        /// <param name="tellOthers">   The tell others. </param>

        public void PropertyChanged(object sender, TellOthers tellOthers)
        {
            throw new NotImplementedException();
        }

        private void socialMediaGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var rowIndex = e.RowIndex;

            if (rowIndex != -1)
            {
                var columnIndex = e.ColumnIndex;
                var row = socialMediaGridView.Rows[rowIndex];
                var cell = row.Cells[columnIndex];

                if (cell is DataGridViewTextBoxCell dataGridViewTextBoxCell)
                {
                    var entry = socialMediaListViewable[rowIndex];

                    UpdateEntry.Raise(this, entry);
                }
            }
        }
    }
}
