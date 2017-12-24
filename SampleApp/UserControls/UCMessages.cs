using System;
using System.Windows.Forms;
using ZulipNetCore;

namespace SampleApp.UserControls {
    public partial class UCMessages : UserControl {
        public UCMessages() {
            InitializeComponent();
            AddHandlers();
            InitControls();
            ViewHelpers.DataGridViewHelper.SetDGVProperties(dgvMessages);
            btnGet.Click += btnGet_Click;
        }

        private void AddHandlers() {
            lnkFillCombos.LinkClicked += new LinkLabelLinkClickedEventHandler(lnkFillCombos_LinkClicked);
            btnSendToPrivate.Click += BtnSendToPrivate_Click;
            btnSendToStream.Click += BtnSendToStream_Click;
        }

        private void BtnSendToStream_Click(object sender, EventArgs e) {
            if (cboStreams.SelectedValue != null && txtStreamMsg.Text != "" && txtStreamTopic.Text != "") {
                try {
                    
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void BtnSendToPrivate_Click(object sender, EventArgs e) {
            if (cboUsers.SelectedValue != null && txtPrivateMsg.Text != "") {
                try {
                    var pm = new PrivateMessage(Program.client);
                    pm.PostPrivateMessage(cboUsers.SelectedValue.ToString(), txtPrivateMsg.Text);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private async void btnGet_Click(object sender, System.EventArgs e) {
            Program.GetZulipClient();
            Messages msgs = new Messages(Program.client);
            try {
                await msgs.GetMessagesAsync();
                dgvMessages.DataSource = msgs.MessageCollection;
                txtResponse.Text = msgs.JsonOutput;
            } catch (System.Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void InitControls() {
            txtPrivateMsg.Text = $"new private message sent via .NET client at {DateTime.Now}";
            txtStreamMsg.Text = $"new stream message sent via .NET client at {DateTime.Now}";
            txtStreamTopic.Text = "API Test";
        }

        private async void lnkFillCombos_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Program.GetZulipClient();
            try {
                Users users = new Users(Program.client);
                await users.GetUsersAsync();
                cboUsers.DisplayMember = nameof(User.FullName);
                cboUsers.ValueMember = nameof(User.Email);
                cboUsers.DataSource = users.UserCollection;

                Streams streams = new Streams(Program.client);
                await streams.GetStreamsAsync();
                cboStreams.DisplayMember = nameof(Stream.Name);
                cboStreams.ValueMember = nameof(Stream.Name);
                cboStreams.DataSource = streams.StreamCollection;

            } catch (System.Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}